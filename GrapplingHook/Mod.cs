﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using Common;
using HarmonyLib;
using LitJson;
using UnityEngine;

namespace GrapplingHook
{
    [BepInPlugin("dickdangerjustice.GrapplingHook", "Grappling Hook", "1.0.0")]
    public class Mod : BaseUnityPlugin
    {
        private Harmony _harmony;
        public static RecipesConfig Recipes;
        public static readonly Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();
        public static readonly List<StatusEffect> StatusEffects = new List<StatusEffect>();

        // Config
        public static ConfigEntry<string> GrapplingHookHotkey;
        public static ConfigEntry<float> HorizontalPosition;
        public static ConfigEntry<bool> KeepCameraAligned;
        public static ConfigEntry<bool> UseExperimentalFallDamageCancel;

        // Static vars
        public static bool EnableSoftLanding;

        public static KeyCode GrapplingHookKeyCode;
        private void HandleGrapplingHookHotkeyChange(object sender, EventArgs e)
        {
            UpdateGrapplingHookKeyCode();
        }

        private void UpdateGrapplingHookKeyCode()
        {
            if (!Enum.TryParse<KeyCode>(GrapplingHookHotkey.Value, out var grapplingHookKeyCode))
            {
                Debug.Log("Grappling hook hotkey not valid. Value not changed.");
                return;
            }

            GrapplingHookKeyCode = grapplingHookKeyCode;
        }

        private void Awake()
        {
            Debug.Log("GRAPPLING HOOK AWAKE");

            GrapplingHookHotkey = Config.Bind("General", "GrapplingHookHotkey", "G", "Grappling Hook Hotkey");
            UpdateGrapplingHookKeyCode();
            GrapplingHookHotkey.SettingChanged += HandleGrapplingHookHotkeyChange;
            HorizontalPosition = Config.Bind("General", "HorizontalPosition", 1.8f, "Horizontal Position");
            KeepCameraAligned = Config.Bind("General", "KeepCameraAligned", true, "Keep Camera Aligned");
            UseExperimentalFallDamageCancel = Config.Bind("General", "UseExperimentalFallDamageCancel", false, "Use Experimental Fall Damage Cancel");

            Recipes = LoadJsonFile<RecipesConfig>("recipes.json");
            var assetBundle = GetAssetBundleFromResources("grapplinghook");
            var grapplingHook = assetBundle.LoadAsset<GameObject>("assets/customitems/grapplinghook/grapplinghook.prefab");

            var grapplingHookItemDrop = grapplingHook.GetComponent<ItemDrop>();
            var grappled = ScriptableObject.CreateInstance(typeof(SE_Grappled)) as SE_Grappled;
            grappled.name = "Grappled";
            StatusEffects.Add(grappled);
            grapplingHookItemDrop.m_itemData.m_shared.m_attackStatusEffect = grappled;

            Prefabs.Add("GrapplingHook", grapplingHook);

            assetBundle?.Unload(false);

            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        public static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var execAssembly = Assembly.GetExecutingAssembly();

            var resourceName = execAssembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(fileName));

            using var stream = execAssembly.GetManifestResourceStream(resourceName);

            return AssetBundle.LoadFromStream(stream);
        }

        private static T LoadJsonFile<T>(string filename) where T : class
        {
            var jsonFileName = GetAssetPath(filename);
            if (!string.IsNullOrEmpty(jsonFileName))
            {
                var jsonFile = File.ReadAllText(jsonFileName);
                return JsonMapper.ToObject<T>(jsonFile);
            }

            return null;
        }

        private static AssetBundle LoadAssetBundle(string filename)
        {
            var assetBundlePath = GetAssetPath(filename);
            if (!string.IsNullOrEmpty(assetBundlePath))
            {
                return AssetBundle.LoadFromFile(assetBundlePath);
            }

            return null;
        }

        private static string GetAssetPath(string assetName)
        {
            var assetFileName = Path.Combine(Paths.PluginPath, "GrapplingHook", assetName);
            if (!File.Exists(assetFileName))
            {
                Assembly assembly = typeof(Mod).Assembly;
                assetFileName = Path.Combine(Path.GetDirectoryName(assembly.Location), assetName);
                if (!File.Exists(assetFileName))
                {
                    Debug.LogError($"Could not find asset ({assetName})");
                    return null;
                }
            }

            return assetFileName;
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            foreach (var prefab in Prefabs.Values)
            {
                Destroy(prefab);
            }
            Prefabs.Clear();
        }

        public static void TryRegisterPrefabs(ZNetScene zNetScene)
        {
            if (zNetScene == null)
            {
                return;
            }

            Debug.Log("Register prefabs: " + Prefabs.Values.Count);
            foreach (var prefab in Prefabs.Values)
            {
                zNetScene.m_prefabs.Add(prefab);
            }
        }

        internal static void TryRegisterStatusEffects()
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_StatusEffects.Count == 0)
            {
                return;
            }

            foreach (var statusEffect in StatusEffects)
            {
                if (ObjectDB.instance.GetStatusEffect(statusEffect.name) == null)
                {
                    ObjectDB.instance.m_StatusEffects.Add(statusEffect);
                }
            }
        }

        public static void TryRegisterItems()
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 || ObjectDB.instance.GetItemPrefab("Amber") == null)
            {
                return;
            }

            foreach (var prefab in Prefabs.Values)
            {
                var itemDrop = prefab.GetComponent<ItemDrop>();
                if (itemDrop != null)
                {
                    if (ObjectDB.instance.GetItemPrefab(prefab.name.GetStableHashCode()) == null)
                    {
                        ObjectDB.instance.m_items.Add(prefab);
                        Dictionary<int, GameObject> m_itemsByHash = (Dictionary<int, GameObject>)typeof(ObjectDB).GetField("m_itemByHash", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ObjectDB.instance);
                        m_itemsByHash[prefab.name.GetStableHashCode()] = prefab;
                    }
                }
            }
        }

        public static void TryRegisterRecipes()
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0)
            {
                return;
            }

            PrefabCreator.Reset();
            foreach (var recipe in Recipes.recipes)
            {
                PrefabCreator.AddNewRecipe(recipe.name, recipe.item, recipe);
            }
        }
    }
}
