using UnityEngine;

namespace BOH
{
    [CreateAssetMenu(fileName = "Trigger", menuName = "BOH/Data/Trigger")]
    public class TriggerSO : ScriptableObject
    {
        public enum TriggerType
        {
            NPC,        // Proximity to NPC
            Phone,      // Time-based phone call
            Ambient     // Area/condition based
        }

        [Header("Trigger Info")]
        public string triggerId;
        public TriggerType type = TriggerType.NPC;
        public ErrandSO errandToOffer;
        
        [Header("Activation")]
        public int activationDay = 1;
        public int startHour = 6;
        public int endHour = 22;
        public bool isRepeatable = false;
        
        [Header("NPC Settings")]
        public string npcName = "Grocer";
        public float triggerRadius = 2f;
        
        [Header("Dialogue")]
        [TextArea(3, 5)]
        public string offerText = "Can you help me with something?";
        public string acceptText = "Yes, I'll help";
        public string declineText = "Sorry, not now";
        
        [Header("Preconditions")]
        public int requiredTrustLevel = 0;
        public ErrandSO prerequisiteErrand;
    }

    // ScriptRole: Trigger configuration for offering errands
    // RelatedScripts: TriggerSystem, ErrandSO
}