using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class TimeBeforeNodeView : BaseNodeView<TimeBeforeNode>
    {
        protected override string Title => "Time Before";

        public TimeBeforeNodeView(Conversation conversation) : base(new TimeBeforeNode(), conversation) { }
        public TimeBeforeNodeView(TimeBeforeNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var hourFieldInfo      = typeof(TimeBeforeNode).GetField("hour",      BindingFlags.NonPublic | BindingFlags.Instance);
            var minuteFieldInfo    = typeof(TimeBeforeNode).GetField("minute",    BindingFlags.NonPublic | BindingFlags.Instance);
            var inclusiveFieldInfo = typeof(TimeBeforeNode).GetField("inclusive", BindingFlags.NonPublic | BindingFlags.Instance);

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

            var inclusiveToggle = new Toggle("Inclusive ≤");
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

