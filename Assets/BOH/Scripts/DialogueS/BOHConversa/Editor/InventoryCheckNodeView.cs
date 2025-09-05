using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class InventoryCheckNodeView : BaseNodeView<InventoryCheckNode>
    {
        protected override string Title => "Inventory Check";

        public InventoryCheckNodeView(Conversation conversation) : base(new InventoryCheckNode(), conversation) { }
        public InventoryCheckNodeView(InventoryCheckNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var itemFieldInfo  = typeof(InventoryCheckNode).GetField("item",  BindingFlags.NonPublic | BindingFlags.Instance);
            var countFieldInfo = typeof(InventoryCheckNode).GetField("count", BindingFlags.NonPublic | BindingFlags.Instance);

            var itemField = new ObjectField("Item") { objectType = typeof(ItemSO) };
            itemField.SetValueWithoutNotify(itemFieldInfo?.GetValue(Data) as ItemSO);
            itemField.RegisterValueChangedCallback(e => itemFieldInfo?.SetValue(Data, e.newValue as ItemSO));

            var countField = new IntegerField("Count");
            countField.SetValueWithoutNotify((int)(countFieldInfo?.GetValue(Data) ?? 1));
            countField.RegisterValueChangedCallback(e => countFieldInfo?.SetValue(Data, Mathf.Max(1, e.newValue)));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(itemField);
            wrapper.Add(countField);

            bodyContainer.Add(wrapper);
        }
    }
}