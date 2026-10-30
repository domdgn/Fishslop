using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

namespace FishSlop 
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovementHandler : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpForce = 6f;

        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float lookSens = 0.2f;
        [SerializeField] private float maxLookY = 80f;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.2f;
        [SerializeField] private LayerMask groundMask;

        private Rigidbody rb;
        private PlayerInput playerInput;
        
        private InputAction moveAction;
        private InputAction sprintAction;
        private InputAction jumpAction;
        private InputAction lookAction;

        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpPressed;

        private float yaw;
        private float pitch;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();

            playerInput.enabled = false;

            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            cameraPivot.gameObject.SetActive(false);
            rb.isKinematic = true;
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            rb.isKinematic = false;

            playerInput.enabled = true;

            moveAction = playerInput.actions.FindAction("Player/Move", true);
            lookAction = playerInput.actions.FindAction("Player/Look", true);
            jumpAction = playerInput.actions.FindAction("Player/Jump", true);
            sprintAction = playerInput.actions.FindAction("Player/Sprint", true);

            yaw = transform.eulerAngles.y;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (cameraPivot != null) { cameraPivot.gameObject.SetActive(true); }
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();

            rb.isKinematic = true;

            playerInput.enabled = false;
            if (cameraPivot != null) { cameraPivot.gameObject.SetActive(false); }
        }

        private void Update()
        {
            if (!isOwned) { return; }
            ReadInput();
            HandleLook();
        }

        private void ReadInput()
        {
            moveInput = moveAction.ReadValue<Vector2>();
            lookInput = lookAction.ReadValue<Vector2>();
            jumpPressed = jumpAction.triggered;
        }

        private void HandleLook()
        {
            yaw += lookInput.x * lookSens;
            pitch -= lookInput.y * lookSens;
            pitch = Mathf.Clamp(pitch, -maxLookY, maxLookY);

            if (cameraPivot != null)
            {
                cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
            }
        }

        private void FixedUpdate()
        {
            if (!isOwned) { return; }

            rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));

            HandleMovement();
            HandleJump();
        }

        private void HandleMovement()
        {
            Vector3 moveDir = transform.right * moveInput.x + transform.forward * moveInput.y;
            float speed = sprintAction.IsPressed() ? sprintSpeed : moveSpeed;
            Vector3 velocity = moveDir * speed;
            velocity.y = rb.linearVelocity.y;
            rb.linearVelocity = velocity;
        }

        private void HandleJump()
        {
            if (jumpPressed && IsGrounded())
            {
                Debug.Log("Jumping");
                Vector3 vel = rb.linearVelocity;
                vel.y = 0f;
                rb.linearVelocity = vel;
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            jumpAction.Reset();
        }

        private bool IsGrounded()
        {
            return Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);
        }
    }
}
