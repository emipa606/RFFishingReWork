using System.Linq;
using Verse;

namespace RBB_Code;

[StaticConstructorOnStartup]
internal static class RBB_Initializer
{
    public static bool iceFishing;

    static RBB_Initializer()
    {
        if (ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name.Contains("Permafrost")) ||
            ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name.Contains("Dynamic Terrain")) ||
            ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name == "Ice") ||
            ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name == "Nature's Pretty Sweet"))
        {
            iceFishing = true;
        }
    }
}