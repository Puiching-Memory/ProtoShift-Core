/**
 * ProtoShift opencode 插件
 *
 * 注入 ProtoShift 的 hooks：
 * - 工具定义（引擎桥接命令）
 * - 上下文注入（SharedCore 状态、当前引擎）
 * - 权限控制
 */

/** @type {import("@opencode-ai/plugin").Plugin} */
export default async function protoshift(input) {
  return {
    // 自定义工具：暴露 ProtoShift 统一语义
    tool: {
      protoshift_open_project: {
        description: "打开引擎项目",
        parameters: {
          type: "object",
          properties: {
            engine: {
              type: "string",
              enum: ["godot", "unreal"],
              description: "目标引擎",
            },
            path: {
              type: "string",
              description: "项目路径",
            },
          },
          required: ["engine", "path"],
        },
        async execute(params) {
          return {
            output: `[ProtoShift] Open ${params.engine} project: ${params.path}`,
            metadata: { engine: params.engine, action: "open_project" },
          };
        },
      },

      protoshift_run_project: {
        description: "运行当前引擎项目",
        parameters: {
          type: "object",
          properties: {
            engine: {
              type: "string",
              enum: ["godot", "unreal"],
              description: "目标引擎",
            },
          },
          required: ["engine"],
        },
        async execute(params) {
          return {
            output: `[ProtoShift] Run ${params.engine} project`,
            metadata: { engine: params.engine, action: "run_project" },
          };
        },
      },

      protoshift_stop_project: {
        description: "停止当前运行的引擎项目",
        parameters: {
          type: "object",
          properties: {
            engine: {
              type: "string",
              enum: ["godot", "unreal"],
              description: "目标引擎",
            },
          },
          required: ["engine"],
        },
        async execute(params) {
          return {
            output: `[ProtoShift] Stop ${params.engine} project`,
            metadata: { engine: params.engine, action: "stop_project" },
          };
        },
      },

      protoshift_get_scene: {
        description: "获取场景/关卡结构",
        parameters: {
          type: "object",
          properties: {
            engine: {
              type: "string",
              enum: ["godot", "unreal"],
            },
            path: {
              type: "string",
              description: "场景路径",
            },
          },
          required: ["engine", "path"],
        },
        async execute(params) {
          return {
            output: `[ProtoShift] Get scene tree: ${params.engine}:${params.path}`,
            metadata: { engine: params.engine, action: "get_scene" },
          };
        },
      },

      protoshift_create_object: {
        description: "在场景中创建对象",
        parameters: {
          type: "object",
          properties: {
            engine: { type: "string", enum: ["godot", "unreal"] },
            parent: { type: "string", description: "父节点路径" },
            type: { type: "string", description: "对象类型" },
            name: { type: "string", description: "对象名称" },
          },
          required: ["engine", "parent", "type", "name"],
        },
        async execute(params) {
          return {
            output: `[ProtoShift] Create ${params.type} '${params.name}' under ${params.parent}`,
            metadata: { engine: params.engine, action: "create_object" },
          };
        },
      },

      protoshift_compile: {
        description: "编译或热重载引擎项目",
        parameters: {
          type: "object",
          properties: {
            engine: { type: "string", enum: ["godot", "unreal"] },
          },
          required: ["engine"],
        },
        async execute(params) {
          return {
            output: `[ProtoShift] Compile/Reload ${params.engine}`,
            metadata: { engine: params.engine, action: "compile" },
          };
        },
      },

      protoshift_export_migration: {
        description: "导出迁移模型",
        parameters: {
          type: "object",
          properties: {
            output: { type: "string", description: "输出路径" },
          },
          required: ["output"],
        },
        async execute(params) {
          return {
            output: `[ProtoShift] Export migration model to: ${params.output}`,
            metadata: { action: "export_migration" },
          };
        },
      },

      protoshift_import_migration: {
        description: "从迁移模型生成 Unreal 侧结构",
        parameters: {
          type: "object",
          properties: {
            model: { type: "string", description: "迁移模型路径" },
          },
          required: ["model"],
        },
        async execute(params) {
          return {
            output: `[ProtoShift] Import migration model: ${params.model}`,
            metadata: { action: "import_migration" },
          };
        },
      },
    },

    // 会话消息 hook：注入 ProtoShift 上下文
    "chat.message": async (message) => {
      // 可在此处注入当前项目状态、引擎连接状态等上下文
      return message;
    },

    // 工具执行前 hook：记录操作日志
    "tool.execute.before": async (tool) => {
      if (tool.name?.startsWith("protoshift_")) {
        console.log(`[ProtoShift] Executing: ${tool.name}`);
      }
      return tool;
    },
  };
}
