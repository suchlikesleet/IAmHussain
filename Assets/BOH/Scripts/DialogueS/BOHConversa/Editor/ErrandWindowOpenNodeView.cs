using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH
{
    public sealed class ErrandWindowOpenNodeView : BaseNodeView<ErrandWindowOpenNode>
    {
        protected override string Title => "Errand Window Open?";

        public ErrandWindowOpenNodeView(Conversation conversation) : base(new ErrandWindowOpenNode(), conversation) { }
        public ErrandWindowOpenNodeView(ErrandWindowOpenNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            /*var f = new ObjectField("Errand") { objectType = typeof(ErrandSO), allowSceneObjects = false };
            f.value = ErrandNodeViewUtil.GetPrivate<ErrandWindowOpenNode, ErrandSO>(Data, "errand");
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