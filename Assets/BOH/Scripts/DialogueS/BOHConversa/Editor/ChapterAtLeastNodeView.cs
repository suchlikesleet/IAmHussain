// Assets/BOH/Scripts/DialogueS/BOHConversa/Editor/ChapterAtLeastNodeView.cs
using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class ChapterAtLeastNodeView : BaseNodeView<ChapterAtLeastNode>
    {
        protected override string Title => "Chapter ≥ Min?";

        public ChapterAtLeastNodeView(Conversation conversation) : base(new ChapterAtLeastNode(), conversation) { }
        public ChapterAtLeastNodeView(ChapterAtLeastNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var minFI = typeof(ChapterAtLeastNode).GetField("minChapter", BindingFlags.NonPublic | BindingFlags.Instance);

            var min = new IntegerField("Min Chapter");
            min.SetValueWithoutNotify((int)(minFI?.GetValue(Data) ?? 0));
            min.RegisterValueChangedCallback(e => minFI?.SetValue(Data, e.newValue));

            var box = new VisualElement(); box.AddToClassList("p-5");
            box.Add(min);
            bodyContainer.Add(box);
        }
    }
}