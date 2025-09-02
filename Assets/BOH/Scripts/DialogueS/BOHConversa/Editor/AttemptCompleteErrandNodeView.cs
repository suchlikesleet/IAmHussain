using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH
{
    public sealed class AttemptCompleteErrandNodeView : BaseNodeView<AttemptCompleteErrandNode>
    {
        protected override string Title => "Attempt Complete Errand";

        public AttemptCompleteErrandNodeView(Conversation conversation) : base(new AttemptCompleteErrandNode(), conversation) { }
        public AttemptCompleteErrandNodeView(AttemptCompleteErrandNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            
        }
    }
}