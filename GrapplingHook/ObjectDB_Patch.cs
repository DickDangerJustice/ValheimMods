using HarmonyLib;
using UnityEngine;

namespace GrapplingHook
{
    [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
    public static class ObjectDB_CopyOtherDB_Patch
    {
        public static void Postfix()
        {
            GrapplingHook.TryRegisterStatusEffects();
            GrapplingHook.TryRegisterItems();
            GrapplingHook.TryRegisterRecipes();
        }
    }

    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    public static class ObjectDB_Awake_Patch
    {
        public static void Postfix()
        {
            GrapplingHook.TryRegisterStatusEffects();
            GrapplingHook.TryRegisterItems();
            GrapplingHook.TryRegisterRecipes();
        }
    }
}
