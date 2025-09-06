// Assets/BOH/Scripts/DialogueS/BOHConversa/Editor/TrustAtLeastNodeView.cs
using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class TrustAtLeastNodeView : BaseNodeView<TrustAtLeastNode>
    {
        protected override string Title => "Trust ≥ Min?";

        public TrustAtLeastNodeView(Conversation conversation) : base(new TrustAtLeastNode(), conversation) { }
        public TrustAtLeastNodeView(TrustAtLeastNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var idFI    = typeof(TrustAtLeastNode).GetField("contactId", BindingFlags.NonPublic | BindingFlags.Instance);
            var minFI   = typeof(TrustAtLeastNode).GetField("minTrust",  BindingFlags.NonPublic | BindingFlags.Instance);

            var id = new TextField("Contact Id");
            id.SetValueWithoutNotify(idFI?.GetValue(Data) as string ?? string.Empty);
            id.RegisterValueChangedCallback(e => idFI?.SetValue(Data, e.newValue));

            var min = new IntegerField("Min Trust");
            min.SetValueWithoutNotify((int)(minFI?.GetValue(Data) ?? 0));
            min.RegisterValueChangedCallback(e => minFI?.SetValue(Data, e.newValue));

            var box = new VisualElement(); box.AddToClassList("p-5");
            box.Add(id); box.Add(min);
            bodyContainer.Add(box);
        }
    }
}