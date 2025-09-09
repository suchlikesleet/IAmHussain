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
    public class ResourceChangeNode : HybridNode
    {
        [ConversationProperty("Money Δ",     140, 200, 140)] [SerializeField] private int moneyDelta = 0;
        [ConversationProperty("Energy Δ",    140, 200, 140)] [SerializeField] private int energyDelta = 0;
        [ConversationProperty("Blessings Δ", 140, 200, 140)] [SerializeField] private int blessingsDelta = 0;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var res = GameServices.Resources ?? UnityEngine.Object.FindFirstObjectByType<ResourceSystem>();
            if (res == null)
            {
                Debug.LogWarning("[ResourceChangeNode] ResourceSystem not found.");
                _result = false; _hasResult = true; Continue(conversation, events); return;
            }

            if (moneyDelta != 0)
            {
                if (moneyDelta > 0) res.AddMoney(moneyDelta); else res.SpendMoney(-moneyDelta);
            }
            if (energyDelta != 0)
            {
                if (energyDelta > 0) res.AddEnergy(energyDelta); else res.SpendEnergy(-energyDelta);
            }
            if (blessingsDelta != 0)
            {
                if (blessingsDelta > 0) res.AddBlessings(blessingsDelta); else res.RemoveBlessings(-blessingsDelta);
            }

            _result = true;
            _hasResult = true;
            Continue(conversation, events);
        }
    }
}

