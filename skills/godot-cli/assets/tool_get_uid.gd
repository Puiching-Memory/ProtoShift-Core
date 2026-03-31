extends SceneTree
const ToolCommon = preload("res://tools/tool_common.gd")

func _init() -> void:
	var payload := ToolCommon.parse_single_payload(self, OS.get_cmdline_args())
	if payload.has("__codex_error__"):
		return
	var raw_file_path = ToolCommon.require_key(self, payload, "file_path")
	if raw_file_path == null:
		return
	var file_path := String(raw_file_path)
	var res_path := ToolCommon.to_res_path(file_path)
	if not FileAccess.file_exists(res_path):
		ToolCommon.fail(self, "file not found: %s" % file_path)
		return
	var uid_path := "%s.uid" % res_path
	var result := {
		"file": file_path,
		"exists": false,
	}
	var handle := FileAccess.open(uid_path, FileAccess.READ)
	if handle:
		result["exists"] = true
		result["uid"] = handle.get_as_text().strip_edges()
		handle.close()
	else:
		result["message"] = "uid file does not exist"
	print(JSON.stringify(result))
	quit()

