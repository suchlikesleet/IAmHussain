using System;
using Conversa.Runtime;
using Conversa.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using BOH;
using Conversa.Runtime.Attributes;

namespace BOH.Conversa
{
    [MovedFrom(true, null, "Assembly-CSharp")]
    [Serializable]
    [Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
    [Port("Next", "next", typeof(BaseNode), Flow.Out, Capacity.One)]
    [Port("Result", "value", typeof(bool), Flow.Out, Capacity.One)]
    public class ErrandCheckNode : HybridNode
    {
        [ConversationProperty("Errand Id", 0.9f, 0.6f, 0.2f)]
        [SerializeField] private string errandId;

        [ConversationProperty("Errand (optional)", 0.25f, 0.6f, 0.95f)]
        [SerializeField] private ErrandSO errand;

        public override void Process(Conversation conversation, ConversationEvents conversationEvents)
        {
            var id = !string.IsNullOrEmpty(errandId)
                ? errandId
                : (errand != null ? errand.errandId : null);

            var errands = FindErrands();
            _result = (errands != null && !string.IsNullOrEmpty(id) && errands.IsErrandCompleted(id));
            _hasResult = true;

            Continue(conversation, conversationEvents);
        }
    }
}