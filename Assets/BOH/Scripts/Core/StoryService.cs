using UnityEngine;

namespace BOH
{
    public class StoryService : MonoBehaviour, IStoryService
    {
        [Min(0)] [SerializeField] private int currentChapter = 0;

        // IStoryService
        public int GetChapter() => currentChapter;

        // Helpers you can call from scripts / nodes later
        public void SetChapter(int chapter) { currentChapter = Mathf.Max(0, chapter); }
        public void AdvanceChapter(int delta = 1) { currentChapter = Mathf.Max(0, currentChapter + delta); }
    }
}