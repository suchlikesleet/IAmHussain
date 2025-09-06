using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class ErrandCheckNodeView : BaseNodeView<ErrandCheckNode>
    {
        protected override string Title => "Errand Check";

        public ErrandCheckNodeView(Conversation conversation) : base(new ErrandCheckNode(), conversation) { }
        public ErrandCheckNodeView(ErrandCheckNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var idFieldInfo      = typeof(ErrandCheckNode).GetField("errandId", BindingFlags.NonPublic | BindingFlags.Instance);
            var errandFieldInfo  = typeof(ErrandCheckNode).GetField("errand",   BindingFlags.NonPublic | BindingFlags.Instance);

            var errandId = new TextField("Errand Id");
            errandId.SetValueWithoutNotify(idFieldInfo?.GetValue(Data) as string ?? string.Empty);
            errandId.RegisterValueChangedCallback(e => idFieldInfo?.SetValue(Data, e.newValue));

            var errandObj = new ObjectField("Errand (optional)") { objectType = typeof(ErrandSO) };
            errandObj.SetValueWithoutNotify(errandFieldInfo?.GetValue(Data) as ErrandSO);
            errandObj.RegisterValueChangedCallback(e => errandFieldInfo?.SetValue(Data, e.newValue as ErrandSO));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(errandId);
            wrapper.Add(errandObj);

            bodyContainer.Add(wrapper);
        }
    }
}