using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH
{
    public sealed class HasActiveErrandNodeView : BaseNodeView<HasActiveErrandNode>
    {
        protected override string Title => "Has Active Errand?";

        public HasActiveErrandNodeView(Conversation conversation) : base(new HasActiveErrandNode(), conversation) { }
        public HasActiveErrandNodeView(HasActiveErrandNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            /*var f = new ObjectField("Errand") { objectType = typeof(ErrandSO), allowSceneObjects = false };
            f.value = ErrandNodeViewUtil.GetPrivate<HasActiveErrandNode, ErrandSO>(Data, "errand");
            f.RegisterValueChangedCallback(evt =>
            {
                SetUndo("Set Errand");
                ErrandNodeViewUtil.SetPrivate(Data, "errand", evt.newValue);
                Save();
            });
            Body.Add(f);*/
        }
    }
}