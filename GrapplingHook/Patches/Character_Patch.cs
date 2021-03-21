using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GrapplingHook
{
    [HarmonyPatch(typeof(Character), "UpdateGroundContact")]
    public static class Character_UpdateGroundContact_Patch
    {
        public static void Prefix(Character __instance, bool ___m_groundContact, ref float ___m_maxAirAltitude)
        {
            if (__instance.IsPlayer() && ___m_groundContact && Mod.EnableSoftLanding)
            {
                Debug.Log("Grounded");
                ___m_maxAirAltitude = __instance.transform.position.y;
                Mod.EnableSoftLanding = false;
            }
        }
    }
}
