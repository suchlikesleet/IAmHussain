using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class TrustChangeNodeView : BaseNodeView<TrustChangeNode>
    {
        protected override string Title => "Trust Change";

        public TrustChangeNodeView(Conversation conversation) : base(new TrustChangeNode(), conversation) { }
        public TrustChangeNodeView(TrustChangeNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var idFieldInfo    = typeof(TrustChangeNode).GetField("contactId", BindingFlags.NonPublic | BindingFlags.Instance);
            var deltaFieldInfo = typeof(TrustChangeNode).GetField("delta",     BindingFlags.NonPublic | BindingFlags.Instance);

            var idField = new TextField("Contact Id");
            idField.SetValueWithoutNotify(idFieldInfo?.GetValue(Data) as string ?? string.Empty);
            idField.RegisterValueChangedCallback(e => idFieldInfo?.SetValue(Data, e.newValue));

            var deltaField = new IntegerField("Delta");
            deltaField.SetValueWithoutNotify((int)(deltaFieldInfo?.GetValue(Data) ?? 1));
            deltaField.RegisterValueChangedCallback(e => deltaFieldInfo?.SetValue(Data, e.newValue));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(idField);
            wrapper.Add(deltaField);

            bodyContainer.Add(wrapper);
        }
    }
}