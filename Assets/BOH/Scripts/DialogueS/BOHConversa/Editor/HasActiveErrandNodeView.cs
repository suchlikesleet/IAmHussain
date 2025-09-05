// Assets/BOH/Scripts/DialogueS/BOHConversa/Editor/HasActiveErrandNodeView.cs
using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class HasActiveErrandNodeView : BaseNodeView<HasActiveErrandNode>
    {
        protected override string Title => "Has Active Errand?";

        public HasActiveErrandNodeView(Conversation conversation) : base(new HasActiveErrandNode(), conversation) { }
        public HasActiveErrandNodeView(HasActiveErrandNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var idFI  = typeof(HasActiveErrandNode).GetField("errandId", BindingFlags.NonPublic | BindingFlags.Instance);
            var soFI  = typeof(HasActiveErrandNode).GetField("errand",   BindingFlags.NonPublic | BindingFlags.Instance);

            var id = new TextField("Errand Id");
            id.SetValueWithoutNotify(idFI?.GetValue(Data) as string ?? string.Empty);
            id.RegisterValueChangedCallback(e => idFI?.SetValue(Data, e.newValue));

            var so = new ObjectField("Errand (optional)") { objectType = typeof(ErrandSO) };
            so.SetValueWithoutNotify(soFI?.GetValue(Data) as ErrandSO);
            so.RegisterValueChangedCallback(e => soFI?.SetValue(Data, e.newValue as ErrandSO));

            var box = new VisualElement(); box.AddToClassList("p-5");
            box.Add(id); box.Add(so);
            bodyContainer.Add(box);
        }
    }
}