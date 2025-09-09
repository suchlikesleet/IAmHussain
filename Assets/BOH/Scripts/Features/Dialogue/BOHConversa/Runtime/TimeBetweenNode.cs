// Checks if current time is between a start and end time, handling wrap-around
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
    public class TimeBetweenNode : HybridNode
    {
        [ConversationProperty("Start Hour",   120, 200, 255)] [SerializeField] private int startHour = 9;
        [ConversationProperty("Start Minute", 120, 200, 255)] [SerializeField] private int startMinute = 0;
        [ConversationProperty("End Hour",     120, 200, 255)] [SerializeField] private int endHour = 17;
        [ConversationProperty("End Minute",   120, 200, 255)] [SerializeField] private int endMinute = 0;
        [ConversationProperty("Inclusive Start", 120, 200, 255)] [SerializeField] private bool inclusiveStart = true;
        [ConversationProperty("Inclusive End",   120, 200, 255)] [SerializeField] private bool inclusiveEnd   = false;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var time = GameServices.Time ?? UnityEngine.Object.FindFirstObjectByType<TimeSystem>();
            if (time == null)
            {
                Debug.LogWarning("[TimeBetweenNode] No TimeSystem found. Returning false.");
                _result = false;
                _hasResult = true;
                Continue(conversation, events);
                return;
            }

            int start = Mathf.Clamp(startHour, 0, 23) * 60 + Mathf.Clamp(startMinute, 0, 59);
            int end   = Mathf.Clamp(endHour,   0, 23) * 60 + Mathf.Clamp(endMinute,   0, 59);
            int cur   = time.GetTotalMinutes();

            bool afterStart = inclusiveStart ? cur >= start : cur > start;
            bool beforeEnd  = inclusiveEnd   ? cur <= end   : cur < end;

            if (start <= end)
                _result = afterStart && beforeEnd;
            else
                // Wrap-around window: e.g., 22:00 .. 06:00 -> cur >= start OR cur <= end
                _result = afterStart || beforeEnd;

            _hasResult = true;
            Continue(conversation, events);
        }
    }
}

