using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Managers;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExampleMob
{
    [BepInPlugin("dickdangerjustice.ExampleMob", "Example Mob", "1.0.0")]
    [BepInDependency(Jotunn.Jotunn.ModGuid)]
    public class Mod : BaseUnityPlugin
    {
        private void Awake()
        {
            PrefabManager.Instance.PrefabRegister += RegisterPrefabs;
        }

        private void RegisterPrefabs(object sender, EventArgs e)
        {
            var boar2Bundle = AssetBundleHelper.GetAssetBundleFromResources("boar2");
            var boar2 = boar2Bundle.LoadAsset<GameObject>("Assets/CustomItems/Boar2/Boar2.prefab");

            PrefabManager.Instance.RegisterPrefab(boar2, "Boar2");
        }
    }
}
