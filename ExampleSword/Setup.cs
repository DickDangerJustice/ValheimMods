using BepInEx;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace ExampleSword
{
    [BepInPlugin("dickdangerjustice.ExampleSword", "Example Sword", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class Setup : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("dickdangerjustice.ExampleSword");
        private static GameObject ExampleSword;

        void Awake()
        {
            var assetBundle = GetAssetBundleFromResources("examplesword");
            ExampleSword = assetBundle.LoadAsset<GameObject>("Assets/CustomItems/ExampleSword.prefab");
            harmony.PatchAll();
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        public static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var execAssembly = Assembly.GetExecutingAssembly();

            var resourceName = execAssembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(fileName));

            using (var stream = execAssembly.GetManifestResourceStream(resourceName))
            {
                return AssetBundle.LoadFromStream(stream);
            }
        }

        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        public static class ZNetScene_Awake_Patch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if (__instance == null)
                {
                    return;
                }

                __instance.m_prefabs.Add(ExampleSword);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        public static class ObjectDB_CopyOtherDB_Patch
        {
            public static void Postfix()
            {
                AddExampleSword();
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "Awake")]
        public static class ObjectDB_Awake_Patch
        {
            public static void Postfix()
            {
                AddExampleSword();
            }
        }

        private static void AddExampleSword()
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0)
            {
                return;
            }

            var itemDrop = ExampleSword.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                if (ObjectDB.instance.GetItemPrefab(ExampleSword.name.GetStableHashCode()) == null)
                {
                    ObjectDB.instance.m_items.Add(ExampleSword);
                    Dictionary<int, GameObject> m_itemsByHash = (Dictionary<int, GameObject>) typeof(ObjectDB).GetField("m_itemByHash", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ObjectDB.instance);
                    m_itemsByHash[ExampleSword.name.GetStableHashCode()] = ExampleSword;
                }
            }
        }
    }
}
