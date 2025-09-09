using System;
using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH.Conversa
{
    [Serializable]
    [Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
    [Port("Next",     "next",     typeof(BaseNode), Flow.Out, Capacity.One)]
    [Port("Result",   "value",    typeof(bool),     Flow.Out, Capacity.One)]
    public class JournalAddEntryNode : HybridNode
    {
        [ConversationProperty("Type",  200, 160, 200)] [SerializeField] private JournalSystem.EntryType entryType = JournalSystem.EntryType.Special;
        [ConversationProperty("Message", 200, 160, 200)] [SerializeField] [TextArea(2,4)] private string message = "";

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var journal = GameServices.Journal ?? UnityEngine.Object.FindFirstObjectByType<JournalSystem>();
            if (journal == null)
            {
                Debug.LogWarning("[JournalAddEntryNode] JournalSystem not found.");
                _result = false; _hasResult = true; Continue(conversation, events); return;
            }

            // JournalSystem currently exposes AddSpecialEntry and AddGiftEntry.
            // Use AddSpecialEntry for generic messages regardless of type for now.
            if (!string.IsNullOrEmpty(message))
            {
                journal.AddSpecialEntry(message);
                _result = true;
            }
            else
            {
                _result = false;
            }

            _hasResult = true;
            Continue(conversation, events);
        }
    }
}

