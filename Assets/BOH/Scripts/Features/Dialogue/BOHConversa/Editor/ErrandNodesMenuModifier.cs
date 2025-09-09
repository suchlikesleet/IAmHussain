using Conversa.Editor;
using Conversa.Runtime;

namespace BOH.Conversa
{
    public class ErrandNodesMenuModifier
    {
        [NodeMenuModifier]
        private static void ModifyMenu(NodeMenuTree tree, Conversation conversation)
        {
            tree.AddGroup("Errands & Inventory");

            // Errands
            tree.AddMenuEntry<HasActiveErrandNodeView>("Has Active Errand?", 2);
            tree.AddMenuEntry<AcceptErrandNodeView>("Accept Errand", 2);
            tree.AddMenuEntry<ErrandCheckNodeView>("Errand Check", 2);
            tree.AddMenuEntry<ErrandCompleteNodeView>("Complete Errand", 2);

            // Inventory
            tree.AddMenuEntry<InventoryCheckNodeView>("Inventory Check", 2);
            tree.AddMenuEntry<InventoryConsumeNodeView>("Inventory Consume", 2);
            tree.AddMenuEntry<GiveItemNodeView>("Give Item", 2);

            // Contacts / Trust
            tree.AddMenuEntry<TrustChangeNodeView>("Trust Change", 2);
            
            
            tree.AddGroup("Story Gates");
            tree.AddMenuEntry<HasFlagNodeView>("Has Flag?", 2);
            tree.AddMenuEntry<TrustAtLeastNodeView>("Trust ≥ Min?", 2);
            tree.AddMenuEntry<ChapterAtLeastNodeView>("Chapter ≥ Min?", 2);
            tree.AddMenuEntry<SetFlagNodeView>("Set Flag", 2);
            tree.AddMenuEntry<AdvanceChapterNodeView>("Advance Chapter", 2);

            // Time
            tree.AddMenuEntry<TimeBeforeNodeView>("Time Before", 2);

            // Avatar
            tree.AddGroup("Avatar");
            tree.AddMenuEntry<AvatarMessageNodeView>("Avatar Message", 2);
            tree.AddMenuEntry<AvatarPropertyNodeView>("Avatar Property", 2);
            tree.AddMenuEntry<AvatarChoiceNodeView>("Avatar Choice", 2);
        }
    }
}
