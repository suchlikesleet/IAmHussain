using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class TimeAfterNodeView : BaseNodeView<TimeAfterNode>
    {
        protected override string Title => "Time After";

        public TimeAfterNodeView(Conversation conversation) : base(new TimeAfterNode(), conversation) { }
        public TimeAfterNodeView(TimeAfterNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var hourFieldInfo      = typeof(TimeAfterNode).GetField("hour",      BindingFlags.NonPublic | BindingFlags.Instance);
            var minuteFieldInfo    = typeof(TimeAfterNode).GetField("minute",    BindingFlags.NonPublic | BindingFlags.Instance);
            var inclusiveFieldInfo = typeof(TimeAfterNode).GetField("inclusive", BindingFlags.NonPublic | BindingFlags.Instance);

            var hourField = new IntegerField("Hour");
            hourField.SetValueWithoutNotify((int)(hourFieldInfo?.GetValue(Data) ?? 0));
            hourField.RegisterValueChangedCallback(e =>
            {
                var v = Mathf.Clamp(e.newValue, 0, 23);
                hourFieldInfo?.SetValue(Data, v);
                hourField.SetValueWithoutNotify(v);
            });

            var minuteField = new IntegerField("Minute");
            minuteField.SetValueWithoutNotify((int)(minuteFieldInfo?.GetValue(Data) ?? 0));
            minuteField.RegisterValueChangedCallback(e =>
            {
                var v = Mathf.Clamp(e.newValue, 0, 59);
                minuteFieldInfo?.SetValue(Data, v);
                minuteField.SetValueWithoutNotify(v);
            });

            var inclusiveToggle = new Toggle("Inclusive ≥");
            inclusiveToggle.SetValueWithoutNotify((bool)(inclusiveFieldInfo?.GetValue(Data) ?? false));
            inclusiveToggle.RegisterValueChangedCallback(e => inclusiveFieldInfo?.SetValue(Data, e.newValue));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(hourField);
            wrapper.Add(minuteField);
            wrapper.Add(inclusiveToggle);

            bodyContainer.Add(wrapper);
        }
    }
}

