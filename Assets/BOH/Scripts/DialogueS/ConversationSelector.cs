using UnityEngine;
using Conversa.Runtime;
using System.Collections.Generic;

namespace BOH
{
    [System.Serializable]
    public class ConversationEntry
    {
        public Conversation conversation;
        public string requiredFlag;  // leave empty to ignore
        public int minChapter = -1;  // -1 = ignore
        public int minTrust = -999;  // ignore if Contacts not used
        public string contactId;     // whose trust to check
        public int priority = 0;     // higher wins
    }

    /// <summary>
    /// Decide which Conversation to start now.
    /// Use with ConversationStarter: call Select() then pass to StartConversation.
    /// </summary>
    public class ConversationSelector : MonoBehaviour
    {
        public List<ConversationEntry> options = new();

        public Conversation Select()
        {
            Conversation best = null;
            int bestPriority = int.MinValue;

            foreach (var e in options)
            {
                if (e.conversation == null) continue;
                if (!string.IsNullOrEmpty(e.requiredFlag) && (GameServices.Flags == null || !GameServices.Flags.HasFlag(e.requiredFlag)))
                    continue;
                if (e.minChapter >= 0 && (GameServices.Story == null || GameServices.Story.GetChapter() < e.minChapter))
                    continue;
                if (!string.IsNullOrEmpty(e.contactId) && GameServices.Contacts != null && e.minTrust > int.MinValue)
                {
                    if (GameServices.Contacts.GetTrust(e.contactId) < e.minTrust)
                        continue;
                }

                if (e.priority >= bestPriority)
                {
                    bestPriority = e.priority;
                    best = e.conversation;
                }
            }

            return best;
        }
    }
}