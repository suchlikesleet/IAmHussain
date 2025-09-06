// Assets/BOH/Scripts/DialogueS/BOHConversa/Runtime/HasActiveErrandNode.cs
using System;
using Conversa.Runtime;
using Conversa.Runtime.Interfaces;
using UnityEngine;
using BOH;
using Conversa.Runtime.Attributes;

namespace BOH.Conversa
{
    [Serializable]
    [Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
    [Port("Next",     "next",     typeof(BaseNode), Flow.Out, Capacity.One)]
    [Port("Result",   "value",    typeof(bool),     Flow.Out, Capacity.One)]
    public class HasActiveErrandNode : HybridNode
    {
        [ConversationProperty("Errand Id", 230, 170, 60)]
        [SerializeField] private string errandId;

        [ConversationProperty("Errand (optional)", 64, 153, 242)]
        [SerializeField] private ErrandSO errand;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var id = !string.IsNullOrEmpty(errandId) ? errandId : (errand != null ? errand.errandId : null);
            var sys = GameServices.Errands ?? FindErrands();

            _result = (sys != null && !string.IsNullOrEmpty(id) && sys.HasActive(id));
            _hasResult = true;
            Continue(conversation, events);
        }
    }
}
