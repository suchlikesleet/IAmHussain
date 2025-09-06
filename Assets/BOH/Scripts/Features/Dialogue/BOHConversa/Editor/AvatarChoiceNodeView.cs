using System;
using System.Linq;
using Conversa.Editor;
using Conversa.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BOH.Conversa
{
    public class AvatarChoiceNodeView : BaseNodeView<AvatarChoiceNode>
    {
        protected override string Title => "Avatar Choice";

        private Label avatarLabel;
        private Label messageLabel;
        private VisualElement optionList;

        public AvatarChoiceNodeView(Conversation conversation) : base(new AvatarChoiceNode(), conversation) { }
        public AvatarChoiceNodeView(AvatarChoiceNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var container = new VisualElement();
            container.AddToClassList("p-5");

            avatarLabel = new Label(Data.Avatar != null ? Data.Avatar.displayName : "");
            messageLabel = new Label(Data.Message);

            container.Add(new Label("Avatar:"));
            container.Add(avatarLabel);
            container.Add(new Label("Message:"));
            container.Add(messageLabel);

            optionList = new VisualElement();
            optionList.AddToClassList("option-list");
            container.Add(optionList);

            bodyContainer.Add(container);

            Data.Options.ForEach(AddOption);
            schedule.Execute(HandleNodeChange).Every(100);
        }

        private void AddOption(PortDefinition<BaseNode> option)
        {
            var optionElement = new global::Conversa.Editor.ChoiceOption(option);
            optionList.Add(optionElement);
            RegisterPort(optionElement.port, option.Guid);
        }

        private void HandleNodeChange()
        {
            var currentName = Data.Avatar != null ? Data.Avatar.displayName : "";
            if (avatarLabel.text != currentName) avatarLabel.text = currentName;
            if (messageLabel.text != Data.Message) messageLabel.text = Data.Message;

            var optionElements = bodyContainer.Query<global::Conversa.Editor.ChoiceOption>().ToList();
            // Remove old
            var toRemove = optionElements.Where(x => Data.Options.All(y => y.Guid != x.portDefinition.Guid)).ToList();
            foreach (var el in toRemove)
            {
                foreach (var conn in el.port.connections) GraphView.DeleteElements(new[] { conn });
                el.RemoveFromHierarchy();
            }
            // Add new
            var toAdd = Data.Options.Where(x => optionElements.TrueForAll(y => y.portDefinition.Guid != x.Guid)).ToList();
            toAdd.ForEach(AddOption);
            // Update labels
            foreach (var el in bodyContainer.Query<global::Conversa.Editor.ChoiceOption>().ToList()) el.Update();
        }

        public override void CollectElements(System.Collections.Generic.HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
        {
            base.CollectElements(collectedElementSet, conditionFunc);
            var choicePorts = bodyContainer.Query<Port>().ToList();
            collectedElementSet.UnionWith(choicePorts.SelectMany(port => port.connections));
        }
    }
}
