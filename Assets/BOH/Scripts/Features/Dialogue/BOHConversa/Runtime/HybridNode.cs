// Assets/BOH/Scripts/DialogueS/BOHConversa/Runtime/HybridNodes.cs
using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Interfaces;
using Conversa.Runtime.Nodes;
using UnityEngine;

namespace BOH.Conversa
{
    // Base helper with common pattern (Next + bool Result)
    public abstract class HybridNode : BaseNode, IEventNode, IValueNode
    {
        protected bool _result;
        protected bool _hasResult;

        protected void Continue(Conversation conversation, ConversationEvents events)
        {
            var nextNodes = conversation.GetOppositeNodes(GetNodePort("next")).ToList();
            if (nextNodes.Count > 0)
                conversation.Process(nextNodes[0], events);
        }

        public abstract void Process(Conversation conversation, ConversationEvents conversationEvents);

        // IMPORTANT: no Converter usage here — just return the bool as T.
        public T GetValue<T>(string portGuid, Conversation conversation)
        {
            var value = (_hasResult && portGuid == "value") ? _result : false;

            // The 'Result' port is declared as bool, so casts are safe.
            // The Conversa graph's own converter handles any mismatched links.
            return (T)(object)value;
        }

        // Fully qualify UnityEngine.Object to avoid ambiguity.
        protected static ErrandSystem   FindErrands()   => UnityEngine.Object.FindFirstObjectByType<ErrandSystem>();
        protected static InventorySystem FindInventory() => UnityEngine.Object.FindFirstObjectByType<InventorySystem>();
        protected static ContactSystem   FindContacts()  => UnityEngine.Object.FindFirstObjectByType<ContactSystem>();
    }
}