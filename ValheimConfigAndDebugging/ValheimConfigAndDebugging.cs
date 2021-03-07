using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;

namespace ValheimConfigAndDebugging
{
    [BepInPlugin("dickdangerjustice.ValheimConfigAndDebugging", "Valheim Config and Debugging", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class ValheimConfigAndDebugging : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("dickdangerjustice.ValheimConfigAndDebugging");
        private static ConfigEntry<float> sailForceFactorMultiplier;

        void Awake()
        {
            sailForceFactorMultiplier = Config.Bind<float>("General", "SailForceFactorMultiplier", 1f, "Sail Force Factor Multiplier");

            Debug.Log("Initialized config and debugging");
            harmony.PatchAll();
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(Ship), "Awake")]
        static class Ship_Awake_Patch
        {
            static void Postfix(ref float ___m_sailForceFactor)
            {
                Debug.Log($"Old sailForceFactor: {___m_sailForceFactor}");
                ___m_sailForceFactor *= sailForceFactorMultiplier.Value;
                Debug.Log($"New sailForceFactor: {___m_sailForceFactor}");
            }
        }
    }
}
