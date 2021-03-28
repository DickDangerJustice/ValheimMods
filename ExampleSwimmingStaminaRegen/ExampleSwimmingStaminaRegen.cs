using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;

namespace ExampleSwimmingStaminaRegen
{
    [BepInPlugin("dickdangerjustice.ExampleSwimmingStaminaRegen", "Example Swimming Stamina Regen", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class ExampleSwimmingStaminaRegen : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("dickdangerjustice.ExampleSwimmingStaminaRegen");
        private static ConfigEntry<float> treadingStaminaRegen;

        void Awake()
        {
            treadingStaminaRegen = Config.Bind("General", "TreadingStaminaRegen", 1f, "Treading Stamina Regen");

            harmony.PatchAll();
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(Player), "UpdateStats")]
        static class Player_UpdateStats_Patch
        {
            static void Postfix(float dt, Character __instance, ref float ___m_stamina)
            {
                if (__instance.IsSwiming() && !__instance.IsOnGround())
                {
                    ___m_stamina = Mathf.Min(__instance.GetMaxStamina(), ___m_stamina + treadingStaminaRegen.Value * dt);
                }
            }
        }
    }
}
