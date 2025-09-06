using System.Reflection;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class AvatarMessageNodeView : BaseNodeView<AvatarMessageNode>
    {
        protected override string Title => "Avatar Message";

        public AvatarMessageNodeView(Conversation conversation) : base(new AvatarMessageNode(), conversation) { }
        public AvatarMessageNodeView(AvatarMessageNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var avatarFieldInfo      = typeof(AvatarMessageNode).GetField("avatar",        BindingFlags.NonPublic | BindingFlags.Instance);
            var expressionFieldInfo  = typeof(AvatarMessageNode).GetField("expressionKey", BindingFlags.NonPublic | BindingFlags.Instance);
            var messageFieldInfo     = typeof(AvatarMessageNode).GetField("message",       BindingFlags.NonPublic | BindingFlags.Instance);

            var avatarField = new ObjectField("Avatar") { objectType = typeof(BOH.AvatarSO) };
            avatarField.SetValueWithoutNotify(avatarFieldInfo?.GetValue(Data) as BOH.AvatarSO);
            avatarField.RegisterValueChangedCallback(e => avatarFieldInfo?.SetValue(Data, e.newValue as BOH.AvatarSO));

            var exprField = new TextField("Expression Key");
            exprField.SetValueWithoutNotify((string)(expressionFieldInfo?.GetValue(Data) ?? string.Empty));
            exprField.RegisterValueChangedCallback(e => expressionFieldInfo?.SetValue(Data, e.newValue));

            var msgField = new TextField("Message") { multiline = true }; // UIElements supports multiline
            msgField.SetValueWithoutNotify((string)(messageFieldInfo?.GetValue(Data) ?? string.Empty));
            msgField.RegisterValueChangedCallback(e => messageFieldInfo?.SetValue(Data, e.newValue));

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(avatarField);
            wrapper.Add(exprField);
            wrapper.Add(msgField);

            bodyContainer.Add(wrapper);
        }
    }
}
