using BepInEx;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Common
{
    class AssetBundleHelper
    {
        public static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var execAssembly = Assembly.GetExecutingAssembly();

            var resourceName = execAssembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(fileName));

            AssetBundle assetBundle;
            using (var stream = execAssembly.GetManifestResourceStream(resourceName))
            {
                assetBundle = AssetBundle.LoadFromStream(stream);
            }

            return assetBundle;
        }
    }
}
