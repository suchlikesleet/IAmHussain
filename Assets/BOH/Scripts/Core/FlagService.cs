using System.Collections.Generic;
using UnityEngine;

namespace BOH
{
    public class FlagService : MonoBehaviour, IFlagService
    {
        [Tooltip("Flags that should be true at game start.")]
        [SerializeField] private List<string> initialFlags = new();

        private readonly HashSet<string> _flags = new();

        private void Awake()
        {
            for (int i = 0; i < initialFlags.Count; i++)
            {
                var f = initialFlags[i];
                if (!string.IsNullOrEmpty(f)) _flags.Add(f);
            }
        }

        // IFlagService
        public bool HasFlag(string flag) => !string.IsNullOrEmpty(flag) && _flags.Contains(flag);

        // Helpers you can call from scripts / nodes later
        public void SetFlag(string flag)            { if (!string.IsNullOrEmpty(flag)) _flags.Add(flag); }
        public void ClearFlag(string flag)          { if (!string.IsNullOrEmpty(flag)) _flags.Remove(flag); }
        public void SetFlag(string flag, bool on)   { if (on) SetFlag(flag); else ClearFlag(flag); }
    }
}