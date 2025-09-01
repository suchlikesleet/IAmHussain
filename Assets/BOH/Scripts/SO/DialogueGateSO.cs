using UnityEngine;
using System.Collections.Generic;

namespace BOH
{
    [CreateAssetMenu(fileName = "DialogueGate", menuName = "BOH/Data/DialogueGate")]
    public class DialogueGateSO : ScriptableObject
    {
        [Header("Required Equipment")]
        public string requiredEquipTag; // e.g., "FOUND_NECKLACE"
        
        [Header("NPC Filter")]
        public List<ContactSO> npcFilter = new List<ContactSO>();
        public List<string> npcTags = new List<string>();
        
        [Header("Dialogue")]
        [TextArea(3, 5)]
        public string offerText = "Offer the {0}?";
        public string declineText = "Keep it";
        
        [Header("Priority")]
        public int priority = 0; // UI sorting
    }

    // ScriptRole: Gates dialogue options based on equipped items
    // RelatedScripts: DialogueSystem, NPCInteraction
}