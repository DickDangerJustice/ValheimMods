using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrapplingHook.Patches
{
    class Character_Patch
    {
        [HarmonyPatch(typeof(Character), "GetMass")]
        public static class Character_GetMass_Patch
        {
            public static float Postfix(float __result)
            {
                if (Mod.IsGrappled) return 0;
                return __result;
            }
        }
    }
}
