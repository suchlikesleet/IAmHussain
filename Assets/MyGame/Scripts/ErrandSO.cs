using UnityEngine;
using System.Collections.Generic;

namespace BOH
{
    [CreateAssetMenu(fileName = "Errand", menuName = "BOH/Data/Errand")]
    public class ErrandSO : ScriptableObject
    {
        public enum ErrandType
        {
            Strict,      // Same day completion
            MultiDay,    // Active across multiple days
            Persistent,  // No deadline
            FollowUp,    // Chains to another errand
            PlayerPlanned // Optional, no trigger
        }

        [Header("Basic Info")]
        public string errandId;
        public string errandTitle;
        [TextArea(2, 3)]
        public string description;
        public ErrandType type = ErrandType.Strict;
        
        [Header("Time Window")]
        public int startDay = 1;
        public int expiryDay = 1;
        public int startHour = 6;
        public int endHour = 22;
        
        [Header("Requirements")]
        public List<ItemRequirement> itemsRequired = new List<ItemRequirement>();
        public int energyCost = 0;
        
        [Header("Rewards")]
        public int blessingsReward = 10;
        public int moneyReward = 0;
        public List<ItemReward> itemRewards = new List<ItemReward>();
        
        [Header("Outcomes")]
        [TextArea(2, 3)]
        public string onTimeOutcome = "You completed the task on time.";
        [TextArea(2, 3)]
        public string lateOutcome = "You were late, but it still helped.";
        [TextArea(2, 3)]
        public string failOutcome = "You couldn't complete this task.";
        
        [Header("Chain")]
        public ErrandSO followUpErrand;
        
        [System.Serializable]
        public class ItemRequirement
        {
            public ItemSO item;
            public int count = 1;
        }
        
        [System.Serializable]
        public class ItemReward
        {
            public ItemSO item;
            public int count = 1;
        }
    }

    // ScriptRole: Errand definition with requirements and rewards
    // RelatedScripts: ErrandSystem, TriggerSO
}