using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class AdvanceChapterNodeView : BaseNodeView<AdvanceChapterNode>
    {
        protected override string Title => "Advance Chapter";

        public AdvanceChapterNodeView(Conversation conversation) : base(new AdvanceChapterNode(), conversation) {}
        public AdvanceChapterNodeView(AdvanceChapterNode data, Conversation conversation) : base(data, conversation) {}

        protected override void SetBody()
        {
            var fDelta = typeof(AdvanceChapterNode).GetField("delta", BindingFlags.NonPublic|BindingFlags.Instance);

            var delta = new IntegerField("Delta");
            delta.SetValueWithoutNotify((int)(fDelta?.GetValue(Data) ?? 1));
            delta.RegisterValueChangedCallback(e => fDelta?.SetValue(Data, e.newValue));

            var box = new VisualElement(); box.AddToClassList("p-5");
            box.Add(delta);
            bodyContainer.Add(box);
        }
    }
}