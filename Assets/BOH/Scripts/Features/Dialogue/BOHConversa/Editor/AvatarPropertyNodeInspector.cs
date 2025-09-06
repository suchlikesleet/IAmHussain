using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class AvatarPropertyNodeInspector : BaseNodeInspector<AvatarPropertyNode>
    {
        public AvatarPropertyNodeInspector(AvatarPropertyNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");

            var avatarField = new ObjectField("Avatar") { objectType = typeof(BOH.AvatarSO) };
            avatarField.RegisterValueChangedCallback(e => data.Avatar = e.newValue as BOH.AvatarSO);
            avatarField.SetValueWithoutNotify(data.Avatar);
            wrapper.Add(avatarField);

            Add(wrapper);
        }
    }
}

