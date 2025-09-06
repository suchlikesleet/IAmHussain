// Assets/BOH/Scripts/DialogueS/BOHConversa/Runtime/HasFlagNode.cs
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
    public class HasFlagNode : HybridNode
    {
        [ConversationProperty("Flag", 190, 120, 255)]
        [SerializeField] private string flag;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var flags = GameServices.Flags; // interface service
            if (flags == null)
            {
                Debug.LogWarning("[HasFlagNode] No IFlagService set on GameServices.Flags");
                _result = false;
            }
            else
            {
                _result = !string.IsNullOrEmpty(flag) && flags.HasFlag(flag);
            }

            _hasResult = true;
            Continue(conversation, events);
        }
    }
}
