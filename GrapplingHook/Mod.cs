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
        public static ConfigEntry<string> GrapplingHookHotkey;

        private void Awake()
        {
            Debug.Log("GRAPPLING HOOK AWAKE");

            GrapplingHookHotkey = Config.Bind<string>("General", "GrapplingHookHotkey", "Z", "Grappling Hook Hotkey");

            Recipes = LoadJsonFile<RecipesConfig>("recipes.json");
            //var assetBundle = LoadAssetBundle("custom_item_grappling_hook");
            var assetBundle = GetAssetBundleFromResources("grapplinghook");
            var grapplingHook = assetBundle.LoadAsset<GameObject>("assets/customitems/grapplinghook/grapplinghook.prefab");

            var grapplingHookItemDrop = grapplingHook.GetComponent<ItemDrop>();
            var grappled = ScriptableObject.CreateInstance(typeof(SE_Grappled)) as SE_Grappled;
            grappled.name = "Grappled";
            StatusEffects.Add(grappled);
            grapplingHookItemDrop.m_itemData.m_shared.m_attackStatusEffect = grappled;
            //var grappled = assetBundle.LoadAsset<StatusEffect>("assets/customitems/grapplinghook/grappled.asset");

            //grapplingHookItemDrop.m_itemData.m_shared.m_attackStatusEffect = grappled;
            Prefabs.Add("GrapplingHook", grapplingHook);
            //StatusEffects.Add(grappled);

            //var assetBundle = GetAssetBundleFromResources("custom_item_grappling_hook");
            //var prefab = assetBundle.LoadAsset<GameObject>("assets/prefabinstance/queensjam.prefab");
            //Debug.Log("Fixed Item Drop?: " + prefab.GetComponent<ItemDrop>());
            //Prefabs.Add("GrapplingHook", prefab);
            //assetBundle.LoadAllAssets();
            //Debug.Log("Hook asset bundle: " + assetBundle);
            //if (Recipes != null && assetBundle != null)
            //{
            //    foreach (var recipe in Recipes.recipes)
            //    {
            //        if (assetBundle.Contains(recipe.item))
            //        {
            //            //var prefab = assetBundle.LoadAsset<GameObject>(recipe.item);
            //            //Debug.Log("Prefab: " + prefab);
            //            var prefab = assetBundle.LoadAsset<GameObject>($"assets/customitems/grapplinghook/{recipe.item}.prefab");
            //            Prefabs.Add(recipe.item, prefab);
            //        }
            //    }
            //}

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
            _harmony?.UnpatchAll();
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
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0)
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
