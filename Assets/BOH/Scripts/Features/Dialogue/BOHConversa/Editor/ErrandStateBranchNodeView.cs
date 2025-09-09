using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class ErrandStateBranchNodeView : BaseNodeView<ErrandStateBranchNode>
    {
        protected override string Title => "Errand State Branch";

        public ErrandStateBranchNodeView(Conversation conversation) : base(new ErrandStateBranchNode(), conversation) { }
        public ErrandStateBranchNodeView(ErrandStateBranchNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var t = typeof(ErrandStateBranchNode);
            var idInfo = t.GetField("errandId", BindingFlags.NonPublic | BindingFlags.Instance);
            var soInfo = t.GetField("errand",   BindingFlags.NonPublic | BindingFlags.Instance);

            var idField = new TextField("Errand Id");
            idField.SetValueWithoutNotify((string)(idInfo?.GetValue(Data) ?? ""));
            idField.RegisterValueChangedCallback(e => idInfo?.SetValue(Data, e.newValue));

            var soField = new ObjectField("Errand (optional)") { objectType = typeof(ErrandSO) };
            soField.SetValueWithoutNotify((ErrandSO)(soInfo?.GetValue(Data)));
            soField.RegisterValueChangedCallback(e => soInfo?.SetValue(Data, e.newValue as ErrandSO));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(idField);
            wrapper.Add(soField);
            bodyContainer.Add(wrapper);
        }
    }
}

