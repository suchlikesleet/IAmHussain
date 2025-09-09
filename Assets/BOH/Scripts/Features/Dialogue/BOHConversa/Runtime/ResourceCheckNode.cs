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
    public class ResourceCheckNode : HybridNode
    {
        public enum ResourceType { Money, Energy, Blessings }
        public enum Comparator { GreaterOrEqual, LessOrEqual, Equal }

        [ConversationProperty("Type",  200, 160, 120)] [SerializeField] private ResourceType type = ResourceType.Money;
        [ConversationProperty("Cmp",   200, 160, 120)] [SerializeField] private Comparator comparator = Comparator.GreaterOrEqual;
        [ConversationProperty("Amount",200, 160, 120)] [SerializeField] private int amount = 0;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var resources = GameServices.Resources ?? UnityEngine.Object.FindFirstObjectByType<ResourceSystem>();
            if (resources == null)
            {
                Debug.LogWarning("[ResourceCheckNode] ResourceSystem not found.");
                _result = false; _hasResult = true; Continue(conversation, events); return;
            }

            int current = type switch
            {
                ResourceType.Money => resources.GetMoney(),
                ResourceType.Energy => resources.GetEnergy(),
                _ => resources.GetBlessings()
            };

            _result = comparator switch
            {
                Comparator.GreaterOrEqual => current >= amount,
                Comparator.LessOrEqual => current <= amount,
                Comparator.Equal => current == amount,
                _ => false
            };
            _hasResult = true;
            Continue(conversation, events);
        }
    }
}

