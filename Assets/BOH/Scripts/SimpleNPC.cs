using UnityEngine;
using UnityEngine.InputSystem;

namespace BOH
{
    public class SimpleNPC : MonoBehaviour
    {
        [Header("NPC Info")]
        [SerializeField] private string npcName = "Grocer";
        [SerializeField] private float interactionRadius = 2f;
        
        [Header("Visual")]
        [SerializeField] private GameObject interactionPrompt;
        
        private Transform playerTransform;
        private bool playerInRange = false;

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            
            gameObject.name = npcName; // Ensure name matches for TriggerSystem
            
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }

        private void Update()
        {
            if (playerTransform == null) return;
            
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            bool wasInRange = playerInRange;
            playerInRange = distance <= interactionRadius;
            
            if (playerInRange != wasInRange)
            {
                if (interactionPrompt != null)
                    interactionPrompt.SetActive(playerInRange);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }

    // ScriptRole: Simple NPC for proximity triggers
    // RelatedScripts: TriggerSystem
}