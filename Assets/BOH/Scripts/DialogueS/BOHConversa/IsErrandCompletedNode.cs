using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH
{
    [Port("Value", "value", typeof(bool), Flow.Out, Capacity.One)]
    [System.Serializable]
    public sealed class IsErrandCompletedNode : BaseNode, IValueNode
    {
        [SerializeField, ConversationProperty("Errand", 160, 120, 255)]
        private ErrandSO errand;

        [Slot("Errand (In)", "errand", Flow.In, Capacity.One)]
        public ErrandSO ErrandInput { get; set; }

        public T GetValue<T>(string portGuid, Conversation conversation)
        {
            var sys = Object.FindFirstObjectByType<ErrandSystem>();
            var e = ErrandInput ?? errand;
            bool done = sys != null && e != null && sys.IsErrandCompleted(e.errandId); // :contentReference[oaicite:9]{index=9}
            return (T)(object)done;
        }
    }
}