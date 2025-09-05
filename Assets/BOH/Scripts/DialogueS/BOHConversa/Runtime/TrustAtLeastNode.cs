// Assets/BOH/Scripts/DialogueS/BOHConversa/Runtime/TrustAtLeastNode.cs
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
    public class TrustAtLeastNode : HybridNode
    {
        [ConversationProperty("Contact Id", 242, 128, 170)]
        [SerializeField] private string contactId;

        [ConversationProperty("Min Trust", 242, 128, 170)]
        [SerializeField] private int minTrust = 0;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var contacts = GameServices.Contacts ?? FindContacts();
            if (contacts == null)
            {
                Debug.LogWarning("[TrustAtLeastNode] No ContactSystem found");
                _result = false;
            }
            else
            {
                // Assuming ContactSystem exposes GetTrust(string). If not, adapt to your API.
                int t = contacts.GetTrust(contactId);
                _result = t >= minTrust;
            }

            _hasResult = true;
            Continue(conversation, events);
        }
    }
}