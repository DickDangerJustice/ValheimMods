using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;

namespace ValheimConfigAndDebugging
{
    [BepInPlugin("dickdangerjustice.TextSizeChangeExample", "Text Size Change Example", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class TextSizeChangeExample : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("dickdangerjustice.TextSizeChangeExample");
        private static ConfigEntry<int> largeFontSize;

        void Awake()
        {
            largeFontSize = Config.Bind<int>("DamageText", "LargeFontSize", 16, "Large Font Size");

            Debug.Log("Initialized text size change example.");
            harmony.PatchAll();
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        // We can modify this value on DamageText.Awake because it won't change. 
        // This means it is set on DamageText instantiation, at game world load.
        [HarmonyPatch(typeof(DamageText), "Awake")]
        static class DamageText_Awake_Patch
        {
            static void Postfix(DamageText __instance, ref int ___m_largeFontSize)
            {
                // Set font size to configured value
                Debug.Log($"Old largeFontSize: {___m_largeFontSize}");
                ___m_largeFontSize = largeFontSize.Value;
                Debug.Log($"New largeFontSize: {___m_largeFontSize}");

                // Get the TextTemplate child, found by using Unity Explorer
                var textTemplate = __instance.transform.Find("TextTemplate");

                // Load the template's transform
                var rectTransform = textTemplate.GetComponent<RectTransform>();

                // Expand the transform out to be able to hold higher values.
                // If you don't do this, the max text size is 35 (you can see by commenting this out if you want)
                // Expand further if you want the text even bigger.
                Debug.Log($"Old sizeDelta: {rectTransform.sizeDelta}");
                rectTransform.sizeDelta = new Vector2(200, 200);
                Debug.Log($"New sizeDelta: {rectTransform.sizeDelta}");
            }
        }
    }
}
