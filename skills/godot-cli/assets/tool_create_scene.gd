extends SceneTree
const ToolCommon = preload("res://tools/tool_common.gd")


func _init() -> void:
	var payload := ToolCommon.parse_single_payload(self, OS.get_cmdline_args())
	if payload.has("__codex_error__"):
		return
	var raw_scene_path = ToolCommon.require_key(self, payload, "scene_path")
	if raw_scene_path == null:
		return
	var scene_path := String(raw_scene_path)
	var root_node_type := String(payload.get("root_node_type", "Node2D"))
	var scene_root = ToolCommon.instantiate_type(self, root_node_type)
	if scene_root == null or not (scene_root is Node):
		ToolCommon.fail(self, "root node type must create a Node")
		return
	scene_root.name = "root"
	scene_root.owner = scene_root
	ToolCommon.save_scene_root(self, scene_root, scene_path)
	if ToolCommon.has_failed(self):
		return
	print("created scene: %s" % scene_path)
	quit()
