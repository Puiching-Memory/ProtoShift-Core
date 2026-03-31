extends SceneTree
const ToolCommon = preload("res://tools/tool_common.gd")

func _init() -> void:
	var payload := ToolCommon.parse_single_payload(self, OS.get_cmdline_args())
	if payload.has("__codex_error__"):
		return
	var raw_scene_path = ToolCommon.require_key(self, payload, "scene_path")
	var raw_node_type = ToolCommon.require_key(self, payload, "node_type")
	var raw_node_name = ToolCommon.require_key(self, payload, "node_name")
	if raw_scene_path == null or raw_node_type == null or raw_node_name == null:
		return
	var scene_path := String(raw_scene_path)
	var node_type := String(raw_node_type)
	var node_name := String(raw_node_name)
	var parent_path := String(payload.get("parent_node_path", "root"))
	var scene_root := ToolCommon.instantiate_scene_root(self, scene_path)
	if scene_root == null:
		return
	var parent := ToolCommon.resolve_node_path(scene_root, parent_path)
	if parent == null:
		ToolCommon.fail(self, "parent node not found: %s" % parent_path)
		return
	var new_node = ToolCommon.instantiate_type(self, node_type)
	if new_node == null or not (new_node is Node):
		ToolCommon.fail(self, "node_type must create a Node")
		return
	new_node.name = node_name
	var properties: Dictionary = payload.get("properties", {})
	for property_name in properties.keys():
		new_node.set(property_name, ToolCommon.maybe_load_resource(properties[property_name]))
	parent.add_child(new_node)
	new_node.owner = scene_root
	ToolCommon.save_scene_root(self, scene_root, scene_path)
	if ToolCommon.has_failed(self):
		return
	print("added node: %s" % node_name)
	quit()
