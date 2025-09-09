using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BOH
{
    public class InventoryUIExtended : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Transform itemListContainer;
        [SerializeField] private GameObject itemSlotPrefab;
        [SerializeField] private TextMeshProUGUI equippedItemText;
        
        [Header("Events")]
        [SerializeField] private ScriptableEventNoParam onInventoryChanged;
        [SerializeField] private ScriptableEventNoParam onItemEquipped;
        
        private InventorySystem inventorySystem;

        private void OnEnable()
        {
            if (onInventoryChanged != null) 
                onInventoryChanged.OnRaised += RefreshInventoryDisplay;
            if (onItemEquipped != null)
                onItemEquipped.OnRaised += RefreshEquippedDisplay;
        }

        private void OnDisable()
        {
            if (onInventoryChanged != null) 
                onInventoryChanged.OnRaised -= RefreshInventoryDisplay;
            if (onItemEquipped != null)
                onItemEquipped.OnRaised -= RefreshEquippedDisplay;;
        }

        private void Start()
        {
            inventorySystem = GameServices.Inventory ?? FindFirstObjectByType<InventorySystem>();
            RefreshInventoryDisplay();
            RefreshEquippedDisplay();
        }

        private void RefreshInventoryDisplay()
        {
            // Clear existing slots
            foreach (Transform child in itemListContainer)
            {
                Destroy(child.gameObject);
            }

            // Create slots for each item
            var items = inventorySystem?.GetAllItems() ?? new List<InventorySystem.InventoryItem>();
            
            foreach (var item in items)
            {
                CreateItemSlot(item);
            }
        }

        private void CreateItemSlot(InventorySystem.InventoryItem item)
        {
            if (itemSlotPrefab == null || itemListContainer == null) return;
            if (item == null || item.itemData == null) return;
            
            GameObject slot = Instantiate(itemSlotPrefab, itemListContainer);
            
            // Setup visuals
            var icon = slot.transform.Find("Icon")?.GetComponent<Image>();
            var nameText = slot.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            var countText = slot.transform.Find("Count")?.GetComponent<TextMeshProUGUI>();
            var equipButton = slot.transform.Find("EquipButton")?.GetComponent<Button>();
            
            if (icon != null && item.itemData.icon != null)
                icon.sprite = item.itemData.icon;
            
            if (nameText != null)
            {
                string equipped = item.isEquipped ? " [E]" : "";
                string persistent = item.itemData.isPersistent ? "◆" : "○";
                nameText.text = $"{persistent} {item.itemData.displayName}{equipped}";
                // Override with clean, ASCII-only persistent marker
                nameText.text = $"{(item.itemData.isPersistent ? "(P) " : string.Empty)}{item.itemData.displayName}{equipped}";
            }
            
            if (countText != null)
                countText.text = $"x{item.count}";
            
            // Setup equip button for special items
            if (equipButton != null)
            {
                if (item.itemData.isEquippable)
                {
                    equipButton.gameObject.SetActive(true);
                    var buttonText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                        buttonText.text = item.isEquipped ? "Unequip" : "Equip";
                    
                    equipButton.onClick.RemoveAllListeners();
                    equipButton.onClick.AddListener(() =>
                    {
                        if (item.isEquipped)
                            inventorySystem.UnequipItem(item.itemData.itemId);
                        else
                            inventorySystem.EquipItem(item.itemData.itemId);
                    });
                }
                else
                {
                    equipButton.gameObject.SetActive(false);
                }
            }
        }

        private void RefreshEquippedDisplay()
        {
            if (equippedItemText == null) return;
            
            var equipped = inventorySystem?.GetEquippedItem();
            if (equipped != null)
            {
                var name = equipped.itemData != null ? equipped.itemData.displayName : "(Unknown Item)";
                equippedItemText.text = $"Equipped: {name}";
            }
            else
            {
                equippedItemText.text = "No item equipped";
            }
        }
    }

    // ScriptRole: Extended inventory UI with equip functionality
    // RelatedScripts: InventorySystem
    // UsesSO: GameEventSO
    // ReceivesFrom: InventorySystem events
}
