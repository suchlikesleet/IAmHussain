using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class AcceptErrandNodeView : BaseNodeView<AcceptErrandNode>
    {
        protected override string Title => "Accept Errand";

        public AcceptErrandNodeView(Conversation conversation) : base(new AcceptErrandNode(), conversation) { }
        public AcceptErrandNodeView(AcceptErrandNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var fieldInfo = typeof(AcceptErrandNode).GetField("errand", BindingFlags.NonPublic | BindingFlags.Instance);

            var errandField = new ObjectField("Errand") { objectType = typeof(ErrandSO) };
            errandField.SetValueWithoutNotify(fieldInfo?.GetValue(Data) as ErrandSO);
            errandField.RegisterValueChangedCallback(e =>
            {
                fieldInfo?.SetValue(Data, e.newValue as ErrandSO);
            });

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(errandField);

            bodyContainer.Add(wrapper);
        }
    }
}