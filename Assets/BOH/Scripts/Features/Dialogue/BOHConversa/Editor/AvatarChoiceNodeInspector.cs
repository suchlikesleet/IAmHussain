using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class AvatarChoiceNodeInspector : BaseNodeInspector<AvatarChoiceNode>
    {
        public AvatarChoiceNodeInspector(AvatarChoiceNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");

            // Avatar field
            var avatarField = new ObjectField("Avatar") { objectType = typeof(BOH.AvatarSO) };
            avatarField.RegisterValueChangedCallback(e => data.Avatar = e.newValue as BOH.AvatarSO);
            avatarField.SetValueWithoutNotify(data.Avatar);
            wrapper.Add(avatarField);

            // Expression key
            var exprField = new TextField("Expression Key");
            exprField.RegisterValueChangedCallback(e => data.ExpressionKey = e.newValue);
            exprField.SetValueWithoutNotify(data.ExpressionKey);
            wrapper.Add(exprField);

            // Message
            var messageField = new TextField("Message") { multiline = true };
            messageField.RegisterValueChangedCallback(e => data.Message = e.newValue);
            messageField.SetValueWithoutNotify(data.Message);
            wrapper.Add(messageField);

            // Options list
            var optionsWrapper = new VisualElement();
            optionsWrapper.AddToClassList("option-list");
            wrapper.Add(new Label("Options"));
            wrapper.Add(optionsWrapper);

            foreach (var portDef in data.Options)
            {
                var option = new global::Conversa.Editor.ChoiceOptionForm(portDef);
                option.OnDelete.AddListener(() => HandleDeleteOption(option, optionsWrapper));
                optionsWrapper.Add(option);
            }

            var addButton = new Button(() => HandleAddOption(optionsWrapper)) { text = "+ Add option" };
            wrapper.Add(addButton);

            Add(wrapper);
        }

        private void HandleAddOption(VisualElement optionsWrapper)
        {
            var newOption = new PortDefinition<BaseNode>(global::Conversa.Editor.General.NewGuid(), "");
            data.Options.Add(newOption);
            var form = new global::Conversa.Editor.ChoiceOptionForm(newOption);
            form.OnDelete.AddListener(() => HandleDeleteOption(form, optionsWrapper));
            optionsWrapper.Add(form);
        }

        private void HandleDeleteOption(global::Conversa.Editor.ChoiceOptionForm form, VisualElement optionsWrapper)
        {
            data.Options.Remove(form.portDefinition);
            optionsWrapper.Remove(form);
        }
    }
}
