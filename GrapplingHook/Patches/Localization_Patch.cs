using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GrapplingHook
{
    [HarmonyPatch(typeof(Localization), "SetupLanguage")]
    public static class Localization_SetupLanguage_Patch
    {
        public static void Postfix(Localization __instance)
        {
            var addWord = AccessTools.Method(typeof(Localization), "AddWord");
            addWord.Invoke(__instance, new object[] { "item_grappling_hook", "Grappling Hook" });
            addWord.Invoke(__instance, new object[] { "item_grappling_hook_description", "A grappling hook." });
            addWord.Invoke(__instance, new object[] { "se_grappled_name", "Grappled" });
        }
    }
}
