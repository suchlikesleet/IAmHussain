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
    public class ErrandCompleteNode : HybridNode
    {
        [ConversationProperty("Errand", 0.25f, 0.6f, 0.95f)]
        [SerializeField] private ErrandSO errand;

        public override void Process(Conversation conversation, ConversationEvents conversationEvents)
        {
            var errands = FindErrands();
            _result = false;

            if (errands != null && errand != null && !string.IsNullOrEmpty(errand.errandId))
            {
                _result = errands.TryCompleteErrand(errand.errandId);
            }

            _hasResult = true;
            Continue(conversation, conversationEvents);
        }
    }
}