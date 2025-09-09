using System.Linq;
using Obvious.Soap;
using UnityEngine;

namespace BOH
{
    public class SpecialItemIndicator : MonoBehaviour
    {
        [Header("NPC Context")]
        [SerializeField] private ContactSO contact; // This NPC's contact id
        [SerializeField] private GameObject indicator;
        [SerializeField] private bool showOnlyInRange = true;
        [SerializeField] private float interactionRadius = 2f;

        [Header("Optional Dialogue Gate")] 
        [SerializeField] private DialogueGateSO gate; // If set, use equipTag + NPC filters

        [Header("Events (assign the same assets used by InventorySystem)")]
        [SerializeField] private ScriptableEventNoParam onItemEquipped;
        [SerializeField] private ScriptableEventNoParam onInventoryChanged;
        [SerializeField] private ScriptableEventNoParam onDayEnd;

        private InventorySystem inventorySystem;
        private GiftingSystem giftingSystem;
        private Transform playerTransform;
        private bool playerInRange;

        private void OnEnable()
        {
            if (onItemEquipped != null) onItemEquipped.OnRaised += RefreshIndicator;
            if (onInventoryChanged != null) onInventoryChanged.OnRaised += RefreshIndicator;
            if (onDayEnd != null) onDayEnd.OnRaised += HideIndicator;
        }

        private void OnDisable()
        {
            if (onItemEquipped != null) onItemEquipped.OnRaised -= RefreshIndicator;
            if (onInventoryChanged != null) onInventoryChanged.OnRaised -= RefreshIndicator;
            if (onDayEnd != null) onDayEnd.OnRaised -= HideIndicator;
        }

        private void Start()
        {
            inventorySystem = GameServices.Inventory ?? FindFirstObjectByType<InventorySystem>();
            giftingSystem = GameServices.Gifting ?? FindFirstObjectByType<GiftingSystem>();

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;

            HideIndicator();
            RefreshIndicator();
        }

        private void Update()
        {
            if (!showOnlyInRange) return;
            if (playerTransform == null) return;

            bool wasInRange = playerInRange;
            playerInRange = Vector3.Distance(transform.position, playerTransform.position) <= interactionRadius;
            if (playerInRange != wasInRange)
            {
                RefreshIndicator();
            }
        }

        private void HideIndicator()
        {
            if (indicator != null) indicator.SetActive(false);
        }

        private void RefreshIndicator()
        {
            if (indicator == null)
                return;

            if (showOnlyInRange && !playerInRange)
            {
                indicator.SetActive(false);
                return;
            }

            var equipped = inventorySystem?.GetEquippedItem();
            if (equipped == null || equipped.itemData == null)
            {
                indicator.SetActive(false);
                return;
            }

            bool valid = false;

            // Option A: Dialogue gate check via equipTag and NPC filters
            if (gate != null)
            {
                var equipTag = equipped.itemData.equipTag;
                if (!string.IsNullOrEmpty(gate.requiredEquipTag) && gate.requiredEquipTag == equipTag)
                {
                    bool npcAllowed = true;
                    if (gate.npcFilter != null && gate.npcFilter.Count > 0)
                    {
                        npcAllowed = (contact != null) && gate.npcFilter.Contains(contact);
                    }
                    if (npcAllowed && gate.npcTags != null && gate.npcTags.Count > 0)
                    {
                        // If tags filter is provided, ensure at least one tag matches
                        npcAllowed = contact != null && contact.tags != null && contact.tags.Any(t => gate.npcTags.Contains(t));
                    }
                    valid = npcAllowed;
                }
            }

            // Option B: Gifting rules check per item -> recipient
            if (!valid && giftingSystem != null && contact != null)
            {
                var list = giftingSystem.GetValidRecipients(equipped.itemData.itemId);
                if (list != null)
                    valid = list.Contains(contact.contactId);
            }

            indicator.SetActive(valid);
        }
    }
}
