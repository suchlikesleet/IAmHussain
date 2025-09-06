using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class ErrandCompleteNodeView : BaseNodeView<ErrandCompleteNode>
    {
        protected override string Title => "Complete Errand";

        public ErrandCompleteNodeView(Conversation conversation) : base(new ErrandCompleteNode(), conversation) { }
        public ErrandCompleteNodeView(ErrandCompleteNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var fieldInfo = typeof(ErrandCompleteNode).GetField("errand", BindingFlags.NonPublic | BindingFlags.Instance);

            var errandField = new ObjectField("Errand") { objectType = typeof(ErrandSO) };
            errandField.SetValueWithoutNotify(fieldInfo?.GetValue(Data) as ErrandSO);
            errandField.RegisterValueChangedCallback(e => fieldInfo?.SetValue(Data, e.newValue as ErrandSO));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(errandField);

            bodyContainer.Add(wrapper);
        }
    }
}