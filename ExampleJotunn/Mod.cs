using BepInEx;
using ExampleJotunn.Prefabs;
using HarmonyLib;
using JotunnLib.Entities;
using JotunnLib.Managers;
using Shared;
using System;
using UnityEngine;

namespace ExampleJotunn
{
    [BepInPlugin("dickdangerjustice.ExampleJotunn", "Example Jotunn", "1.0.0")]
    public class Mod : BaseUnityPlugin
    {
        private void Awake()
        {
            PrefabManager.Instance.PrefabRegister += RegisterPrefabs;
            ObjectManager.Instance.ObjectRegister += InitObjects;
        }

        private void RegisterPrefabs(object sender, EventArgs e)
        {
            var swordBlockBundle = AssetBundleHelper.GetAssetBundleFromResources("swordblock");
            var swordBlock = swordBlockBundle.LoadAsset<GameObject>("Assets/CustomItems/SwordBlock/SwordBlock.prefab");

            AccessTools.Method(typeof(PrefabManager), "RegisterPrefab", new Type[] { typeof(GameObject), typeof(string) }).Invoke(PrefabManager.Instance, new object[] { swordBlock, "SwordBlock" });

            PrefabManager.Instance.RegisterPrefab(new ExampleWood());
        }

        private void InitObjects(object sender, EventArgs e)
        {
            // Add block sword as an item
            ObjectManager.Instance.RegisterItem("SwordBlock");

            // Add example wood as an item
            ObjectManager.Instance.RegisterItem("ExampleWood");

            // Add a sample recipe for the example sword
            ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_SwordBlock",

                // Name of the prefab for the crafted item
                Item = "SwordBlock",

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
                        Item = "DeerHide",

                        // Amount required
                        Amount = 1
                    }
                }
            });
        }
    }
}
