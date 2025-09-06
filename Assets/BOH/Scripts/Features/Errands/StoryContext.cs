// Assets/BOH/Scripts/StoryContext.cs
using System;

namespace BOH
{
    /// <summary>
    /// Minimal context used by ErrandSystem.CanAccept(...) and future Conversa nodes.
    /// No LINQ/lambdas; class (not struct) to avoid capture issues.
    /// </summary>
    public class StoryContext
    {
        private readonly ErrandSystem errands;
        private readonly IFlagService flags;
        private readonly ITrustService trust;
        private readonly IStoryService story;

        public int CurrentDay { get; private set; }

        public StoryContext(ErrandSystem errands,
                            IFlagService flags,
                            ITrustService trust,
                            IStoryService story,
                            int currentDay)
        {
            this.errands = errands;
            this.flags   = flags;
            this.trust   = trust;
            this.story   = story;
            this.CurrentDay = currentDay;
        }

        // ---------- Flag helpers (no LINQ) ----------
        public bool HasAllFlags(string[] list)
        {
            if (list == null || list.Length == 0) return true;
            if (flags == null) return false;
            for (int i = 0; i < list.Length; i++)
            {
                var f = list[i];
                if (!string.IsNullOrEmpty(f) && !flags.HasFlag(f)) return false;
            }
            return true;
        }

        public bool HasAnyFlag(string[] list)
        {
            if (list == null || list.Length == 0) return false;
            if (flags == null) return false;
            for (int i = 0; i < list.Length; i++)
            {
                var f = list[i];
                if (!string.IsNullOrEmpty(f) && flags.HasFlag(f)) return true;
            }
            return false;
        }

        public bool HasCompletedAll(string[] ids)
        {
            if (ids == null || ids.Length == 0) return true;
            if (errands == null) return false;
            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                if (string.IsNullOrEmpty(id)) return false;
                if (!errands.IsErrandCompleted(id)) return false;
            }
            return true;
        }

        // ---------- Trust / Story ----------
        public int GetTrust(string npcId)
        {
            if (string.IsNullOrEmpty(npcId) || trust == null) return int.MaxValue;
            return trust.GetTrust(npcId);
        }

        public bool IsChapterInRange(int min, int max)
        {
            int ch = (story != null) ? story.GetChapter() : 0;
            if (min >= 0 && ch < min) return false;
            if (max >= 0 && ch > max) return false;
            return true;
        }

        // Optional: implement if you decide to gate *offers* by hour as well.
        public bool IsHourInWindow(int startHour, int endHour)
        {
            // Accept everything by default; completion lateness is already handled in ErrandSystem.
            return true;
        }
    }
}
