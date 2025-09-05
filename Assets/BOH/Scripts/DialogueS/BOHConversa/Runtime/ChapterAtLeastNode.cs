// Assets/BOH/Scripts/DialogueS/BOHConversa/Runtime/ChapterAtLeastNode.cs
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
    public class ChapterAtLeastNode : HybridNode
    {
        [ConversationProperty("Min Chapter", 120, 200, 120)]
        [SerializeField] private int minChapter = 0;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var story = GameServices.Story;
            if (story == null)
            {
                Debug.LogWarning("[ChapterAtLeastNode] No IStoryService set on GameServices.Story");
                _result = false;
            }
            else
            {
                _result = story.GetChapter() >= minChapter;
            }

            _hasResult = true;
            Continue(conversation, events);
        }
    }
}