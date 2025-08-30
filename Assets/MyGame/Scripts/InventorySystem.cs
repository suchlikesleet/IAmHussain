using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Obvious.Soap;

namespace BOH
{
    public class InventorySystem : MonoBehaviour
    {
        [System.Serializable]
        public class InventoryItem
        {
            public ItemSO itemData;
            public int count;
            public bool isEquipped;
        }

        [Header("Inventory")]
        [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();
        [SerializeField] private InventoryItem equippedSpecialItem;
        
        [Header("Events")]
        [SerializeField] private ScriptableEventNoParam onInventoryChanged;
        [SerializeField] private ScriptableEventNoParam onItemEquipped;
        [SerializeField] private ScriptableEventNoParam onDayEnd;
        
        private void OnEnable()
        {
            if (onDayEnd != null) onDayEnd.OnRaised += ClearDayScopedItems;
            //onDayEnd.OnRaised += ClearDayScopedItems;
        }

        private void OnDisable()
        {
            if (onDayEnd != null) onDayEnd.OnRaised -= ClearDayScopedItems;
            //onDayEnd.OnRaised -= ClearDayScopedItems;
        }

        public void AddItem(ItemSO item, int count = 1)
        {
            if (item == null) return;
            
            var existing = inventory.FirstOrDefault(i => i.itemData.itemId == item.itemId);
            
            if (existing != null && item.isStackable)
            {
                existing.count += count;
            }
            else
            {
                inventory.Add(new InventoryItem
                {
                    itemData = item,
                    count = count,
                    isEquipped = false
                });
            }
            
            Debug.Log($"Added {count}x {item.displayName}");
            onInventoryChanged?.Raise();
        }

        public bool HasItem(string itemId, int count = 1)
        {
            var item = inventory.FirstOrDefault(i => i.itemData.itemId == itemId);
            return item != null && item.count >= count;
        }

        public bool ConsumeItem(string itemId, int count = 1)
        {
            var item = inventory.FirstOrDefault(i => i.itemData.itemId == itemId);
            if (item == null || item.count < count) return false;
            
            // Unequip if consuming equipped item
            if (item.isEquipped && item.count <= count)
            {
                UnequipItem(itemId);
            }
            
            item.count -= count;
            if (item.count <= 0)
                inventory.Remove(item);
            
            Debug.Log($"Consumed {count}x {itemId}");
            onInventoryChanged?.Raise();
            return true;
        }

        public bool EquipItem(string itemId)
        {
            var item = inventory.FirstOrDefault(i => 
                i.itemData.itemId == itemId && 
                i.itemData.isEquippable);
            
            if (item == null) return false;
            
            // Unequip previous item
            if (equippedSpecialItem != null)
            {
                equippedSpecialItem.isEquipped = false;
            }
            
            // Equip new item
            item.isEquipped = true;
            equippedSpecialItem = item;
            
            Debug.Log($"Equipped: {item.itemData.displayName}");
            onItemEquipped?.Raise();
            onInventoryChanged?.Raise();
            return true;
        }

        public void UnequipItem(string itemId = null)
        {
            if (equippedSpecialItem != null)
            {
                if (itemId == null || equippedSpecialItem.itemData.itemId == itemId)
                {
                    equippedSpecialItem.isEquipped = false;
                    Debug.Log($"Unequipped: {equippedSpecialItem.itemData.displayName}");
                    equippedSpecialItem = null;
                    onItemEquipped?.Raise();
                    onInventoryChanged?.Raise();
                }
            }
        }

        public InventoryItem GetEquippedItem()
        {
            return equippedSpecialItem;
        }

        public string GetEquippedTag()
        {
            return equippedSpecialItem?.itemData.equipTag ?? "";
        }

        public bool GiftItem(string itemId, string recipientId)
        {
            if (!HasItem(itemId, 1)) return false;
            
            // Process through GiftingSystem
            var giftingSystem = FindFirstObjectByType<GiftingSystem>();
            if (giftingSystem != null && giftingSystem.ProcessGift(itemId, recipientId))
            {
                ConsumeItem(itemId, 1);
                return true;
            }
            
            return false;
        }

        private void ClearDayScopedItems()
        {
            // Unequip if day-scoped
            if (equippedSpecialItem != null && !equippedSpecialItem.itemData.isPersistent)
            {
                UnequipItem();
            }
            
            int removedCount = inventory.RemoveAll(i => !i.itemData.isPersistent);
            if (removedCount > 0)
            {
                Debug.Log($"Cleared {removedCount} day-scoped items");
                onInventoryChanged?.Raise();
            }
        }

        public List<InventoryItem> GetAllItems()
        {
            return new List<InventoryItem>(inventory);
        }

        public int GetItemCount(string itemId)
        {
            var item = inventory.FirstOrDefault(i => i.itemData.itemId == itemId);
            return item?.count ?? 0;
        }
    }

    // ScriptRole: Inventory with special item equipping support
    // RelatedScripts: GiftingSystem, DialogueSystem
    // UsesSO: ItemSO, GameEventSO
    // SendsTo: UI, DialogueSystem via events
}