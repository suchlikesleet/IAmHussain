using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class TimeBetweenNodeView : BaseNodeView<TimeBetweenNode>
    {
        protected override string Title => "Time Between";

        public TimeBetweenNodeView(Conversation conversation) : base(new TimeBetweenNode(), conversation) { }
        public TimeBetweenNodeView(TimeBetweenNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var t = typeof(TimeBetweenNode);
            var startHourInfo   = t.GetField("startHour",   BindingFlags.NonPublic | BindingFlags.Instance);
            var startMinuteInfo = t.GetField("startMinute", BindingFlags.NonPublic | BindingFlags.Instance);
            var endHourInfo     = t.GetField("endHour",     BindingFlags.NonPublic | BindingFlags.Instance);
            var endMinuteInfo   = t.GetField("endMinute",   BindingFlags.NonPublic | BindingFlags.Instance);
            var inclStartInfo   = t.GetField("inclusiveStart", BindingFlags.NonPublic | BindingFlags.Instance);
            var inclEndInfo     = t.GetField("inclusiveEnd",   BindingFlags.NonPublic | BindingFlags.Instance);

            IntegerField IntField(string label, int v, System.Action<int> on)
            {
                var f = new IntegerField(label);
                f.SetValueWithoutNotify(v);
                f.RegisterValueChangedCallback(e => on(e.newValue));
                return f;
            }

            var startHour   = IntField("Start Hour",   (int)(startHourInfo?.GetValue(Data) ?? 0), v => startHourInfo?.SetValue(Data, Mathf.Clamp(v,0,23)));
            var startMinute = IntField("Start Minute", (int)(startMinuteInfo?.GetValue(Data) ?? 0), v => startMinuteInfo?.SetValue(Data, Mathf.Clamp(v,0,59)));
            var endHour     = IntField("End Hour",     (int)(endHourInfo?.GetValue(Data) ?? 0), v => endHourInfo?.SetValue(Data, Mathf.Clamp(v,0,23)));
            var endMinute   = IntField("End Minute",   (int)(endMinuteInfo?.GetValue(Data) ?? 0), v => endMinuteInfo?.SetValue(Data, Mathf.Clamp(v,0,59)));

            var inclStart = new Toggle("Inclusive Start");
            inclStart.SetValueWithoutNotify((bool)(inclStartInfo?.GetValue(Data) ?? true));
            inclStart.RegisterValueChangedCallback(e => inclStartInfo?.SetValue(Data, e.newValue));

            var inclEnd = new Toggle("Inclusive End");
            inclEnd.SetValueWithoutNotify((bool)(inclEndInfo?.GetValue(Data) ?? false));
            inclEnd.RegisterValueChangedCallback(e => inclEndInfo?.SetValue(Data, e.newValue));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(startHour);
            wrapper.Add(startMinute);
            wrapper.Add(endHour);
            wrapper.Add(endMinute);
            wrapper.Add(inclStart);
            wrapper.Add(inclEnd);

            bodyContainer.Add(wrapper);
        }
    }
}

