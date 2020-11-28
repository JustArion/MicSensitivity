using MelonLoader;
[assembly: MelonInfo(typeof(Dawn.Mic.MicSensitivity), Dawn.Update.AssemblyInfo.Name, Dawn.Update.AssemblyInfo.Version + Dawn.Update.AssemblyInfo.Version, Dawn.Update.AssemblyInfo.Authors)]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonOptionalDependencies("UIExpansionKit")]
namespace Dawn.Update
{
    internal static class AssemblyInfo
    {
        internal const string Name = "MicSensitivity";

        internal const string Version = "1.2";

        internal const string Description = "";

        internal const string Authors = "@arion#1223";
    }
}
