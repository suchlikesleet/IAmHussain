using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH
{
    [Port("Value", "value", typeof(bool), Flow.Out, Capacity.One)]
    [System.Serializable]
    public sealed class HasActiveErrandNode : BaseNode, IValueNode
    {
        [SerializeField, ConversationProperty("Errand", 200, 160, 70)]
        private ErrandSO errand;

        [Slot("Errand (In)", "errand", Flow.In, Capacity.One)]
        public ErrandSO ErrandInput { get; set; }

        public T GetValue<T>(string portGuid, Conversation conversation)
        {
            var sys = Object.FindFirstObjectByType<ErrandSystem>();
            var e = ErrandInput ?? errand;
            bool has = sys != null && e != null &&
                       sys.GetActiveErrands().Any(a => a.errandData && a.errandData.errandId == e.errandId); // :contentReference[oaicite:8]{index=8}
            return (T)(object)has;
        }
    }
}