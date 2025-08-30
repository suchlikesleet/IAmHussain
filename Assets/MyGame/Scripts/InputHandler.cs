using System;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BOH
{
    public class InputHandler : MonoBehaviour
    {
        
        public event Action onTogglePause;
        public UnityEvent onTogglePauseEvent;
        public UnityEvent onInteractEvent;
        
        public void TogglePause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                onTogglePause?.Invoke();
                onTogglePauseEvent?.Invoke();
            }
            
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                onInteractEvent?.Invoke();
            }
        }
    }
}
