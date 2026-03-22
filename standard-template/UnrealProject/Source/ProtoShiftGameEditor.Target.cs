using UnrealBuildTool;
using System.Collections.Generic;

public class ProtoShiftGameEditorTarget : TargetRules
{
	public ProtoShiftGameEditorTarget(TargetInfo Target) : base(Target)
	{
		Type = TargetType.Editor;
		DefaultBuildSettings = BuildSettingsVersion.V6;
		IncludeOrderVersion = EngineIncludeOrderVersion.Latest;
		ExtraModuleNames.Add("ProtoShiftGame");
	}
}