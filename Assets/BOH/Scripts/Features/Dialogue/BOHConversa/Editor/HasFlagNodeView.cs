// Assets/BOH/Scripts/DialogueS/BOHConversa/Editor/HasFlagNodeView.cs
using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class HasFlagNodeView : BaseNodeView<HasFlagNode>
    {
        protected override string Title => "Has Flag?";

        public HasFlagNodeView(Conversation conversation) : base(new HasFlagNode(), conversation) { }
        public HasFlagNodeView(HasFlagNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var flagFI = typeof(HasFlagNode).GetField("flag", BindingFlags.NonPublic | BindingFlags.Instance);

            var tf = new TextField("Flag");
            tf.SetValueWithoutNotify(flagFI?.GetValue(Data) as string ?? string.Empty);
            tf.RegisterValueChangedCallback(e => flagFI?.SetValue(Data, e.newValue));

            var box = new VisualElement(); box.AddToClassList("p-5");
            box.Add(tf);
            bodyContainer.Add(box);
        }
    }
}