using System;
using Dawn.Update;
using MelonLoader;
using static Dawn.Update.AssemblyInfo;
using ModType = Dawn.Mic.MicSensitivity;

[assembly: MelonInfo(typeof(ModType), Name, AssemblyInfo.Version, Authors)]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.DarkCyan)]
[assembly: MelonOptionalDependencies("UIExpansionKit")]

namespace Dawn.Update
{
    internal static class AssemblyInfo
    {
        internal const string Name = "MicSensitivity";

        internal const string Version = "1.4.4";

        internal const string Description = "";

        internal const string Authors = "arion#1223";
    }
}