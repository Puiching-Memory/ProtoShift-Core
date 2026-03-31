extends RefCounted

static func fail(tree: SceneTree, message: String) -> void:
	tree.set_meta("__codex_failed__", true)
	printerr("[ERROR] %s" % message)
	tree.quit(1)

static func has_failed(tree: SceneTree) -> bool:
	return tree.has_meta("__codex_failed__") and bool(tree.get_meta("__codex_failed__"))

static func parse_single_payload(tree: SceneTree, args: PackedStringArray) -> Dictionary:
	var script_index := args.find("--script")
	if script_index == -1:
		fail(tree, "missing --script argument")
		return {"__codex_error__": true}
	if args.size() <= script_index + 2:
		fail(tree, "expected: --script <script> <json-payload>")
		return {"__codex_error__": true}
	var raw := String(args[script_index + 2])
	if raw.begins_with("@"):
		var payload_path := raw.trim_prefix("@")
		var payload_file := FileAccess.open(payload_path, FileAccess.READ)
		if payload_file == null:
			fail(tree, "cannot open payload file: %s" % payload_path)
			return {"__codex_error__": true}
		raw = payload_file.get_as_text()
		payload_file.close()
	var json := JSON.new()
	var error := json.parse(raw)
	if error != OK:
		fail(tree, "invalid json payload")
		return {"__codex_error__": true}
	var data = json.get_data()
	if typeof(data) != TYPE_DICTIONARY:
		fail(tree, "payload must be a dictionary")
		return {"__codex_error__": true}
	return data

static func require_key(tree: SceneTree, payload: Dictionary, key: String) -> Variant:
	if not payload.has(key):
		fail(tree, "missing key: %s" % key)
		return null
	return payload[key]

static func to_res_path(path: String) -> String:
	var normalized := path.replace("\\", "/")
	if normalized.begins_with("res://"):
		return normalized
	while normalized.begins_with("/"):
		normalized = normalized.substr(1)
	return "res://%s" % normalized

static func ensure_parent_dir(tree: SceneTree, res_path: String) -> void:
	var full_path := to_res_path(res_path)
	var base_dir := full_path.get_base_dir()
	if base_dir == "res://":
		return
	var relative_dir := base_dir.replace("res://", "")
	var dir := DirAccess.open("res://")
	if dir == null:
		fail(tree, "cannot open project root")
		return
	var error := dir.make_dir_recursive(relative_dir)
	if error != OK and error != ERR_ALREADY_EXISTS:
		fail(tree, "cannot create dir: %s" % relative_dir)

static func load_packed_scene(tree: SceneTree, scene_path: String) -> PackedScene:
	var full_scene_path := to_res_path(scene_path)
	if not ResourceLoader.exists(full_scene_path):
		fail(tree, "scene not found: %s" % scene_path)
		return null
	var packed := load(full_scene_path) as PackedScene
	if packed == null:
		fail(tree, "failed to load scene: %s" % scene_path)
		return null
	return packed

static func instantiate_scene_root(tree: SceneTree, scene_path: String) -> Node:
	var packed := load_packed_scene(tree, scene_path)
	if packed == null:
		return null
	var root := packed.instantiate()
	if root == null:
		fail(tree, "failed to instantiate scene: %s" % scene_path)
		return null
	return root

static func save_scene_root(tree: SceneTree, scene_root: Node, target_path: String) -> void:
	var packed := PackedScene.new()
	var pack_error := packed.pack(scene_root)
	if pack_error != OK:
		fail(tree, "failed to pack scene")
		return
	ensure_parent_dir(tree, target_path)
	if has_failed(tree):
		return
	var save_error := ResourceSaver.save(packed, to_res_path(target_path))
	if save_error != OK:
		fail(tree, "failed to save scene: %s" % target_path)

static func get_script_by_name_or_path(type_name: String) -> Script:
	var raw := type_name.replace("\\", "/")
	if raw.begins_with("res://") and ResourceLoader.exists(raw, "Script"):
		return load(raw) as Script
	if raw.contains("/"):
		var res_path := to_res_path(raw)
		if ResourceLoader.exists(res_path, "Script"):
			return load(res_path) as Script
	for global_class in ProjectSettings.get_global_class_list():
		if global_class["class"] == type_name:
			return load(global_class["path"]) as Script
	return null

static func instantiate_type(tree: SceneTree, type_name: String) -> Variant:
	if ClassDB.class_exists(type_name):
		if not ClassDB.can_instantiate(type_name):
			fail(tree, "type cannot be instantiated: %s" % type_name)
			return null
		return ClassDB.instantiate(type_name)
	var script := get_script_by_name_or_path(type_name)
	if script is GDScript:
		return script.new()
	fail(tree, "unknown type: %s" % type_name)
	return null

static func resolve_node_path(scene_root: Node, raw_path: String) -> Node:
	if raw_path.is_empty() or raw_path == "root":
		return scene_root
	var normalized := raw_path
	if normalized.begins_with("root/"):
		normalized = normalized.substr(5)
	return scene_root.get_node_or_null(normalized)

static func maybe_load_resource(value: Variant) -> Variant:
	if typeof(value) != TYPE_STRING:
		return value
	var raw := String(value)
	var res_path := to_res_path(raw)
	if ResourceLoader.exists(res_path):
		return load(res_path)
	return value

static func find_first_mesh_instance(node: Node) -> MeshInstance3D:
	if node is MeshInstance3D:
		return node
	for child in node.get_children():
		if child is MeshInstance3D:
			return child
	return null

static func find_files(path: String, extension: String) -> Array:
	var files: Array = []
	var dir := DirAccess.open(path)
	if dir == null:
		return files
	dir.list_dir_begin()
	var entry := dir.get_next()
	while entry != "":
		var child_path := path.path_join(entry)
		if dir.current_is_dir() and not entry.begins_with("."):
			files.append_array(find_files(child_path, extension))
		elif entry.ends_with(extension):
			files.append(child_path)
		entry = dir.get_next()
	return files
