using NocturneThree.EventSystem;
using UnityEngine;

/// <summary>
/// Event definition for TextFloatEvent.
/// </summary>
public struct TextFloatEvent : IGameEvent
{
        public readonly Vector3 WorldPosition;
        public readonly string TextContent;

        public TextFloatEvent(Vector3 worldPos, string textContent)
        {
            WorldPosition = worldPos;
            TextContent = textContent;
        }
}
