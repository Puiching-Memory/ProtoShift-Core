import { copyFile, mkdir, readdir, readFile, rename, stat, writeFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { CallToolRequestSchema, ListToolsRequestSchema } from "@modelcontextprotocol/sdk/types.js";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const repoRoot = path.resolve(__dirname, "..", "..");
const templateRoot = path.join(repoRoot, "standard-template");
const workspaceRoot = path.join(repoRoot, "workspace");
const templateProjectName = "ProtoShiftGame";

const ignoredTemplateEntries = new Set([
  ".godot",
  ".vs",
  "Binaries",
  "bin",
  "obj",
  "DerivedDataCache",
  "Intermediate",
  "Saved",
]);

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
    if (ignoredTemplateEntries.has(entry.name)) {
      continue;
    }

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

const textExtensions = new Set([
  ".md",
  ".godot",
  ".cs",
  ".csproj",
  ".cpp",
  ".h",
  ".json",
  ".sln",
  ".target",
  ".tscn",
  ".props",
  ".uproject",
]);

function isValidProjectSlug(projectSlug) {
  return /^[A-Za-z0-9][A-Za-z0-9_-]*$/.test(projectSlug);
}

function isValidAssemblyName(assemblyName) {
  return /^[A-Za-z_][A-Za-z0-9_]*$/.test(assemblyName);
}

function escapeGodotString(value) {
  return value.replaceAll("\\", "\\\\").replaceAll('"', '\\"');
}

async function replaceInDirectory(targetDir, replacements) {
  const entries = await readdir(targetDir, { withFileTypes: true });

  for (const entry of entries) {
    const entryPath = path.join(targetDir, entry.name);

    if (entry.isDirectory()) {
      await replaceInDirectory(entryPath, replacements);
      continue;
    }

    if (!textExtensions.has(path.extname(entry.name))) {
      continue;
    }

    await replaceInFile(entryPath, replacements);
  }
}

async function renameEntriesContaining(targetDir, fromText, toText) {
  const entries = await readdir(targetDir, { withFileTypes: true });

  for (const entry of entries) {
    const entryPath = path.join(targetDir, entry.name);

    if (entry.isDirectory() && !ignoredTemplateEntries.has(entry.name)) {
      await renameEntriesContaining(entryPath, fromText, toText);
    }

    if (!entry.name.includes(fromText)) {
      continue;
    }

    const renamedPath = path.join(targetDir, entry.name.split(fromText).join(toText));
    await rename(entryPath, renamedPath);
  }
}

async function initializeGodotProject(args) {
  const projectSlug = String(args.projectSlug ?? "").trim();
  const displayName = String(args.displayName ?? "").trim();
  const assemblyName = String(args.assemblyName ?? displayName).trim();

  if (!projectSlug) {
    throw new Error("projectSlug is required");
  }

  if (!isValidProjectSlug(projectSlug)) {
    throw new Error("projectSlug must contain only letters, numbers, dashes, or underscores");
  }

  if (!displayName) {
    throw new Error("displayName is required");
  }

  if (!assemblyName) {
    throw new Error("assemblyName is required");
  }

  if (!isValidAssemblyName(assemblyName)) {
    throw new Error("assemblyName must be a valid code identifier using only letters, numbers, and underscores");
  }

  if (!(await exists(templateRoot))) {
    throw new Error(`ProtoShift standard template not found: ${templateRoot}`);
  }

  const targetDir = path.join(workspaceRoot, projectSlug);
  if (await exists(targetDir)) {
    throw new Error(`Target project already exists: ${targetDir}`);
  }

  await copyDirectory(templateRoot, targetDir);

  const replacements = [[templateProjectName, assemblyName]];
  await replaceInDirectory(targetDir, replacements);
  await renameEntriesContaining(targetDir, templateProjectName, assemblyName);

  const projectFilePath = path.join(targetDir, "project.godot");
  await replaceInFile(projectFilePath, [[`config/name="${templateProjectName}"`, `config/name="${escapeGodotString(displayName)}"`]]);

  const targetCsproj = path.join(targetDir, `${assemblyName}.csproj`);
  const unrealProjectPath = path.join(targetDir, "UnrealProject", `${assemblyName}.uproject`);
  const unrealHostCsprojPath = path.join(targetDir, "UnrealProject", "Managed", `${assemblyName}.UnrealHost.csproj`);

  for (const expectedPath of [targetCsproj, unrealProjectPath, unrealHostCsprojPath]) {
    if (!(await exists(expectedPath))) {
      throw new Error(`Initialized project is missing expected file: ${expectedPath}`);
    }
  }

  return {
    projectPath: targetDir,
    csprojPath: targetCsproj,
    projectFilePath,
    unrealProjectPath,
    unrealHostCsprojPath,
    requiredLayout: [
      `workspace/${projectSlug}/GamePackage/`,
      `workspace/${projectSlug}/GamePackage.Tests/`,
      `workspace/${projectSlug}/GodotHost/`,
      `workspace/${projectSlug}/Migration/`,
      `workspace/${projectSlug}/UnrealProject/`,
    ],
    nextSteps: [
      `dotnet build workspace/${projectSlug}/${assemblyName}.csproj`,
      `dotnet test workspace/${projectSlug}/GamePackage.Tests/GamePackage.Tests.csproj`,
      `Use Godot MCP to open workspace/${projectSlug}/project.godot`,
      `Create or replace GamePackage.Tests so they validate the new game's runtime instead of template sample behavior`,
      `Write project rules into workspace/${projectSlug}/GamePackage/ before editing GodotHost`,
      `Keep workspace/${projectSlug}/Migration/ artifacts, README, and Unreal handoff docs updated as gameplay evolves`,
      `Design the project so the same GamePackage is ready for both Godot and Unreal hosts from day one`,
      `Run dotnet build again before launching the project`,
    ],
  };
}

const server = new Server(
  {
    name: "protoshift-mcp",
    version: "0.2.0",
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
      description: "Copy the ProtoShift unified Godot template into workspace/<project-slug>/, preserving the standard GamePackage/GodotHost/Migration/UnrealProject layout, replacing project naming fields, and renaming the root .csproj.",
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