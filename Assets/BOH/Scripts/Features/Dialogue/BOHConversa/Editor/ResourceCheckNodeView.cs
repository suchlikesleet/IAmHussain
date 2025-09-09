using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class ResourceCheckNodeView : BaseNodeView<ResourceCheckNode>
    {
        protected override string Title => "Resource Check";

        public ResourceCheckNodeView(Conversation conversation) : base(new ResourceCheckNode(), conversation) { }
        public ResourceCheckNodeView(ResourceCheckNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var t = typeof(ResourceCheckNode);
            var typeInfo  = t.GetField("type",       BindingFlags.NonPublic | BindingFlags.Instance);
            var cmpInfo   = t.GetField("comparator", BindingFlags.NonPublic | BindingFlags.Instance);
            var amtInfo   = t.GetField("amount",     BindingFlags.NonPublic | BindingFlags.Instance);

            var typeField = new EnumField("Type");
            typeField.Init((ResourceCheckNode.ResourceType)(typeInfo?.GetValue(Data) ?? ResourceCheckNode.ResourceType.Money));
            typeField.RegisterValueChangedCallback(e => typeInfo?.SetValue(Data, e.newValue));

            var cmpField = new EnumField("Comparator");
            cmpField.Init((ResourceCheckNode.Comparator)(cmpInfo?.GetValue(Data) ?? ResourceCheckNode.Comparator.GreaterOrEqual));
            cmpField.RegisterValueChangedCallback(e => cmpInfo?.SetValue(Data, e.newValue));

            var amountField = new IntegerField("Amount");
            amountField.SetValueWithoutNotify((int)(amtInfo?.GetValue(Data) ?? 0));
            amountField.RegisterValueChangedCallback(e => amtInfo?.SetValue(Data, Mathf.Max(0, e.newValue)));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(typeField);
            wrapper.Add(cmpField);
            wrapper.Add(amountField);

            bodyContainer.Add(wrapper);
        }
    }
}

