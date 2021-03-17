using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;
using System.Collections;

namespace ExamplePingShips
{
    [BepInPlugin("dickdangerjustice.ExamplePingShips", "Example Ping Ships", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class ExamplePingShips : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("dickdangerjustice.ExamplePingShips");

        void Awake() 
        { 
            harmony.PatchAll();
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(Player), "Update")]
        static class Player_Update_Patch
        {
            static void Postfix()
            {
                // set configuration variable to allow different key options
                if (Input.GetKeyDown(KeyCode.O))
                {
                    Debug.Log($"Pinging all ships.");
                    var ships = FindObjectsOfType<Ship>();
                    Debug.Log($"There are {ships.Length} ships.");

                    // using chat instance as a dummy object to run coroutine
                    // probably a better way
                    Chat.instance.StartCoroutine(PingShipsCoroutine(ships));
                }
            }

            static IEnumerator PingShipsCoroutine(Ship[] ships)
            {
                Debug.Log("Starting coroutine.");
                foreach (var ship in ships)
                {
                    Debug.Log("Pinging ship.");
                    // only one ping can be active at a time
                    // wait 2 seconds between pings
                    // could add more information here by sending a shout with boat metadata (type?)
                    Chat.instance.SendPing(ship.transform.position);
                    yield return new WaitForSeconds(2);
                }
            }
        }

        [HarmonyPatch(typeof(Player), "StopShipControl")]
        static class Player_StopShipControl_Patch
        {
            static void Postfix(Player __instance)
            {
                Minimap.instance.AddPin(__instance.transform.position, Minimap.PinType.Icon3, "Ship", true, false);
            }
        }
    }
}
