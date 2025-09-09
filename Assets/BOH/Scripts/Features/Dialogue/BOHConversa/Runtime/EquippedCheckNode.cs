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
    public class EquippedCheckNode : HybridNode
    {
        public enum Mode { ByItemId, ByTag }

        [ConversationProperty("Mode",  160, 200, 180)] [SerializeField] private Mode mode = Mode.ByTag;
        [ConversationProperty("Item Id", 160, 200, 180)] [SerializeField] private string itemId;
        [ConversationProperty("Equip Tag", 160, 200, 180)] [SerializeField] private string equipTag;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            var inv = FindInventory();
            _result = false;

            if (inv != null)
            {
                var equipped = inv.GetEquippedItem();
                if (equipped != null && equipped.itemData != null)
                {
                    if (mode == Mode.ByItemId)
                        _result = !string.IsNullOrEmpty(itemId) && equipped.itemData.itemId == itemId;
                    else
                        _result = !string.IsNullOrEmpty(equipTag) && (equipped.itemData.equipTag == equipTag);
                }
            }

            _hasResult = true;
            Continue(conversation, events);
        }
    }
}

