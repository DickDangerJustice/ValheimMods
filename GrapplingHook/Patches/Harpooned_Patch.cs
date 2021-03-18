using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GrapplingHook
{
    [HarmonyPatch(typeof(SE_Harpooned), "UpdateStatusEffect")]
    public static class Harpooned_UpdateStatusEffect_Patch
    {
        public static bool Prefix(SE_Harpooned __instance)
        {
            //Debug.Log(__instance.m_name);
            //if (__instance.m_name == "harpooned")
            //GrapplingHook.TryRegisterPrefabs(__instance);
            return true;
        }
    }
}
