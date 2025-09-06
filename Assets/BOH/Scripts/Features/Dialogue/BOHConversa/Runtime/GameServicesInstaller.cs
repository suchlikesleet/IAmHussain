using UnityEngine;

namespace BOH
{
    public class GameServicesInstaller : MonoBehaviour
    {
        [Header("Core Systems (assign or auto-find)")]
        [SerializeField] private ErrandSystem errandSystem;
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private ContactSystem contactSystem;

        [Header("Optional Services (assign if used)")]
        [SerializeField] private FlagService  flagService;   // implements IFlagService
        [SerializeField] private StoryService storyService;  // implements IStoryService

        [SerializeField] private bool dontDestroyOnLoad = true;

        private void Awake()
        {
            // Core systems: use assigned refs or auto-find
            GameServices.Errands   = errandSystem   ? errandSystem   : FindFirstObjectByType<ErrandSystem>();
            GameServices.Inventory = inventorySystem? inventorySystem: FindFirstObjectByType<InventorySystem>();
            GameServices.Contacts  = contactSystem  ? contactSystem  : FindFirstObjectByType<ContactSystem>();

            // Optional services
            GameServices.Flags = flagService;
            GameServices.Story = storyService;

            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
    }
}