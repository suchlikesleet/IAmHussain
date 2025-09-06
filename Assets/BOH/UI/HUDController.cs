using Obvious.Soap;
using UnityEngine;
using TMPro;

namespace BOH
{
    public class HUDController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI blessingsText;
        
        [Header("Events")]
        [SerializeField] private ScriptableEventInt onMoneyChanged;
        [SerializeField] private ScriptableEventInt onEnergyChanged;
        [SerializeField] private ScriptableEventInt onBlessingsChanged;
        [SerializeField] private ScriptableEventInt onMinuteTick;
        
        private TimeSystem timeSystem;

        private void OnEnable()
        {
            if (onMoneyChanged != null) onMoneyChanged.OnRaised += UpdateMoney;
            if (onEnergyChanged != null) onEnergyChanged.OnRaised += UpdateEnergy;
            if (onBlessingsChanged != null) onBlessingsChanged.OnRaised += UpdateBlessings;
            if (onMinuteTick != null) onMinuteTick.OnRaised += UpdateTime;
        }

        private void OnDisable()
        {
            if (onMoneyChanged != null) onMoneyChanged.OnRaised -= UpdateMoney;
            if (onEnergyChanged != null) onEnergyChanged.OnRaised -= UpdateEnergy;
            if (onBlessingsChanged != null) onBlessingsChanged.OnRaised -= UpdateBlessings;
            if (onMinuteTick != null) onMinuteTick.OnRaised -= UpdateTime;
        }

        private void Start()
        {
            timeSystem = FindFirstObjectByType<TimeSystem>();
        }

        private void UpdateMoney(int value)
        {
            if (moneyText != null)
                moneyText.text = $"Money: {value:N0}";
        }

        private void UpdateEnergy(int value)
        {
            if (energyText != null)
                energyText.text = $"Energy: {value}";
        }

        private void UpdateBlessings(int value)
        {
            if (blessingsText != null)
                blessingsText.text = $"Blessings: {value}";
        }

        private void UpdateTime(int totalMinutes)
        {
            if (timeText != null && timeSystem != null)
                timeText.text = timeSystem.GetTimeString();
        }
    }

    // ScriptRole: Updates HUD display elements
    // RelatedScripts: TimeSystem, ResourceSystem
    // UsesSO: IntEventSO
    // ReceivesFrom: TimeSystem, ResourceSystem via events
}

