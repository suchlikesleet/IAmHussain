using UnityEngine;
using System.Collections.Generic;

namespace BOH
{
    [CreateAssetMenu(fileName = "GiftingRules", menuName = "BOH/Data/GiftingRules")]
    public class GiftingRulesSO : ScriptableObject
    {
        [Header("Item")]
        public ItemSO item;
        
        [Header("Recipients")]
        public List<RecipientRule> recipients = new List<RecipientRule>();
        
        [Header("Default Outcome")]
        public GiftOutcome defaultOutcome;
        
        [System.Serializable]
        public class RecipientRule
        {
            public string contact; // ContactSO id or tag
            public List<Condition> conditions = new List<Condition>();
            public GiftOutcome outcome;
        }
        
        [System.Serializable]
        public class Condition
        {
            public int minTrust = 0;
            public int chapterRange = 1;
            public int bondState = 0;
            public string equipRequired = "";
        }
        
        [System.Serializable]
        public class GiftOutcome
        {
            public int blessingsDelta = 0;
            public int trustDelta = 0;
            public int bondDelta = 0;
            public int moneyDelta = 0;
            public string journalLine = "";
            public string fxPrefab = "";
        }
    }

    // ScriptRole: Defines gifting rules and outcomes for special items
    // RelatedScripts: DialogueSystem, InventorySystem
}