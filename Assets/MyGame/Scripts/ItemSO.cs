using UnityEngine;
using System.Collections.Generic;

namespace BOH
{
    [CreateAssetMenu(fileName = "ItemSpecial", menuName = "BOH/Data/ItemSpecial")]
    public class ItemSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemId;
        public string displayName;
        [TextArea(2, 3)]
        public string description;
        
        [Header("Category")]
        public ItemCategory category = ItemCategory.Consumable;
        
        [Header("Properties")]
        public bool isPersistent = false;
        public bool isStackable = true;
        public bool isEquippable = false;
        
        [Header("Special Item")]
        public string equipTag; // e.g., "FOUND_NECKLACE"
        public int buyPrice = 0;
        public int sellPrice = 0;
        public int energyValue = 0;
        
        [Header("Visual")]
        public Sprite icon;
        
        public enum ItemCategory
        {
            Consumable,
            Material,
            Token,
            Document,
            Special
        }
    }

    // ScriptRole: Extended item definition supporting special items
    // RelatedScripts: InventorySystem, GiftingRulesSO
}