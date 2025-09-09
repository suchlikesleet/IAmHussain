using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class ResourceChangeNodeView : BaseNodeView<ResourceChangeNode>
    {
        protected override string Title => "Resource Change";

        public ResourceChangeNodeView(Conversation conversation) : base(new ResourceChangeNode(), conversation) { }
        public ResourceChangeNodeView(ResourceChangeNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var t = typeof(ResourceChangeNode);
            var moneyInfo     = t.GetField("moneyDelta",     BindingFlags.NonPublic | BindingFlags.Instance);
            var energyInfo    = t.GetField("energyDelta",    BindingFlags.NonPublic | BindingFlags.Instance);
            var blessingsInfo = t.GetField("blessingsDelta", BindingFlags.NonPublic | BindingFlags.Instance);

            IntegerField Field(string label, int v, System.Action<int> on)
            {
                var f = new IntegerField(label);
                f.SetValueWithoutNotify(v);
                f.RegisterValueChangedCallback(e => on(e.newValue));
                return f;
            }

            var money     = Field("Money Δ",     (int)(moneyInfo?.GetValue(Data) ?? 0), v => moneyInfo?.SetValue(Data, v));
            var energy    = Field("Energy Δ",    (int)(energyInfo?.GetValue(Data) ?? 0), v => energyInfo?.SetValue(Data, v));
            var blessings = Field("Blessings Δ", (int)(blessingsInfo?.GetValue(Data) ?? 0), v => blessingsInfo?.SetValue(Data, v));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(money);
            wrapper.Add(energy);
            wrapper.Add(blessings);

            bodyContainer.Add(wrapper);
        }
    }
}

