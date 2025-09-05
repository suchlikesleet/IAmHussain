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
    public class TrustChangeNode : HybridNode
    {
        [ConversationProperty("Contact Id", 0.95f, 0.5f, 0.7f)]
        [SerializeField] private string contactId;


        [ConversationProperty("Delta", 0.95f, 0.5f, 0.7f)]
        [SerializeField] private int delta = 1;

        public override void Process(Conversation conversation, ConversationEvents conversationEvents)
        {
            var contacts = FindContacts();
            _result = false;

            if (contacts != null && !string.IsNullOrEmpty(contactId))
            {
                contacts.ModifyTrust(contactId, delta);
                _result = true;
            }

            _hasResult = true;
            Continue(conversation, conversationEvents);
        }
    }
}