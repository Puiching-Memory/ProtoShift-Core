extends SceneTree
const ToolCommon = preload("res://tools/tool_common.gd")

var scene_path := ""
var output_path := ""
var camera_node_path := ""
var wait_frames := 4
var remaining_frames := 4
var resolution := Vector2i.ZERO
var scene_is_ready := false
var has_capture_started := false

func _init() -> void:
	var payload := ToolCommon.parse_single_payload(self, OS.get_cmdline_args())
	if payload.has("__codex_error__"):
		return
	var raw_scene_path = ToolCommon.require_key(self, payload, "scene_path")
	var raw_output_path = ToolCommon.require_key(self, payload, "output_path")
	if raw_scene_path == null or raw_output_path == null:
		return
	scene_path = String(raw_scene_path)
	output_path = String(raw_output_path)
	if not output_path.to_lower().ends_with(".png"):
		ToolCommon.fail(self, "output_path must end with .png")
		return
	wait_frames = max(1, int(payload.get("wait_frames", 4)))
	remaining_frames = wait_frames
	camera_node_path = String(payload.get("camera_node_path", ""))
	resolution = parse_resolution(payload.get("resolution", null))
	if ToolCommon.has_failed(self):
		return
	call_deferred("_setup_scene")

func _setup_scene() -> void:
	if ToolCommon.has_failed(self):
		return
	if resolution != Vector2i.ZERO:
		get_root().size = resolution
	var scene_root := ToolCommon.instantiate_scene_root(self, scene_path)
	if scene_root == null:
		return
	get_root().add_child(scene_root)
	current_scene = scene_root
	if not camera_node_path.is_empty():
		var camera_node := ToolCommon.resolve_node_path(scene_root, camera_node_path)
		if camera_node == null:
			ToolCommon.fail(self, "camera node not found: %s" % camera_node_path)
			return
		if not camera_node.has_method("make_current"):
			ToolCommon.fail(self, "camera node cannot become current: %s" % camera_node_path)
			return
		camera_node.call("make_current")
	scene_is_ready = true

func _process(_delta: float) -> bool:
	if ToolCommon.has_failed(self) or has_capture_started or not scene_is_ready:
		return false
	if remaining_frames > 0:
		remaining_frames -= 1
		return false
	has_capture_started = true
	ToolCommon.ensure_parent_dir(self, output_path)
	if ToolCommon.has_failed(self):
		return false
	var image := get_root().get_texture().get_image()
	if image == null:
		ToolCommon.fail(self, "failed to read viewport image")
		return false
	var save_error := image.save_png(ProjectSettings.globalize_path(ToolCommon.to_res_path(output_path)))
	if save_error != OK:
		ToolCommon.fail(self, "failed to save screenshot: %s" % output_path)
		return false
	print("captured scene: %s" % output_path)
	quit()
	return false

func parse_resolution(value: Variant) -> Vector2i:
	match typeof(value):
		TYPE_NIL:
			return Vector2i.ZERO
		TYPE_STRING:
			var parts := String(value).to_lower().split("x", false)
			if parts.size() != 2:
				ToolCommon.fail(self, "resolution must look like 1280x720")
				return Vector2i.ZERO
			return ensure_valid_resolution(Vector2i(int(parts[0]), int(parts[1])))
		TYPE_ARRAY:
			var items: Array = value
			if items.size() != 2:
				ToolCommon.fail(self, "resolution array must contain [width, height]")
				return Vector2i.ZERO
			return ensure_valid_resolution(Vector2i(int(items[0]), int(items[1])))
		TYPE_DICTIONARY:
			var data: Dictionary = value
			var width = data.get("width", data.get("x", null))
			var height = data.get("height", data.get("y", null))
			if width == null or height == null:
				ToolCommon.fail(self, "resolution dictionary must include width and height")
				return Vector2i.ZERO
			return ensure_valid_resolution(Vector2i(int(width), int(height)))
		_:
			ToolCommon.fail(self, "unsupported resolution payload")
			return Vector2i.ZERO

func ensure_valid_resolution(size: Vector2i) -> Vector2i:
	if size.x <= 0 or size.y <= 0:
		ToolCommon.fail(self, "resolution must be larger than zero")
		return Vector2i.ZERO
	return size
