using UnityEngine;

namespace BOH
{
    [CreateAssetMenu(fileName = "Contact", menuName = "BOH/Data/Contact")]
    public class ContactSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string contactId;
        public string displayName;
        public string role; // e.g., "Bookseller", "Nurse", "Teacher"
        
        [Header("Trust")]
        public int startingTrust = 0;
        public int maxTrust = 3;
        
        [Header("Bond")]
        public bool isBondProspect = false;
        public string bondArchetype; // "loyalty", "compassion", "wisdom"
        
        [Header("Tags")]
        public string[] tags; // e.g., ["shopkeeper", "poor", "official"]
    }

    // ScriptRole: NPC contact definition
    // RelatedScripts: ContactSystem, GiftingRulesSO
}