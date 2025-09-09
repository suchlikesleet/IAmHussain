using UnityEngine;

namespace BOH
{
    public class GameServicesInstaller : MonoBehaviour
    {
        [Header("Core Systems (assign or auto-find)")]
        [SerializeField] private ErrandSystem errandSystem;
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private ContactSystem contactSystem;
        [SerializeField] private ResourceSystem resourceSystem;
        [SerializeField] private TimeSystem timeSystem;

        [Header("Optional Services (assign if used)")]
        [SerializeField] private FlagService  flagService;   // implements IFlagService
        [SerializeField] private StoryService storyService;  // implements IStoryService
        [SerializeField] private GiftingSystem giftingSystem; // optional
        [SerializeField] private JournalSystem journalSystem; // optional

        [SerializeField] private bool dontDestroyOnLoad = true;

        private void Awake()
        {
            // Core systems: use assigned refs or auto-find
            GameServices.Errands   = errandSystem   ? errandSystem   : FindFirstObjectByType<ErrandSystem>();
            GameServices.Inventory = inventorySystem? inventorySystem: FindFirstObjectByType<InventorySystem>();
            GameServices.Contacts  = contactSystem  ? contactSystem  : FindFirstObjectByType<ContactSystem>();
            GameServices.Resources = resourceSystem ? resourceSystem : FindFirstObjectByType<ResourceSystem>();
            GameServices.Time      = timeSystem     ? timeSystem     : FindFirstObjectByType<TimeSystem>();

            // Optional services
            GameServices.Flags = flagService;
            GameServices.Story = storyService;
            GameServices.Gifting = giftingSystem ? giftingSystem : FindFirstObjectByType<GiftingSystem>();
            GameServices.Journal = journalSystem ? journalSystem : FindFirstObjectByType<JournalSystem>();

            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

            ValidateAssignments();
        }

        [ContextMenu("Validate Assignments")]
        private void ValidateAssignments()
        {
            if (GameServices.Errands == null)
                Debug.LogWarning("[GameServicesInstaller] ErrandSystem not found/assigned.");
            if (GameServices.Inventory == null)
                Debug.LogWarning("[GameServicesInstaller] InventorySystem not found/assigned.");
            if (GameServices.Contacts == null)
                Debug.LogWarning("[GameServicesInstaller] ContactSystem not found/assigned.");
            if (GameServices.Resources == null)
                Debug.LogWarning("[GameServicesInstaller] ResourceSystem not found/assigned.");
            if (GameServices.Time == null)
                Debug.LogWarning("[GameServicesInstaller] TimeSystem not found/assigned.");

            if (GameServices.Flags == null)
                Debug.Log("[GameServicesInstaller] IFlagService is optional and not set.");
            if (GameServices.Story == null)
                Debug.Log("[GameServicesInstaller] IStoryService is optional and not set.");

            if (GameServices.Gifting == null)
                Debug.Log("[GameServicesInstaller] GiftingSystem is optional and not set.");
            if (GameServices.Journal == null)
                Debug.Log("[GameServicesInstaller] JournalSystem is optional and not set.");
        }

        private void OnValidate()
        {
            // Keep the scene hints up to date in the editor without play mode
            if (!Application.isPlaying)
            {
                if (errandSystem == null) errandSystem = FindFirstObjectByType<ErrandSystem>();
                if (inventorySystem == null) inventorySystem = FindFirstObjectByType<InventorySystem>();
                if (contactSystem == null) contactSystem = FindFirstObjectByType<ContactSystem>();
                if (resourceSystem == null) resourceSystem = FindFirstObjectByType<ResourceSystem>();
                if (timeSystem == null) timeSystem = FindFirstObjectByType<TimeSystem>();

                if (flagService == null) flagService = FindFirstObjectByType<FlagService>();
                if (storyService == null) storyService = FindFirstObjectByType<StoryService>();
                if (giftingSystem == null) giftingSystem = FindFirstObjectByType<GiftingSystem>();
                if (journalSystem == null) journalSystem = FindFirstObjectByType<JournalSystem>();
            }
        }
    }
}
