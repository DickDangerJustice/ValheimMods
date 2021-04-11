using BepInEx;
using HarmonyLib;

namespace ExampleShowTamed
{
    [BepInPlugin("dickdangerjustice.ExampleShowTamed", "Example Show Tamed", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class Mod : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("dickdangerjustice.ExampleShowTamed");

        void Awake()
        {
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Tameable), nameof(Tameable.GetHoverText))]
        class Tameable_Patch
        {
            // Display when a tamed animal is following you by hovering the mouse over the animal.
            // Private methods are accessible because the assemblies have been run through a publicizer (https://github.com/CabbageCrow/AssemblyPublicizer)
            static bool Prefix(Tameable __instance, ref string __result, Character ___m_character, MonsterAI ___m_monsterAI, ZNetView ___m_nview)
            {
                // Construct the logic that fits your case
                if (___m_nview.IsValid() && ___m_character.IsTamed() && (bool)___m_monsterAI.GetFollowTarget())
                {
                    // Some code duplication here. Could use transpiler patch to avoid, but they're complicated.
                    // Probably not a big deal.
                    var str = Localization.instance.Localize(___m_character.m_name);
                    str += Localization.instance.Localize(" ( $hud_tame, " + __instance.GetStatusString() + ", Following )");

                    // Set result and skip original function execution using harmony concept (https://harmony.pardeike.net/articles/patching-prefix.html)
                    __result = str + Localization.instance.Localize("\n[<color=yellow><b>$KEY_Use</b></color>] $hud_pet");
                    return false;
                }

                // Continue with original method if our case is not met
                return true;
            }
        }
    }
}