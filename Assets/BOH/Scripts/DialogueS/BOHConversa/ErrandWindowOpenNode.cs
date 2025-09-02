using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH
{
    [Port("Value", "value", typeof(bool), Flow.Out, Capacity.One)]
    [System.Serializable]
    public sealed class ErrandWindowOpenNode : BaseNode, IValueNode
    {
        [SerializeField, ConversationProperty("Errand", 120, 200, 255)]
        private ErrandSO errand;

        [Slot("Errand (In)", "errand", Flow.In, Capacity.One)]
        public ErrandSO ErrandInput { get; set; }

        public T GetValue<T>(string portGuid, Conversation conversation)
        {
            bool open = false;
            var e = ErrandInput ?? errand;
            if (e != null)
            {
                var time = Object.FindFirstObjectByType<TimeSystem>();
                if (time != null)
                {
                    int curHour;
                    try { curHour = time.GetTotalMinutes() / 60; } // preferred numeric path (you already use GetTotalMinutes) :contentReference[oaicite:10]{index=10}
                    catch { int.TryParse(time.GetTimeString()?.Substring(0, 2), out curHour); } // mirrors IsErrandLate hour parse

                    open = curHour >= e.startHour && curHour < e.endHour; // hour window from ErrandSO :contentReference[oaicite:11]{index=11}
                }
            }
            return (T)(object)open;
        }
    }
}