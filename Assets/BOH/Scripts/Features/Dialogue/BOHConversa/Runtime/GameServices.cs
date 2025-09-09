// Assets/BOH/Scripts/DialogueS/BOHConversa/Runtime/GameServices.cs
namespace BOH
{
    // Set these from a bootstrapper/Installer in your scene (optional).
    public static class GameServices
    {
        public static ErrandSystem Errands;
        public static InventorySystem Inventory;
        public static ContactSystem Contacts;
        public static IFlagService Flags;
        public static IStoryService Story;
        public static ResourceSystem Resources;
        public static GiftingSystem Gifting;
        public static JournalSystem Journal;
        public static TimeSystem Time;
    }
}
