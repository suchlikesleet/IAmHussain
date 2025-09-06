using System;
using System.Collections.Generic;
using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH.Conversa
{
    [Serializable]
    [Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
    public class AvatarChoiceNode : BaseNode, IEventNode
    {
        [SerializeField] private BOH.AvatarSO avatar;
        [SerializeField] private string expressionKey = "";
        [SerializeField] private string message = "";

        [SerializeField]
        private List<PortDefinition<BaseNode>> options = new List<PortDefinition<BaseNode>>
        {
            new PortDefinition<BaseNode>("yes", "Yes"),
            new PortDefinition<BaseNode>("no",  "No")
        };

        [ConversationProperty("Avatar", 0.25f, 0.6f, 0.95f)]
        public BOH.AvatarSO Avatar
        {
            get => avatar;
            set => avatar = value;
        }

        [ConversationProperty("Expression Key", 0.3f, 0.8f, 0.4f)]
        public string ExpressionKey
        {
            get => expressionKey;
            set => expressionKey = value;
        }

        [ConversationProperty("Message", 0.3f, 0.8f, 0.4f)]
        public string Message
        {
            get => message;
            set => message = value;
        }

        public List<PortDefinition<BaseNode>> Options
        {
            get => options;
            set => options = value;
        }

        public void Process(Conversation conversation, ConversationEvents conversationEvents)
        {
            void HandleClickOption(PortDefinition<BaseNode> portDefinition)
            {
                var nextNode = conversation.GetOppositeNodes(GetNodePort(portDefinition.Guid)).FirstOrDefault();
                conversation.Process(nextNode, conversationEvents);
            }

            Option MakeOption(PortDefinition<BaseNode> pd) => new Option(pd.Label, () => HandleClickOption(pd));

            var choiceEvent = new AvatarChoiceEvent(avatar, expressionKey, message, options.Select(MakeOption).ToList());
            conversationEvents.OnConversationEvent.Invoke(choiceEvent);
        }

        public override bool ContainsPort(string portId, Flow flow)
        {
            if (base.ContainsPort(portId, flow)) return true;
            return flow == Flow.Out && options.Any(option => option.Guid == portId);
        }
    }
}

