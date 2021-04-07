using BepInEx;
using HarmonyLib;
using JotunnLib.Entities;
using JotunnLib.Managers;
using Shared;
using System;
using UnityEngine;

namespace ExampleTwo
{
    [BepInPlugin("dickdangerjustice.ExampleTwo", "Example Two", "1.0.0")]
    [BepInDependency(JotunnLib.JotunnLib.ModGuid)]
    public class Mod : BaseUnityPlugin
    {
        private void Awake()
        {
            PrefabManager.Instance.PrefabRegister += RegisterPrefabs;
            ObjectManager.Instance.ObjectRegister += InitObjects;
        }

        private void RegisterPrefabs(object sender, EventArgs e)
        {
            var swordBlockBundle = AssetBundleHelper.GetAssetBundleFromResources("two");
            var cape1 = swordBlockBundle.LoadAsset<GameObject>("Assets/CustomItems/Two/Cape1.prefab");
            var cape2 = swordBlockBundle.LoadAsset<GameObject>("Assets/CustomItems/Two/Cape2.prefab");

            // when this is fixed, the call should be:
            // PrefabManager.Instance.RegisterPrefab(cape1, "Cape1");
            AccessTools.Method(typeof(PrefabManager), "RegisterPrefab", new Type[] { typeof(GameObject), typeof(string) }).Invoke(PrefabManager.Instance, new object[] { cape1, "Cape1" });
            AccessTools.Method(typeof(PrefabManager), "RegisterPrefab", new Type[] { typeof(GameObject), typeof(string) }).Invoke(PrefabManager.Instance, new object[] { cape2, "Cape2" });
        }

        private void InitObjects(object sender, EventArgs e)
        {
            // Add block sword as an item
            ObjectManager.Instance.RegisterItem("Cape1");
            ObjectManager.Instance.RegisterItem("Cape2");

            // Add a sample recipe for cape 1
            ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_Cape1",

                // Name of the prefab for the crafted item
                Item = "Cape1",

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "piece_workbench",

                RepairStation = "piece_workbench",

                // List of requirements to craft your item
                Requirements = new PieceRequirementConfig[]
                {
                    new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "Wood",

                        // Amount required
                        Amount = 1
                    }
                }
            });

            // Add a sample recipe for cape 2
            ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_Cape2",

                // Name of the prefab for the crafted item
                Item = "Cape2",

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "piece_workbench",

                RepairStation = "piece_workbench",

                // List of requirements to craft your item
                Requirements = new PieceRequirementConfig[]
                {
                    new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "Wood",

                        // Amount required
                        Amount = 1
                    }
                }
            });
        }
    }
}
