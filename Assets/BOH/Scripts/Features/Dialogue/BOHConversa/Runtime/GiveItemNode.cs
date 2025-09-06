using System;
using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;
using BOH;

namespace BOH.Conversa
{
    [Serializable]
    [Port("Previous","previous",typeof(BaseNode),Flow.In,Capacity.Many)]
    [Port("Next","next",typeof(BaseNode),Flow.Out,Capacity.One)]
    [Port("Result","value",typeof(bool),Flow.Out,Capacity.One)]
    public class GiveItemNode : HybridNode
    {
        [ConversationProperty("Item", 0.25f, 0.6f, 0.95f)]
        [SerializeField] private ItemSO item;

        [ConversationProperty("Count", 120, 200, 120)]
        [SerializeField] private int count = 1;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var inv = GameServices.Inventory ?? FindInventory();
            var qty = Mathf.Max(1, count);

            if (inv != null && item != null)
            {
                inv.AddItem(item, qty);
                _result = true;
            }
            else
            {
                Debug.LogWarning("[GiveItemNode] InventorySystem or Item missing");
                _result = false;
            }

            _hasResult = true;
            Continue(conversation, events);
        }
    }
}
