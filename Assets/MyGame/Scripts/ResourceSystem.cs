using UnityEngine;
using System;
using Obvious.Soap;

namespace BOH
{
    public class ResourceSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ResourceConfigSO config;
        
        [Header("Events")]
        [SerializeField] private ScriptableEventInt onMoneyChanged;
        [SerializeField] private ScriptableEventInt onEnergyChanged;
        [SerializeField] private ScriptableEventInt onBlessingsChanged;
        [SerializeField] private ScriptableEventNoParam onDayStart;
        
        
        [Header("Current Values")]
        [SerializeField] private int currentMoney;
        [SerializeField] private int currentEnergy;
        [SerializeField] private int currentBlessings;

        private void OnEnable()
        {
            if (onDayStart != null) onDayStart.OnRaised += ResetDailyResources;
        }

        private void OnDisable()
        {
            if (onDayStart != null) onDayStart.OnRaised -= ResetDailyResources;
        }

        private void Start()
        {
            InitializeResources();
        }

        private void InitializeResources()
        {
            if (config != null)
            {
                currentMoney = config.startMoney;
                currentEnergy = config.startEnergy;
                currentBlessings = config.startBlessings;
                
                UpdateAllUI();
            }
        }

        private void ResetDailyResources()
        {
            currentEnergy = config.startEnergy;
            Debug.Log("Daily resources reset");
            UpdateAllUI();
        }

        public bool SpendMoney(int amount)
        {
            if (currentMoney >= amount)
            {
                currentMoney -= amount;
                onMoneyChanged?.Raise(currentMoney);
                Debug.Log($"Spent {amount} money. Remaining: {currentMoney}");
                return true;
            }
            return false;
        }

        public void AddMoney(int amount)
        {
            currentMoney = Mathf.Min(currentMoney + amount, config.maxMoney);
            onMoneyChanged?.Raise(currentMoney);
            Debug.Log($"Added {amount} money. Total: {currentMoney}");
        }

        public bool SpendEnergy(int amount)
        {
            if (currentEnergy >= amount)
            {
                currentEnergy -= amount;
                onEnergyChanged?.Raise(currentEnergy);
                Debug.Log($"Spent {amount} energy. Remaining: {currentEnergy}");
                return true;
            }
            return false;
        }

        public void AddEnergy(int amount)
        {
            currentEnergy = Mathf.Min(currentEnergy + amount, config.maxEnergy);
            onEnergyChanged?.Raise(currentEnergy);
            Debug.Log($"Added {amount} energy. Total: {currentEnergy}");
        }

        public void AddBlessings(int amount)
        {
            currentBlessings += amount;
            onBlessingsChanged?.Raise(currentBlessings);
            Debug.Log($"Added {amount} blessings. Total: {currentBlessings}");
        }
        
        public void RemoveBlessings(int amount)
        {
            currentBlessings = Mathf.Max(currentBlessings - amount, 0);
            onBlessingsChanged?.Raise(currentBlessings);
            Debug.Log($"Removed {amount} blessings. Total: {currentBlessings}");
        }

        private void UpdateAllUI()
        {
            onMoneyChanged?.Raise(currentMoney);
            onEnergyChanged?.Raise(currentEnergy);
            onBlessingsChanged?.Raise(currentBlessings);
        }

        public int GetMoney() => currentMoney;
        public int GetEnergy() => currentEnergy;
        public int GetBlessings() => currentBlessings;
    }

    // ScriptRole: Manages game resources (money, energy, blessings)
    // RelatedScripts: HUDController, GameStateManager
    // UsesSO: ResourceConfigSO, IntEventSO, GameEventSO
    // ReceivesFrom: GameStateManager, ErrandSystem (future)
    // SendsTo: HUD via change events
}