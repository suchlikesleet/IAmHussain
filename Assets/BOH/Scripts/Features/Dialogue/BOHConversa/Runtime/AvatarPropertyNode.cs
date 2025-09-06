using System;
using Conversa.Runtime;
using Conversa.Runtime.Attributes;
using Conversa.Runtime.Interfaces;
using UnityEngine;

namespace BOH.Conversa
{
    [Serializable]
    public class AvatarPropertyNode : BaseNode, IValueNode
    {
        [ConversationProperty("Avatar", 0.25f, 0.6f, 0.95f)]
        [SerializeField] private BOH.AvatarSO avatar;

        [Slot("Avatar", "avatar", Flow.Out, Capacity.Many)]
        public BOH.AvatarSO Avatar
        {
            get => avatar;
            set => avatar = value;
        }

        public T GetValue<T>(string portGuid, Conversation conversation)
        {
            if (portGuid == "avatar")
            {
                return (T)(object)avatar;
            }
            return default;
        }
    }
}
