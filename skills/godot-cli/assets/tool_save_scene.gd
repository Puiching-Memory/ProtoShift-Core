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
	var new_path := String(payload.get("new_path", scene_path))
	var scene_root := ToolCommon.instantiate_scene_root(self, scene_path)
	if scene_root == null:
		return
	ToolCommon.save_scene_root(self, scene_root, new_path)
	if ToolCommon.has_failed(self):
		return
	print("saved scene: %s" % new_path)
	quit()
