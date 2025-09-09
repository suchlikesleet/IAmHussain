using System;
using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH.Conversa
{
    [Serializable]
    [Port("Previous",     "previous",     typeof(BaseNode), Flow.In,  Capacity.Many)]
    [Port("Not Started",  "not-started",  typeof(BaseNode), Flow.Out, Capacity.One)]
    [Port("Active",       "active",       typeof(BaseNode), Flow.Out, Capacity.One)]
    [Port("Completed",    "completed",    typeof(BaseNode), Flow.Out, Capacity.One)]
    public class ErrandStateBranchNode : BaseNode, IEventNode
    {
        [ConversationProperty("Errand Id", 0.9f, 0.6f, 0.2f)]
        [SerializeField] private string errandId;

        [ConversationProperty("Errand (optional)", 0.25f, 0.6f, 0.95f)]
        [SerializeField] private ErrandSO errand;

        public void Process(Conversation conversation, ConversationEvents conversationEvents)
        {
            var id = !string.IsNullOrEmpty(errandId) ? errandId : (errand != null ? errand.errandId : null);
            var sys = GameServices.Errands ?? UnityEngine.Object.FindFirstObjectByType<ErrandSystem>();

            string port = "not-started";
            if (sys != null && !string.IsNullOrEmpty(id))
            {
                if (sys.IsErrandCompleted(id)) port = "completed";
                else if (sys.HasActive(id))    port = "active";
                else                           port = "not-started";
            }

            var next = conversation.GetOppositeNodes(GetNodePort(port)).FirstOrDefault();
            conversation.Process(next, conversationEvents);
        }
    }
}

