using System;
using System.Collections.Generic;
using UnityEngine;

namespace BOH
{
    [CreateAssetMenu(fileName = "Avatar", menuName = "BOH/Dialogue/Avatar", order = 0)]
    public class AvatarSO : ScriptableObject
    {
        [Header("Identity")]
        public string avatarId;
        public string displayName;

        [Header("Portraits")]
        public Sprite defaultPortrait;

        [Serializable] public struct Expression
        {
            public string key;     // e.g. "Neutral", "Happy", "Angry"
            public Sprite sprite;
        }

        [Tooltip("List of expression sprites by key.")]
        public List<Expression> expressions = new();

        public Sprite GetExpression(string key)
        {
            if (string.IsNullOrEmpty(key))
                return defaultPortrait;
            for (int i = 0; i < expressions.Count; i++)
                if (expressions[i].key == key)
                    return expressions[i].sprite ? expressions[i].sprite : defaultPortrait;
            return defaultPortrait;
        }
    }
}