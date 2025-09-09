// Checks if current in-game time is before a given hour:minute
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
    public class TimeBeforeNode : HybridNode
    {
        [ConversationProperty("Hour",   120, 200, 255)] [SerializeField] private int hour = 9;
        [ConversationProperty("Minute", 120, 200, 255)] [SerializeField] private int minute = 0;
        [ConversationProperty("Inclusive", 120, 200, 255)] [SerializeField] private bool inclusive = false;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var time = GameServices.Time ?? UnityEngine.Object.FindFirstObjectByType<TimeSystem>();
            int threshold = Mathf.Clamp(hour, 0, 23) * 60 + Mathf.Clamp(minute, 0, 59);
            int current = time != null ? time.GetTotalMinutes() : 0;

            _result = inclusive ? (current <= threshold) : (current < threshold);
            _hasResult = true;
            Continue(conversation, events);
        }
    }
}

