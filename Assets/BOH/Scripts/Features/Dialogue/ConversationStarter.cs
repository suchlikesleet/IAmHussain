using BOH.Conversa;
using UnityEngine;
using Conversa.Runtime;

namespace BOH
{
    /// <summary>
    /// Attach to an NPC. Requires a trigger collider on this GameObject.
    /// Starts a Conversa Conversation when the player is inside the trigger.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ConversationStarter : MonoBehaviour
    {
        [Header("Conversation")]
        [Tooltip("The conversation asset this NPC should start.")]
        public Conversation conversation;

        [Tooltip("If true, start as soon as player enters the trigger. If false, wait for a key press.")]
        public bool autoStartOnEnter = false;

        [Tooltip("Key to press to start the conversation (when autoStartOnEnter is false).")]
        public KeyCode interactKey = KeyCode.E;

        [Header("Optional")]
        [Tooltip("If not set, we try to FindFirstObjectByType<MyConversaController>().")]
        public MyConversaController conversaController;

        [Tooltip("Show a simple on-screen prompt when player is in range (optional).")]
        public GameObject promptUI;

        // State
        private bool _playerInside = false;
        private Transform _player;

        private void Reset()
        {
            // Force collider to be trigger
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void Awake()
        {
            if (conversaController == null)
                conversaController = FindFirstObjectByType<MyConversaController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other)) return;
            _playerInside = true;
            _player = other.transform;

            if (promptUI) promptUI.SetActive(!autoStartOnEnter);

            if (autoStartOnEnter)
                TryStartConversation();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsPlayer(other)) return;

            if (!autoStartOnEnter && Input.GetKeyDown(interactKey))
                TryStartConversation();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsPlayer(other)) return;
            _playerInside = false;
            _player = null;
            if (promptUI) promptUI.SetActive(false);
        }

        private bool IsPlayer(Collider col)
        {
            // Easiest: tag your player "Player"
            return col.CompareTag("Player");
        }

        private void TryStartConversation()
        {
            if (conversation == null || conversaController == null) return;

            // Optional: if you added queue/immediate APIs, choose one:
            // conversaController.SwitchConversationImmediate(conversation);
            // conversaController.QueueConversation(conversation);

            conversaController.StartConversation(conversation); // vanilla start
            if (promptUI) promptUI.SetActive(false);
        }

        // Optional gizmo to see trigger in scene view
        private void OnDrawGizmosSelected()
        {
            var col = GetComponent<Collider>();
            if (col && col.isTrigger)
            {
                Gizmos.color = new Color(0.2f, 0.7f, 1f, 0.25f);
                Gizmos.matrix = transform.localToWorldMatrix;
                if (col is SphereCollider s) Gizmos.DrawSphere(s.center, s.radius);
                else if (col is BoxCollider b) Gizmos.DrawCube(b.center, b.size);
                else if (col is CapsuleCollider c) Gizmos.DrawSphere(c.center, Mathf.Max(c.radius, c.height * 0.5f));
            }
        }
    }
}
