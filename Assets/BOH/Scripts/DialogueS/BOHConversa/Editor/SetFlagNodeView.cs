using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class SetFlagNodeView : BaseNodeView<SetFlagNode>
    {
        protected override string Title => "Set Flag";

        public SetFlagNodeView(Conversation conversation) : base(new SetFlagNode(), conversation) {}
        public SetFlagNodeView(SetFlagNode data, Conversation conversation) : base(data, conversation) {}

        protected override void SetBody()
        {
            var fFlag  = typeof(SetFlagNode).GetField("flag",  BindingFlags.NonPublic|BindingFlags.Instance);
            var fValue = typeof(SetFlagNode).GetField("value", BindingFlags.NonPublic|BindingFlags.Instance);

            var flag = new TextField("Flag");
            flag.SetValueWithoutNotify(fFlag?.GetValue(Data) as string ?? string.Empty);
            flag.RegisterValueChangedCallback(e => fFlag?.SetValue(Data, e.newValue));

            var val = new Toggle("Value");
            val.SetValueWithoutNotify((bool)(fValue?.GetValue(Data) ?? true));
            val.RegisterValueChangedCallback(e => fValue?.SetValue(Data, e.newValue));

            var box = new VisualElement(); box.AddToClassList("p-5");
            box.Add(flag); box.Add(val);
            bodyContainer.Add(box);
        }
    }
}