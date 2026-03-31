extends SceneTree
const ToolCommon = preload("res://tools/tool_common.gd")

func _init() -> void:
	var payload := ToolCommon.parse_single_payload(self, OS.get_cmdline_args())
	if payload.has("__codex_error__"):
		return
	var root_path := String(payload.get("project_path", "res://"))
	var scan_root := root_path if root_path.begins_with("res://") else ToolCommon.to_res_path(root_path)
	if not scan_root.ends_with("/"):
		scan_root += "/"
	for scene_path in ToolCommon.find_files(scan_root, ".tscn"):
		var scene = load(scene_path)
		if scene:
			ResourceSaver.save(scene, scene_path)
	for resource_path in ToolCommon.find_files(scan_root, ".gd") + ToolCommon.find_files(scan_root, ".shader") + ToolCommon.find_files(scan_root, ".gdshader"):
		var uid_path := "%s.uid" % resource_path
		if FileAccess.file_exists(uid_path):
			continue
		var resource = load(resource_path)
		if resource:
			ResourceSaver.save(resource, resource_path)
	print("resaved resources under: %s" % scan_root)
	quit()
