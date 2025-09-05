using System;
using Conversa.Runtime;
using Conversa.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using BOH;
using Conversa.Runtime.Attributes;

namespace BOH.Conversa
{
    [MovedFrom(true, null, "Assembly-CSharp")]
    [Serializable]
    [Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
    [Port("Next", "next", typeof(BaseNode), Flow.Out, Capacity.One)]
    [Port("Result", "value", typeof(bool), Flow.Out, Capacity.One)]
    public class InventoryConsumeNode : HybridNode
    {
        [ConversationProperty("Item", 0.3f, 0.8f, 0.4f)]
        [SerializeField] private ItemSO item;

        [ConversationProperty("Count", 0.3f, 0.8f, 0.4f)]
        [SerializeField] private int count = 1;

        public override void Process(Conversation conversation, ConversationEvents conversationEvents)
        {
            var inv = FindInventory();
            _result = false;

            if (inv != null && item != null && inv.HasItem(item.itemId, Mathf.Max(1, count)))
            {
                inv.ConsumeItem(item.itemId, Mathf.Max(1, count));
                _result = true;
            }

            _hasResult = true;
            Continue(conversation, conversationEvents);
        }
    }
}