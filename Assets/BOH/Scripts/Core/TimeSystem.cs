using UnityEngine;
using System;
using Obvious.Soap;
using TMPro;

namespace BOH
{
    public class TimeSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private TimeConfigSO config;
        
        [Header("Events")]
        [SerializeField] private ScriptableEventInt onMinuteTick;
        [SerializeField] private ScriptableEventNoParam onDayStart;
        [SerializeField] private ScriptableEventNoParam onDayEnd; // Deprecated: day end is owned by GameStateManager
        
        [Header("Current Time")]
        [SerializeField] private int currentHour;
        [SerializeField] private int currentMinute;
        
        private float timeAccumulator = 0f;
        private bool isRunning = false;

        private void OnEnable()
        {
            if (onDayStart != null) onDayStart.OnRaised += StartTime;
            //if (onDayEnd != null) onDayEnd.OnRaised += StopTime;
        }

        private void OnDisable()
        {
            if (onDayStart != null) onDayStart.OnRaised -= StartTime;
            //if (onDayEnd != null) onDayEnd.OnRaised -= StopTime;
        }

        private void Start()
        {
            if (config != null)
            {
                currentHour = config.startHour;
                currentMinute = config.startMinute;
            }
        }

        private void Update()
        {
            if (!isRunning) return;
            
            timeAccumulator += Time.deltaTime;
            
            if (timeAccumulator >= config.secondsPerMinute)
            {
                timeAccumulator -= config.secondsPerMinute;
                AdvanceMinute();
            }
        }

        private void AdvanceMinute()
        {
            currentMinute++;
            
            if (currentMinute >= 60)
            {
                currentMinute = 0;
                currentHour++;
                
                if (currentHour >= 24)
                {
                    currentHour = 0;
                }
            }
            
            onMinuteTick?.Raise(GetTotalMinutes());

            // Use total minutes for robust comparison against end-of-day
            int total = GetTotalMinutes();
            int endTotal = (config.endHour * 60) + Mathf.Clamp(config.endMinute, 0, 59);
            if (total >= endTotal)
            {
                Debug.Log("Day time limit reached");
                StopTime();
            }
        }

        public void StartTime()
        {
            isRunning = true;
            Debug.Log("Time system started");
        }

        public void StopTime()
        {
            isRunning = false;
            Debug.Log("Time system stopped");

            // Delegate day-end transition to GameStateManager to avoid duplicate events
            var gsm = UnityEngine.Object.FindFirstObjectByType<GameStateManager>();
            gsm?.EndDay();
        }

        public string GetTimeString()
        {
            return $"{currentHour:D2}:{currentMinute:D2}";
        }

        public int GetTotalMinutes()
        {
            return currentHour * 60 + currentMinute;
        }
    }

    // ScriptRole: Manages in-game time progression
    // RelatedScripts: GameStateManager, HUDController
    // UsesSO: TimeConfigSO, IntEventSO, GameEventSO
    // ReceivesFrom: GameStateManager events
    // SendsTo: HUD via minute tick events
}
