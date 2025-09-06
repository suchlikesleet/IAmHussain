using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class AvatarPropertyNodeView : BaseNodeView<AvatarPropertyNode>
    {
        protected override string Title => "Avatar Property";

        public AvatarPropertyNodeView(Conversation conversation) : base(new AvatarPropertyNode(), conversation) { }
        public AvatarPropertyNodeView(AvatarPropertyNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var avatarFieldInfo = typeof(AvatarPropertyNode).GetField("avatar", BindingFlags.NonPublic | BindingFlags.Instance);

            var avatarField = new ObjectField("Avatar") { objectType = typeof(BOH.AvatarSO) };
            avatarField.SetValueWithoutNotify(avatarFieldInfo?.GetValue(Data) as BOH.AvatarSO);
            avatarField.RegisterValueChangedCallback(e => avatarFieldInfo?.SetValue(Data, e.newValue as BOH.AvatarSO));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(avatarField);
            bodyContainer.Add(wrapper);
        }
    }
}

