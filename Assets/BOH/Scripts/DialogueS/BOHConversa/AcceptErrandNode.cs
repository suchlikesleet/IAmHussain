using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH.Conversa
{
    [Port("Previous", "previous", typeof(BaseNode), Flow.In,  Capacity.Many)]
    [Port("Next",     "next",     typeof(BaseNode), Flow.Out, Capacity.One)]
    [System.Serializable]
    public sealed class AcceptErrandNode : BaseNode, IEventNode
    {
        [SerializeField, ConversationProperty("Errand", 56, 128, 255)]
        private ErrandSO errand;

        [Slot("Errand (In)", "errand", Flow.In, Capacity.One)]
        public ErrandSO ErrandInput { get; set; }

        public void Process(Conversation conversation, ConversationEvents events)
        {
            var sys = Object.FindFirstObjectByType<ErrandSystem>();
            var e = ErrandInput ?? errand;
            if (sys && e) sys.AddErrand(e); // uses your API (accept)  // 

            var next = conversation.GetOppositeNodes(GetNodePort("next")).FirstOrDefault();
            if (next != null) conversation.Process(next, events);
        }
    }
}