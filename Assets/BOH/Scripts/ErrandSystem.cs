using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Obvious.Soap;

namespace BOH
{
    public class ErrandSystem : MonoBehaviour
    {
        public enum ErrandStatus
        {
            Active,
            Completed,
            Late,
            Partial,
            Failed
        }

        [System.Serializable]
        public class ActiveErrand
        {
            public ErrandSO errandData;
            public ErrandStatus status = ErrandStatus.Active;
            public int acceptedDay;
            public float acceptedTime;
            public int itemsDelivered = 0;
        }

        [Header("Events")]
        [SerializeField] private ScriptableEventNoParam onDayEnd;
        [SerializeField] private ScriptableEventNoParam onErrandCompleted;
        
        [Header("Active Errands")]
        [SerializeField] private List<ActiveErrand> activeErrands = new List<ActiveErrand>();
        [SerializeField] private List<string> completedErrandIds = new List<string>();
        
        private InventorySystem inventorySystem;
        private ResourceSystem resourceSystem;
        private TimeSystem timeSystem;
        private int currentDay = 1;

        private void OnEnable()
        {
            if (onDayEnd != null) onDayEnd.OnRaised+= CheckDayEndErrands;
        }

        private void OnDisable()
        {
            if (onDayEnd != null) onDayEnd.OnRaised-= CheckDayEndErrands;
        }

        private void Start()
        {
            inventorySystem = FindFirstObjectByType<InventorySystem>();
            resourceSystem = FindFirstObjectByType<ResourceSystem>();
            timeSystem = FindFirstObjectByType<TimeSystem>();
            
            Debug.Log("ErrandSystem initialized");
        }

        public void AddErrand(ErrandSO errand)
        {
            if (errand == null) return;
            
            var active = new ActiveErrand
            {
                errandData = errand,
                status = ErrandStatus.Active,
                acceptedDay = currentDay,
                acceptedTime = timeSystem != null ? timeSystem.GetTotalMinutes() : 0
            };
            
            activeErrands.Add(active);
            Debug.Log($"Errand added: {errand.errandTitle}");
        }

        public bool TryCompleteErrand(string errandId)
        {
            var errand = activeErrands.FirstOrDefault(e => 
                e.errandData.errandId == errandId && 
                e.status == ErrandStatus.Active);
            
            if (errand == null)
            {
                Debug.Log($"Errand not found or not active: {errandId}");
                return false;
            }

            // Check requirements
            if (!CheckRequirements(errand.errandData))
            {
                Debug.Log($"Requirements not met for errand: {errandId}");
                return false;
            }

            // Consume required items
            ConsumeRequirements(errand.errandData);
            
            // Check if late
            bool isLate = IsErrandLate(errand);
            errand.status = isLate ? ErrandStatus.Late : ErrandStatus.Completed;
            
            // Give rewards
            GiveRewards(errand.errandData, isLate);
            
            // Move to completed
            completedErrandIds.Add(errandId);
            activeErrands.Remove(errand);
            
            Debug.Log($"Errand completed: {errandId} (Late: {isLate})");
            onErrandCompleted?.Raise();
            
            // Check for follow-up
            if (errand.errandData.followUpErrand != null)
            {
                AddErrand(errand.errandData.followUpErrand);
            }
            
            return true;
        }

        private bool CheckRequirements(ErrandSO errand)
        {
            // Check items
            foreach (var req in errand.itemsRequired)
            {
                if (!inventorySystem.HasItem(req.item.itemId, req.count))
                    return false;
            }
            
            // Check energy
            if (resourceSystem != null && resourceSystem.GetEnergy() < errand.energyCost)
                return false;
            
            return true;
        }

        private void ConsumeRequirements(ErrandSO errand)
        {
            // Consume items
            foreach (var req in errand.itemsRequired)
            {
                inventorySystem.ConsumeItem(req.item.itemId, req.count);
            }
            
            // Consume energy
            if (errand.energyCost > 0 && resourceSystem != null)
            {
                resourceSystem.SpendEnergy(errand.energyCost);
            }
        }

        private void GiveRewards(ErrandSO errand, bool isLate)
        {
            // Reduce rewards if late
            int blessingAmount = isLate ? errand.blessingsReward / 2 : errand.blessingsReward;
            
            if (resourceSystem != null)
            {
                if (blessingAmount > 0)
                    resourceSystem.AddBlessings(blessingAmount);
                
                if (errand.moneyReward > 0)
                    resourceSystem.AddMoney(errand.moneyReward);
            }
            
            // Give item rewards
            foreach (var reward in errand.itemRewards)
            {
                inventorySystem.AddItem(reward.item, reward.count);
            }
        }

        private bool IsErrandLate(ActiveErrand errand)
        {
            if (errand.errandData.type != ErrandSO.ErrandType.Strict)
                return false;
            
            string currentTime = timeSystem.GetTimeString();
            int hour = int.Parse(currentTime.Substring(0, 2));
            
            return hour >= errand.errandData.endHour;
        }

        private void CheckDayEndErrands()
        {
            currentDay++;
            
            // Check for expired errands
            var expired = activeErrands.Where(e => 
                e.errandData.type == ErrandSO.ErrandType.Strict &&
                e.acceptedDay < currentDay).ToList();
            
            foreach (var errand in expired)
            {
                errand.status = ErrandStatus.Failed;
                Debug.Log($"Errand failed: {errand.errandData.errandTitle}");
            }
            
            // Remove failed errands
            activeErrands.RemoveAll(e => e.status == ErrandStatus.Failed);
        }

        public bool IsErrandCompleted(string errandId)
        {
            return completedErrandIds.Contains(errandId);
        }

        public List<ActiveErrand> GetActiveErrands()
        {
            return activeErrands.Where(e => e.status == ErrandStatus.Active).ToList();
        }
    }

    // ScriptRole: Manages errand lifecycle and completion
    // RelatedScripts: TriggerSystem, InventorySystem, ResourceSystem
    // UsesSO: ErrandSO, GameEventSO
    // ReceivesFrom: TriggerSystem, UI interactions
    // SendsTo: InventorySystem, ResourceSystem, Journal
}