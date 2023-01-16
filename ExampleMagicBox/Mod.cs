using BepInEx;
using ExampleMagicBox.Prefabs;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using Shared;
using System;
using UnityEngine;

namespace ExampleMagicBox
{
    [BepInPlugin("dickdangerjustice.ExampleMagicBox", "Example Magic Box", "1.0.0")]
    //[BepInDependency(Jotunn.Jotunn.ModGuid)]
    public class Mod : BaseUnityPlugin
    {
        private void Awake()
        {
            PrefabManager.Instance.PrefabManager += RegisterPrefabs;
            PieceManager.Instance.PieceRegister += RegisterPieces;
            ObjectManager.Instance.ObjectRegister += RegisterObjects;
        }

        private void RegisterPrefabs(object sender, EventArgs e)
        {
            var magicBoxBundle = AssetBundleHelper.GetAssetBundleFromResources("magicbox");
            var magicBox = magicBoxBundle.LoadAsset<GameObject>("Assets/CustomItems/MagicBox/piece_magic_box.prefab");
            // edit additional properties here

            PrefabManager.Instance.RegisterPrefab(magicBox, "MagicBox");

            PrefabManager.Instance.RegisterPrefab(new MagicArmor());
        }

        private void RegisterPieces(object sender, EventArgs e)
        {
            // Add magic box to the hammer piece table
            PieceManager.Instance.RegisterPiece("Hammer", "piece_magic_box");
        }

        private void RegisterObjects(object sender, EventArgs e)
        {
            // Add magic armor as an item
            ObjectManager.Instance.RegisterItem("MagicArmor");

            // Add a sample recipe for the magic armor to the magic box
            ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_MagicArmor",

                // Name of the prefab for the crafted item
                Item = "MagicArmor",

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "piece_magic_box",

                RepairStation = "piece_magic_box",

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
