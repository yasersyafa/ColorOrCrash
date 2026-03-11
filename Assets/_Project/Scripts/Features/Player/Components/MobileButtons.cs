using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ColorOrCrash.Features.Player.Components
{
    public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnHeld;
        public event Action OnReleased;

        public bool IsHeld { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsHeld = true;
            OnHeld?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsHeld = false;
            OnReleased?.Invoke();
        }
    }   
}