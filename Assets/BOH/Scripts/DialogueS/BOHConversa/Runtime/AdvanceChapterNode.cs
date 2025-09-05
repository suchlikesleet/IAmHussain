// Assets/BOH/Scripts/DialogueS/BOHConversa/Runtime/AdvanceChapterNode.cs
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
    public class AdvanceChapterNode : HybridNode
    {
        [ConversationProperty("Delta", 120, 200, 120)] [SerializeField] private int delta = 1;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            if (GameServices.Story is StoryService svc)
            {
                svc.AdvanceChapter(delta);
                _result = true;
            }
            else
            {
                Debug.LogWarning("[AdvanceChapterNode] StoryService missing");
                _result = false;
            }
            _hasResult = true;
            Continue(conversation, events);
        }
    }
}