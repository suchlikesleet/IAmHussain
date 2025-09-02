using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public sealed class AcceptErrandNodeView : BaseNodeView<AcceptErrandNode>
    {
        protected override string Title => "Accept Errand";

        public AcceptErrandNodeView(Conversation conversation) : base(new AcceptErrandNode(), conversation) { }
        public AcceptErrandNodeView(AcceptErrandNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            /*var f = new ObjectField("Errand") { objectType = typeof(ErrandSO), allowSceneObjects = false };
            var fi = typeof(AcceptErrandNode).GetField("errand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f.value = (ErrandSO)fi.GetValue(Data);
            f.RegisterValueChangedCallback(evt =>
            {
                SetUndo("Set Errand");
                fi.SetValue(Data, evt.newValue);
                Save();
            });
            Body.Add(f);*/
        }
    }
}