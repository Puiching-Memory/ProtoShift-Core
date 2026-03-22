import { copyFile, mkdir, readdir, readFile, rename, stat, writeFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { CallToolRequestSchema, ListToolsRequestSchema } from "@modelcontextprotocol/sdk/types.js";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const repoRoot = path.resolve(__dirname, "..", "..");
const templateRoot = path.join(repoRoot, "samples", "godot-standard-template");
const workspaceRoot = path.join(repoRoot, "workspace");

async function exists(targetPath) {
  try {
    await stat(targetPath);
    return true;
  } catch {
    return false;
  }
}

async function copyDirectory(sourceDir, targetDir) {
  await mkdir(targetDir, { recursive: true });
  const entries = await readdir(sourceDir, { withFileTypes: true });

  for (const entry of entries) {
    const sourcePath = path.join(sourceDir, entry.name);
    const targetPath = path.join(targetDir, entry.name);

    if (entry.isDirectory()) {
      await copyDirectory(sourcePath, targetPath);
      continue;
    }

    await copyFile(sourcePath, targetPath);
  }
}

async function replaceInFile(filePath, replacements) {
  const original = await readFile(filePath, "utf8");
  let updated = original;

  for (const [from, to] of replacements) {
    updated = updated.split(from).join(to);
  }

  if (updated !== original) {
    await writeFile(filePath, updated, "utf8");
  }
}

async function initializeGodotProject(args) {
  const projectSlug = String(args.projectSlug ?? "").trim();
  const displayName = String(args.displayName ?? "").trim();
  const assemblyName = String(args.assemblyName ?? displayName).trim();

  if (!projectSlug) {
    throw new Error("projectSlug is required");
  }

  if (!displayName) {
    throw new Error("displayName is required");
  }

  if (!assemblyName) {
    throw new Error("assemblyName is required");
  }

  if (!(await exists(templateRoot))) {
    throw new Error(`Godot standard template not found: ${templateRoot}`);
  }

  const targetDir = path.join(workspaceRoot, projectSlug);
  if (await exists(targetDir)) {
    throw new Error(`Target project already exists: ${targetDir}`);
  }

  await copyDirectory(templateRoot, targetDir);

  const sourceCsproj = path.join(targetDir, "ProtoShiftGame.csproj");
  const targetCsproj = path.join(targetDir, `${assemblyName}.csproj`);
  const replacements = [["ProtoShiftGame", assemblyName]];

  await replaceInFile(path.join(targetDir, "README.md"), replacements);
  await replaceInFile(path.join(targetDir, "project.godot"), replacements);
  await replaceInFile(path.join(targetDir, "scripts", "Main.cs"), replacements);
  await replaceInFile(sourceCsproj, replacements);
  await rename(sourceCsproj, targetCsproj);

  return {
    projectPath: targetDir,
    csprojPath: targetCsproj,
    nextSteps: [
      `dotnet build workspace/${projectSlug}/${assemblyName}.csproj`,
      `Use Godot MCP to open workspace/${projectSlug}/project.godot`,
      "Generate or modify gameplay scripts on top of the copied template",
      `Run dotnet build again before launching the project`,
    ],
  };
}

const server = new Server(
  {
    name: "protoshift-mcp",
    version: "0.1.0",
  },
  {
    capabilities: {
      tools: {},
    },
  },
);

server.setRequestHandler(ListToolsRequestSchema, async () => ({
  tools: [
    {
      name: "initialize_godot_project",
      description: "Copy the ProtoShift Godot standard template into workspace/<project-slug>/, replace project naming fields, and rename the root .csproj.",
      inputSchema: {
        type: "object",
        properties: {
          projectSlug: {
            type: "string",
            description: "Target workspace directory name, for example eight-ball-pool.",
          },
          displayName: {
            type: "string",
            description: "Godot display name, for example EightBallPool.",
          },
          assemblyName: {
            type: "string",
            description: "C# assembly name. Defaults to displayName when omitted.",
          },
        },
        required: ["projectSlug", "displayName"],
      },
    },
  ],
}));

server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args = {} } = request.params;

  if (name !== "initialize_godot_project") {
    throw new Error(`Unknown tool: ${name}`);
  }

  const result = await initializeGodotProject(args);
  return {
    content: [
      {
        type: "text",
        text: JSON.stringify(result, null, 2),
      },
    ],
  };
});

const transport = new StdioServerTransport();
await server.connect(transport);