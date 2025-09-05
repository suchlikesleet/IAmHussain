// Assets/BOH/Scripts/DialogueS/BOHConversa/Runtime/SetFlagNode.cs
using System;
using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH.Conversa
{
    [Serializable]
    [Port("Previous","previous",typeof(BaseNode),Flow.In,Capacity.Many)]
    [Port("Next","next",typeof(BaseNode),Flow.Out,Capacity.One)]
    [Port("Result","value",typeof(bool),Flow.Out,Capacity.One)]
    public class SetFlagNode : HybridNode
    {
        [ConversationProperty("Flag", 190, 120, 255)] [SerializeField] private string flag;
        [ConversationProperty("Value", 190, 120, 255)] [SerializeField] private bool value = true;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            if (GameServices.Flags is FlagService svc && !string.IsNullOrEmpty(flag))
            {
                svc.SetFlag(flag, value);
                _result = true;
            }
            else
            {
                Debug.LogWarning("[SetFlagNode] FlagService missing or flag empty");
                _result = false;
            }
            _hasResult = true;
            Continue(conversation, events);
        }
    }
}