using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BOH
{
    public class SimplePlayerMovement : MonoBehaviour
    {
        [SerializeField]private float speed = 5f;
        private Vector2 _inputVector = Vector2.zero;    
        private Rigidbody2D _rb;
        
        public UnityEvent onInteractEvent;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Move(InputAction.CallbackContext context)
        {
            _inputVector = context.ReadValue<Vector2>();
            
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                onInteractEvent?.Invoke();
            }
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _inputVector * speed;
        }
    }
}
