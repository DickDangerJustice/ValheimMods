using HarmonyLib;
using JotunnLib.Entities;
using UnityEngine;

namespace ExampleJotunn.Prefabs
{
    public class MagicArmor : PrefabConfig
    {
        public MagicArmor() : base("MagicArmor", "ArmorIronChest")
        {
            // Nothing to do here
            // "Prefab" wil be set for us automatically after this is called
        }

        public override void Register()
        {
            // Configure item drop
            // ItemDrop is a component on GameObjects which determines info about the item when it's picked up in the inventory
            ItemDrop item = Prefab.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = "Magic Armor";
            item.m_itemData.m_shared.m_description = "Godlike armor with a twist";
            item.m_itemData.m_shared.m_armor = 999;

            // create status effect
            var burning = ScriptableObject.CreateInstance<SE_Burning>();

            // load damages field
            var burnDamageField = AccessTools.Field(typeof(SE_Burning), "m_damage");
            var burnDamage = (HitData.DamageTypes) burnDamageField.GetValue(burning);

            // edit fire damage
            burnDamage.m_fire = 1;

            // save damages field
            burnDamageField.SetValue(burning, burnDamage);

            // set item status effect
            item.m_itemData.m_shared.m_equipStatusEffect = burning;
        }
    }
}
