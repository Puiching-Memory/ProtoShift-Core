extends SceneTree
const ToolCommon = preload("res://tools/tool_common.gd")

func _init() -> void:
	var payload := ToolCommon.parse_single_payload(self, OS.get_cmdline_args())
	if payload.has("__codex_error__"):
		return
	var raw_scene_path = ToolCommon.require_key(self, payload, "scene_path")
	var raw_output_path = ToolCommon.require_key(self, payload, "output_path")
	if raw_scene_path == null or raw_output_path == null:
		return
	var scene_path := String(raw_scene_path)
	var output_path := String(raw_output_path)
	var allowed_names: Array = payload.get("mesh_item_names", [])
	var filter_enabled := allowed_names.size() > 0
	var scene_root := ToolCommon.instantiate_scene_root(self, scene_path)
	if scene_root == null:
		return
	var library := MeshLibrary.new()
	var item_id := 0
	for child in scene_root.get_children():
		if filter_enabled and not allowed_names.has(child.name):
			continue
		var mesh_instance := ToolCommon.find_first_mesh_instance(child)
		if mesh_instance == null or mesh_instance.mesh == null:
			continue
		library.create_item(item_id)
		library.set_item_name(item_id, child.name)
		library.set_item_mesh(item_id, mesh_instance.mesh)
		library.set_item_preview(item_id, mesh_instance.mesh)
		for collision_child in child.get_children():
			if collision_child is CollisionShape3D and collision_child.shape:
				library.set_item_shapes(item_id, [collision_child.shape])
				break
		item_id += 1
	if item_id == 0:
		ToolCommon.fail(self, "no valid meshes found")
		return
	ToolCommon.ensure_parent_dir(self, output_path)
	if ToolCommon.has_failed(self):
		return
	var save_error := ResourceSaver.save(library, ToolCommon.to_res_path(output_path))
	if save_error != OK:
		ToolCommon.fail(self, "failed to save mesh library")
		return
	print("exported mesh library: %s" % output_path)
	quit()
