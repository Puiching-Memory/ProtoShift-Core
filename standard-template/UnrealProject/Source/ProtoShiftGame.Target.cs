using UnrealBuildTool;
using System.Collections.Generic;

public class ProtoShiftGameTarget : TargetRules
{
	public ProtoShiftGameTarget(TargetInfo Target) : base(Target)
	{
		Type = TargetType.Game;
		DefaultBuildSettings = BuildSettingsVersion.V6;
		IncludeOrderVersion = EngineIncludeOrderVersion.Latest;
		ExtraModuleNames.Add("ProtoShiftGame");
	}
}