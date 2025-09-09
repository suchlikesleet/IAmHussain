// Deducts money and grants an item to the player inventory; optional flag set.
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
    public class PurchaseItemNode : HybridNode
    {
        [ConversationProperty("Item",    64, 153, 242)] [SerializeField] private ItemSO item;
        [ConversationProperty("Count",   64, 153, 242)] [SerializeField] private int count = 1;
        [ConversationProperty("Price",   64, 153, 242)] [SerializeField] private int price = 0;
        [ConversationProperty("Set Flag (optional)", 190,120,255)] [SerializeField] private string flagToSet;

        public override void Process(Conversation conversation, ConversationEvents events)
        {
            _result = false;

            var inv = GameServices.Inventory ?? UnityEngine.Object.FindFirstObjectByType<InventorySystem>();
            var res = GameServices.Resources ?? UnityEngine.Object.FindFirstObjectByType<ResourceSystem>();

            if (item == null || inv == null)
            {
                Debug.LogWarning("[PurchaseItemNode] Missing item or InventorySystem");
                Finish(events, false);
                return;
            }

            int qty = Mathf.Max(1, count);
            int cost = Mathf.Max(0, price);

            if (cost > 0)
            {
                if (res == null)
                {
                    Debug.LogWarning("[PurchaseItemNode] Missing ResourceSystem for priced purchase");
                    Finish(events, false);
                    return;
                }

                if (!res.SpendMoney(cost))
                {
                    // Not enough funds
                    Finish(events, false);
                    return;
                }
            }

            inv.AddItem(item, qty);

            if (!string.IsNullOrEmpty(flagToSet) && GameServices.Flags is FlagService flags)
            {
                flags.SetFlag(flagToSet, true);
            }

            Finish(events, true);
        }

        private void Finish(ConversationEvents events, bool ok)
        {
            _result = ok;
            _hasResult = true;
            Continue(null, events);
        }
    }
}

