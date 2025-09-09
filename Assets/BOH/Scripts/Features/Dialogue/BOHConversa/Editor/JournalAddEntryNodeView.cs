using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class JournalAddEntryNodeView : BaseNodeView<JournalAddEntryNode>
    {
        protected override string Title => "Journal Add Entry";

        public JournalAddEntryNodeView(Conversation conversation) : base(new JournalAddEntryNode(), conversation) { }
        public JournalAddEntryNodeView(JournalAddEntryNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var t = typeof(JournalAddEntryNode);
            var typeInfo = t.GetField("entryType", BindingFlags.NonPublic | BindingFlags.Instance);
            var msgInfo  = t.GetField("message",   BindingFlags.NonPublic | BindingFlags.Instance);

            var typeField = new EnumField("Type");
            typeField.Init((JournalSystem.EntryType)(typeInfo?.GetValue(Data) ?? JournalSystem.EntryType.Special));
            typeField.RegisterValueChangedCallback(e => typeInfo?.SetValue(Data, e.newValue));

            var msgField = new TextField("Message") { multiline = true }; 
            msgField.SetValueWithoutNotify((string)(msgInfo?.GetValue(Data) ?? ""));
            msgField.RegisterValueChangedCallback(e => msgInfo?.SetValue(Data, e.newValue));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(typeField);
            wrapper.Add(msgField);

            bodyContainer.Add(wrapper);
        }
    }
}

