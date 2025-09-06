using System;
using Conversa.Runtime.Interfaces;

namespace BOH.Conversa
{
    public class AvatarMessageEvent : IConversationEvent
    {
        public BOH.AvatarSO Avatar { get; }
        public string ExpressionKey { get; }
        public string Message { get; }
        public string ActorName { get; }
        public Action Advance { get; }

        public AvatarMessageEvent(BOH.AvatarSO avatar, string expressionKey, string message, Action advance)
        {
            Avatar = avatar;
            ExpressionKey = expressionKey;
            Message = message;
            ActorName = avatar != null ? (string.IsNullOrEmpty(avatar.displayName) ? avatar.avatarId : avatar.displayName) : string.Empty;
            Advance = advance;
        }
    }
}

