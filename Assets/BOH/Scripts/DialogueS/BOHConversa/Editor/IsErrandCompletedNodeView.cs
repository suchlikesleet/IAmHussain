using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH
{
    public sealed class IsErrandCompletedNodeView : BaseNodeView<IsErrandCompletedNode>
    {
        protected override string Title => "Is Errand Completed?";

        public IsErrandCompletedNodeView(Conversation conversation) : base(new IsErrandCompletedNode(), conversation) { }
        public IsErrandCompletedNodeView(IsErrandCompletedNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
           
        }
    }
}