using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH
{
    [Port("Previous",  "previous",  typeof(BaseNode), Flow.In,  Capacity.Many)]
    [Port("On Success","onSuccess", typeof(BaseNode), Flow.Out, Capacity.One)]
    [Port("On Fail",   "onFail",    typeof(BaseNode), Flow.Out, Capacity.One)]
    [System.Serializable]
    public sealed class AttemptCompleteErrandNode : BaseNode, IEventNode
    {
        [SerializeField, ConversationProperty("Errand", 56, 200, 120)]
        private ErrandSO errand;

        [Slot("Errand (In)", "errand", Flow.In, Capacity.One)]
        public ErrandSO ErrandInput { get; set; }

        public void Process(Conversation conversation, ConversationEvents events)
        {
            var sys = Object.FindFirstObjectByType<ErrandSystem>();
            var e = ErrandInput ?? errand;
            bool ok = sys != null && e != null && !string.IsNullOrEmpty(e.errandId)
                      && sys.TryCompleteErrand(e.errandId); // checks reqs/consumes/rewards/late/follow-up :contentReference[oaicite:7]{index=7}

            var port = GetNodePort(ok ? "onSuccess" : "onFail");
            var next = conversation.GetOppositeNodes(port).FirstOrDefault();
            if (next != null) conversation.Process(next, events);
        }
    }
}