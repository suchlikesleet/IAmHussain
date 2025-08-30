using UnityEngine;

namespace BOH
{
    [CreateAssetMenu(fileName = "TimeConfig", menuName = "BOH/Config/TimeConfig")]
    public class TimeConfigSO : ScriptableObject
    {
        [Header("Time Settings")]
        public float secondsPerMinute = 1f;
        public int startHour = 6;
        public int startMinute = 0;
        public int endHour = 22;
        public int endMinute = 0;
    }

    // ScriptRole: Time system configuration data
    // RelatedScripts: TimeSystem
}