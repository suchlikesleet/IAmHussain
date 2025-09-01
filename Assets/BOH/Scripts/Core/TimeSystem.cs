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
        [SerializeField] private ScriptableEventNoParam onDayEnd;
        
        [Header("Current Time")]
        [SerializeField] private int currentHour;
        [SerializeField] private int currentMinute;
        
        private float timeAccumulator = 0f;
        private bool isRunning = false;

        private void OnEnable()
        {
            if (onDayStart != null) onDayStart.OnRaised += StartTime;;
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
            
            if (currentHour >= config.endHour && currentMinute >= config.endMinute)
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
            onDayEnd?.Raise();
            
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