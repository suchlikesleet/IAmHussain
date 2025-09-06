using System;
using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH.Conversa
{
    [Serializable]
    [Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
    [Port("Next", "next", typeof(BaseNode), Flow.Out, Capacity.One)]
    public class AvatarMessageNode : BaseNode, IEventNode
    {
        [ConversationProperty("Avatar", 0.25f, 0.6f, 0.95f)]
        [SerializeField] private BOH.AvatarSO avatar;

        // Allow wiring the Avatar as a property input similar to Actor
        [Slot("Avatar", "avatar", Flow.In, Capacity.One)]
        public BOH.AvatarSO Avatar
        {
            get => avatar;
            set => avatar = value;
        }

        [ConversationProperty("Expression Key", 120, 200, 120)]
        [SerializeField] private string expressionKey = "";

        [ConversationProperty("Message", 0.3f, 0.8f, 0.4f)]
        [TextArea(2,5)]
        [SerializeField] private string message = "";

        [Slot("Expression Key", "expression-key", Flow.In, Capacity.One)]
        public string ExpressionKey
        {
            get => expressionKey;
            set => expressionKey = value;
        }

        [Slot("Message", "message", Flow.In, Capacity.One)]
        public string Message
        {
            get => message;
            set => message = value;
        }

        public void Process(Conversation conversation, ConversationEvents conversationEvents)
        {
            void Advance()
            {
                var nextNode = conversation.GetOppositeNodes(GetNodePort("next")).FirstOrDefault();
                conversation.Process(nextNode, conversationEvents);
            }

            var finalAvatar   = ProcessPort(conversation, "avatar", avatar);
            var finalExprKey  = ProcessPort(conversation, "expression-key", expressionKey);
            var finalMessage  = ProcessPort(conversation, "message", message);

            var e = new AvatarMessageEvent(finalAvatar, finalExprKey, finalMessage, Advance);
            conversationEvents.OnConversationEvent.Invoke(e);
        }
    }
}
