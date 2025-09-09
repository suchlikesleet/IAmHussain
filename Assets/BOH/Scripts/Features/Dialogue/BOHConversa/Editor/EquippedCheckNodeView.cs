using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class EquippedCheckNodeView : BaseNodeView<EquippedCheckNode>
    {
        protected override string Title => "Equipped Check";

        public EquippedCheckNodeView(Conversation conversation) : base(new EquippedCheckNode(), conversation) { }
        public EquippedCheckNodeView(EquippedCheckNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var t = typeof(EquippedCheckNode);
            var modeInfo = t.GetField("mode",     BindingFlags.NonPublic | BindingFlags.Instance);
            var idInfo   = t.GetField("itemId",   BindingFlags.NonPublic | BindingFlags.Instance);
            var tagInfo  = t.GetField("equipTag", BindingFlags.NonPublic | BindingFlags.Instance);

            var modeField = new EnumField("Mode");
            modeField.Init((EquippedCheckNode.Mode)(modeInfo?.GetValue(Data) ?? EquippedCheckNode.Mode.ByTag));
            modeField.RegisterValueChangedCallback(e => modeInfo?.SetValue(Data, e.newValue));

            var idField = new TextField("Item Id");
            idField.SetValueWithoutNotify((string)(idInfo?.GetValue(Data) ?? ""));
            idField.RegisterValueChangedCallback(e => idInfo?.SetValue(Data, e.newValue));

            var tagField = new TextField("Equip Tag");
            tagField.SetValueWithoutNotify((string)(tagInfo?.GetValue(Data) ?? ""));
            tagField.RegisterValueChangedCallback(e => tagInfo?.SetValue(Data, e.newValue));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(modeField);
            wrapper.Add(idField);
            wrapper.Add(tagField);

            bodyContainer.Add(wrapper);
        }
    }
}

