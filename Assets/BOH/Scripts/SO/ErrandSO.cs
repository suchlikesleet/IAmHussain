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
        
        [Header("Availability (Optional)")]
        [Tooltip("If set, NPC/graph should only offer when all are present.")]
        public string[] requiredFlags = System.Array.Empty<string>();

        [Tooltip("If any is present, do NOT offer.")]
        public string[] blockedFlags = System.Array.Empty<string>();

        [Tooltip("Require these errands to be completed before offering this one.")]
        public string[] requiredCompletedErrands = System.Array.Empty<string>();

        [Tooltip("Minimum trust needed with the offering NPC (if you use trust gating).")]
        public int minTrust = 0;

        [Tooltip("Story/Chapter gating; -1 means 'no gate'.")]
        public int chapterMin = -1;
        public int chapterMax = -1;

        [Tooltip("Controls re-offer/rehire cadence after completion.")]
        public int cooldownDays = 0;

        [Tooltip("Can this errand be completed more than once overall?")]
        public bool repeatable = false;

        [Tooltip("If repeatable, how many times at most? 0 = unlimited.")]
        public int maxCompletions = 0;

        [Tooltip("Higher = offered earlier when multiple are available.")]
        public int priority = 0;

        [Tooltip("Optional tag of which NPC typically offers this (pure metadata if you drive via graphs).")]
        public string offeredByNpcId;
        
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