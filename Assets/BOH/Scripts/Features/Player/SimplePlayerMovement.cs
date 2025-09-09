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
        [SerializeField]private Animator anim2D;
        private bool isFlipped = false;
        [SerializeField]private Transform rootTransform;
        
        public UnityEvent onInteractEvent;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Move(InputAction.CallbackContext context)
        {
            _inputVector = context.ReadValue<Vector2>();
            anim2D.SetFloat("speed", _inputVector.magnitude);
            if (context.performed)
            {
                if (_inputVector.x >0 && !isFlipped)
                {
                    rootTransform.localScale = new Vector3(-1*Mathf.Abs(rootTransform.localScale.x), rootTransform.localScale.y, 1);
                    isFlipped = true;
                }else if (_inputVector.x < 0 && isFlipped)
                {
                    rootTransform.localScale = new Vector3(Mathf.Abs(rootTransform.localScale.x), rootTransform.localScale.y, 1);
                    isFlipped = false;
                }
            }
            
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
