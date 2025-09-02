
using BOH.Conversa;
using Conversa.Editor;
using Conversa.Runtime;

namespace BOH
{
    public static class ErrandNodesMenuModifier
    {
        [NodeMenuModifier]
        private static void ModifyMenu(NodeMenuTree tree, Conversation conversation)
        {
            // Order indices: group errands together
            tree.AddGroup("My Errands",1);
            tree.AddMenuEntry<AcceptErrandNodeView>("Accept Errand",               2);
            tree.AddMenuEntry<AttemptCompleteErrandNodeView>("Attempt Complete",   2);
            tree.AddMenuEntry<HasActiveErrandNodeView>("Has Active?",              2);
            tree.AddMenuEntry<IsErrandCompletedNodeView>("Is Completed?",          2);
            tree.AddMenuEntry<ErrandWindowOpenNodeView>("Window Open?",            2);
        }
    }
}