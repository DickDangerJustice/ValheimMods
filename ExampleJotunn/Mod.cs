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
    [BepInPlugin("dickdangerjustice.ExampleJotunn", "Example Jotunn", "1.0.0")]
    [BepInDependency(Jotunn.Jotunn.ModGuid)]
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
            var swordBlockItemDrop = swordBlock.GetComponent<ItemDrop>();
            swordBlockItemDrop.m_itemData.m_shared.m_damages.Add(new HitData.DamageTypes
            {
                m_fire = 20
            });

            PrefabManager.Instance.RegisterPrefab(swordBlock, "SwordBlock");

            PrefabManager.Instance.RegisterPrefab(new MagicArmor());
        }

        private void InitObjects(object sender, EventArgs e)
        {
            // Add block sword as an item
            ObjectManager.Instance.RegisterItem("SwordBlock");

            // Add magic armor as an item
            ObjectManager.Instance.RegisterItem("MagicArmor");

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

            // Add a sample recipe for the magic armor
            ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_MagicArmor",

                // Name of the prefab for the crafted item
                Item = "MagicArmor",

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
