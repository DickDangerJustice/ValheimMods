using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace GrapplingHook.Patches
{
    class Player_Patch
    {
        [HarmonyPatch(typeof(Player), "Update")]
        static class Player_Update_Patch
        {
            static void Postfix(Player __instance, ref Attack ___m_currentAttack, ref float ___m_lastCombatTimer, Rigidbody ___m_body, ZSyncAnimation ___m_zanim, CharacterAnimEvent ___m_animEvent, VisEquipment ___m_visEquipment, Attack ___m_previousAttack, float ___m_timeSinceLastAttack, Inventory ___m_inventory)
            {
                // set configuration variable to allow different key options
                if (!Enum.TryParse<KeyCode>(Mod.GrapplingHookHotkey.Value, out var grapplingHookHotkey))
                {
                    Debug.Log("Grappling hook hotkey not set.");
                    return;
                }
                
                if (Input.GetKeyDown(grapplingHookHotkey))
                {
                    var grapplingHook = ___m_inventory.GetAllItems().First(v => v.m_shared.m_name == "$item_grappling_hook");
                    
                    if (grapplingHook == null) return;

                    var attack = grapplingHook.m_shared.m_attack.Clone();
                    attack.Start(__instance, ___m_body, ___m_zanim, ___m_animEvent, ___m_visEquipment, grapplingHook, ___m_previousAttack, ___m_timeSinceLastAttack, __instance.GetAttackDrawPercentage());
                    ___m_currentAttack = attack;
                    ___m_lastCombatTimer = 0f;
                }
            }
        }
    }
}
