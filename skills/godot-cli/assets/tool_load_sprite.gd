extends SceneTree
const ToolCommon = preload("res://tools/tool_common.gd")

func _init() -> void:
	var payload := ToolCommon.parse_single_payload(self, OS.get_cmdline_args())
	if payload.has("__codex_error__"):
		return
	var raw_scene_path = ToolCommon.require_key(self, payload, "scene_path")
	var raw_node_path = ToolCommon.require_key(self, payload, "node_path")
	var raw_texture_path = ToolCommon.require_key(self, payload, "texture_path")
	if raw_scene_path == null or raw_node_path == null or raw_texture_path == null:
		return
	var scene_path := String(raw_scene_path)
	var node_path := String(raw_node_path)
	var texture_path := String(raw_texture_path)
	var scene_root := ToolCommon.instantiate_scene_root(self, scene_path)
	if scene_root == null:
		return
	var sprite_node := ToolCommon.resolve_node_path(scene_root, node_path)
	if sprite_node == null:
		ToolCommon.fail(self, "node not found: %s" % node_path)
		return
	if not (sprite_node is Sprite2D or sprite_node is Sprite3D or sprite_node is TextureRect):
		ToolCommon.fail(self, "node is not sprite-compatible")
		return
	var texture = load(ToolCommon.to_res_path(texture_path))
	if texture == null:
		ToolCommon.fail(self, "texture not found: %s" % texture_path)
		return
	sprite_node.texture = texture
	ToolCommon.save_scene_root(self, scene_root, scene_path)
	if ToolCommon.has_failed(self):
		return
	print("loaded texture: %s" % texture_path)
	quit()
