using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Obvious.Soap;

namespace BOH
{
    public class GiftingSystem : MonoBehaviour
    {
        [Header("Gifting Rules")]
        [SerializeField] private List<GiftingRulesSO> giftingRules = new List<GiftingRulesSO>();
        
        [Header("Events")]
        [SerializeField] private ScriptableEventNoParam onGiftGiven;
        
        private ResourceSystem resourceSystem;
        private ContactSystem contactSystem;
        private JournalSystem journalSystem;

        private void Start()
        {
            resourceSystem = GameServices.Resources ?? FindFirstObjectByType<ResourceSystem>();
            contactSystem  = GameServices.Contacts  ?? FindFirstObjectByType<ContactSystem>();
            journalSystem  = GameServices.Journal   ?? FindFirstObjectByType<JournalSystem>();
        }

        public bool ProcessGift(string itemId, string recipientId)
        {
            // Find gifting rules for this item
            var rule = giftingRules.FirstOrDefault(r => r.item.itemId == itemId);
            if (rule == null)
            {
                Debug.LogWarning($"No gifting rules found for item: {itemId}");
                return false;
            }

            // Find recipient rule
            var recipientRule = rule.recipients.FirstOrDefault(r => r.contact == recipientId);
            var outcome = recipientRule?.outcome ?? rule.defaultOutcome;

            if (outcome == null)
            {
                Debug.LogWarning($"No outcome defined for {itemId} to {recipientId}");
                return false;
            }

            // Apply outcomes
            ApplyOutcome(outcome, itemId, recipientId);
            
            Debug.Log($"Gift processed: {itemId} to {recipientId}");
            onGiftGiven?.Raise();
            return true;
        }

        private void ApplyOutcome(GiftingRulesSO.GiftOutcome outcome, string itemId, string recipientId)
        {
            // Apply resource changes
            if (resourceSystem != null)
            {
                if (outcome.blessingsDelta != 0)
                {
                    if (outcome.blessingsDelta > 0)
                        resourceSystem.AddBlessings(outcome.blessingsDelta);
                    else
                        resourceSystem.RemoveBlessings(-outcome.blessingsDelta);
                }
                
                if (outcome.moneyDelta != 0)
                {
                    if (outcome.moneyDelta > 0)
                        resourceSystem.AddMoney(outcome.moneyDelta);
                    else
                        resourceSystem.SpendMoney(-outcome.moneyDelta);
                }
            }

            // Apply trust changes
            if (contactSystem != null && outcome.trustDelta != 0)
            {
                contactSystem.ModifyTrust(recipientId, outcome.trustDelta);
            }

            // Record in journal
            if (journalSystem != null && !string.IsNullOrEmpty(outcome.journalLine))
            {
                journalSystem.AddSpecialEntry(outcome.journalLine);
            }

            // Play effect if specified
            if (!string.IsNullOrEmpty(outcome.fxPrefab))
            {
                // Load and play effect (stub)
                Debug.Log($"Playing effect: {outcome.fxPrefab}");
            }
        }

        public List<string> GetValidRecipients(string itemId)
        {
            var rule = giftingRules.FirstOrDefault(r => r.item.itemId == itemId);
            if (rule == null) return new List<string>();
            
            return rule.recipients
                .Where(r => CheckConditions(r.conditions))
                .Select(r => r.contact)
                .ToList();
        }

        private bool CheckConditions(List<GiftingRulesSO.Condition> conditions)
        {
            // Check all conditions (stub for now)
            return true;
        }
    }

    // ScriptRole: Processes gift transactions and outcomes
    // RelatedScripts: InventorySystem, ContactSystem, ResourceSystem
    // UsesSO: GiftingRulesSO, GameEventSO
    // ReceivesFrom: DialogueSystem, InventorySystem
    // SendsTo: ResourceSystem, ContactSystem, JournalSystem
}
