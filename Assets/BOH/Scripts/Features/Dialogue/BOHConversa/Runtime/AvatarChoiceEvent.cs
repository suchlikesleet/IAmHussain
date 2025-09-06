using System.Collections.Generic;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;

namespace BOH.Conversa
{
    public class AvatarChoiceEvent : IConversationEvent
    {
        public BOH.AvatarSO Avatar { get; }
        public string ExpressionKey { get; }
        public string Message { get; }
        public System.Collections.Generic.List<Option> Options { get; }
        public string ActorName { get; }

        public AvatarChoiceEvent(BOH.AvatarSO avatar, string expressionKey, string message, List<Option> options)
        {
            Avatar = avatar;
            ExpressionKey = expressionKey;
            Message = message;
            Options = options;
            ActorName = avatar != null ? (string.IsNullOrEmpty(avatar.displayName) ? avatar.avatarId : avatar.displayName) : string.Empty;
        }
    }
}

