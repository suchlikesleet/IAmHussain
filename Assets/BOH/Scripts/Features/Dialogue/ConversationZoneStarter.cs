using BOH.Conversa;

using UnityEngine;
using Conversa.Runtime;

namespace BOH
{
    /// <summary>
    /// Attach to a 2D trigger zone. Starts a Conversation automatically when the player enters.
    /// Useful for cutscene intros, doorways, etc.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ConversationZoneStarter : MonoBehaviour
    {
        public Conversation conversation;
        public bool onlyOnce = true;

        [Tooltip("If not set, we try to FindFirstObjectByType<MyConversaController>().")]
        public MyConversaController conversaController;

        private bool _consumed = false;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }

        private void Awake()
        {
            if (conversaController == null)
                conversaController = FindFirstObjectByType<MyConversaController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_consumed && onlyOnce) return;
            if (!other.CompareTag("Player")) return;

            if (conversation != null && conversaController != null)
            {
                conversaController.StartConversation(conversation);
                if (onlyOnce) _consumed = true;
            }
        }
    }
}
