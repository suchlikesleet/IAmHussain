using UnityEngine;
using System.Collections.Generic;

namespace BOH
{
    public class JournalSystem : MonoBehaviour
    {
        [System.Serializable]
        public class JournalEntry
        {
            public string timestamp;
            public string content;
            public EntryType type;
        }

        public enum EntryType
        {
            Errand,
            Gift,
            Special,
            Trust,
            Bond,
            Daily
        }

        [Header("Journal Data")]
        [SerializeField] private List<JournalEntry> todaysEntries = new List<JournalEntry>();
        [SerializeField] private List<JournalEntry> allEntries = new List<JournalEntry>();
        
        private TimeSystem timeSystem;

        private void Start()
        {
            timeSystem = GameServices.Time ?? FindFirstObjectByType<TimeSystem>();
        }

        public void AddSpecialEntry(string content)
        {
            var entry = new JournalEntry
            {
                timestamp = timeSystem?.GetTimeString() ?? "??:??",
                content = content,
                type = EntryType.Special
            };
            
            todaysEntries.Add(entry);
            allEntries.Add(entry);
            
            Debug.Log($"Journal entry added: {content}");
        }

        public void AddGiftEntry(string giver, string recipient, string item, string outcome)
        {
            string content = $"You gave {item} to {recipient}. {outcome}";
            var entry = new JournalEntry
            {
                timestamp = timeSystem?.GetTimeString() ?? "??:??",
                content = content,
                type = EntryType.Gift
            };
            
            todaysEntries.Add(entry);
            allEntries.Add(entry);
        }

        public List<JournalEntry> GetTodaysEntries()
        {
            return new List<JournalEntry>(todaysEntries);
        }

        public void ClearDailyEntries()
        {
            todaysEntries.Clear();
        }
    }

    // ScriptRole: Records and manages journal entries
    // RelatedScripts: GiftingSystem, ErrandSystem
    // ReceivesFrom: Various systems for logging
}
