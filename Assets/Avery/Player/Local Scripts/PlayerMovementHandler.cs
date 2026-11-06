using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections;

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
        [SerializeField] private Transform headPivot;
        [SerializeField] private float lookSens = 0.2f;
        [SerializeField] private float maxLookY = 80f;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.2f;
        [SerializeField] private LayerMask groundMask;

        private Rigidbody rb;
        private PlayerInput playerInput;

        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpPressed;
        private bool isSprinting;

        private float yaw;
        private float pitch;

        private PlayerInteraction playerInteraction; 
        private ShipMovement currentShip;
        private PlayerSteerShip playerSteerShip;

        private Vector3 lastShipPosition;
        private Quaternion lastShipRotation;

        public GameObject boop;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();
            playerInteraction = GetComponent<PlayerInteraction>();
            playerSteerShip = GetComponent<PlayerSteerShip>();

            playerSteerShip.enabled = false;
            playerInput.enabled = false;
            boop.SetActive(true);

            if (playerInteraction != null) playerInteraction.enabled = false;

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
            playerSteerShip.enabled = true;
            playerInteraction.enabled = true;

            playerInput.SwitchCurrentActionMap("Player");

            yaw = transform.eulerAngles.y;
            pitch = 0f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            boop.SetActive(false);

            if (cameraPivot != null) 
            { 
                cameraPivot.gameObject.SetActive(true);
                playerInteraction.cameraPivot = cameraPivot;
            }
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();

            rb.isKinematic = true;

            playerInput.enabled = false;
            playerInteraction.enabled = false;
            if (cameraPivot != null) { cameraPivot.gameObject.SetActive(false); }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            jumpPressed = false;
            isSprinting = false;

            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!isOwned) { return; }

            HandleLook();

            playerInteraction.DoRaycast();
        }

        public void OnMove(InputValue value)
        {
            if (!isActiveAndEnabled || !netIdentity || !isOwned) return;
            moveInput = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            if (!isActiveAndEnabled || !netIdentity || !isOwned) return;
            lookInput = value.Get<Vector2>();
        }

        public void OnSprint(InputValue value)
        {
            //Debug.Log($"Sprint callback: {value.isPressed}");

            if (!isActiveAndEnabled || !netIdentity || !isOwned) return;

            isSprinting = value.isPressed;
        }

        public void OnJump(InputValue value)
        {
            if (!isActiveAndEnabled || !netIdentity || !isOwned || !value.isPressed) return;
            jumpPressed = true;
        }

        public void OnInteract(InputValue value)
        {
            if (!isActiveAndEnabled || !netIdentity || !isOwned || !value.isPressed) return;
            Debug.Log("PlayerMovementHandler: OnInteract called.");
            playerInteraction?.DoInteract();
        }

        private IEnumerator DelayedJumpReset()
        {
            jumpPressed = true;
            yield return new WaitForFixedUpdate();
            jumpPressed = false;
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

            if (headPivot != null)
            {
                headPivot.localEulerAngles = new Vector3(pitch - 90f, 0f, 0f);
            }
        }

        private void FixedUpdate()
        {
            if (!isOwned) { return; }

            ApplyShipMovement();

            rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));

            HandleMovement();
            HandleJump();
        }

        private void HandleMovement()
        {
            Vector3 moveDir = transform.right * moveInput.x + transform.forward * moveInput.y;
            float speed = isSprinting ? sprintSpeed : moveSpeed;
            Vector3 velocity = moveDir * speed;
            velocity.y = rb.linearVelocity.y;
            rb.linearVelocity = velocity;
        }

        private void HandleJump()
        {
            if (!jumpPressed) return;

            jumpPressed = false;

            if (!IsGrounded()) return;

            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isOwned) return;

            ShipMovement ship = other.GetComponentInParent<ShipMovement>();

            if (ship == null) return;

            currentShip = ship;

            lastShipPosition = ship.transform.position;
            lastShipRotation = ship.transform.rotation;
        }

        private void OnTriggerExit(Collider other)
        {
            ShipMovement ship = other.GetComponentInParent<ShipMovement>();

            if (ship == currentShip)
            {
                currentShip = null;
            }
        }

        private void ApplyShipMovement()
        {
            if (currentShip == null) return;

            Transform shipTransform = currentShip.transform;

            Vector3 localPosition = Quaternion.Inverse(lastShipRotation) * (rb.position - lastShipPosition);

            Vector3 newPosition = shipTransform.position + shipTransform.rotation * localPosition;
            rb.position = newPosition;

            Quaternion deltaRotation = shipTransform.rotation * Quaternion.Inverse(lastShipRotation);

            Vector3 newForward = deltaRotation * Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;

            yaw = Mathf.Atan2(newForward.x, newForward.z) * Mathf.Rad2Deg;

            lastShipPosition = shipTransform.position;
            lastShipRotation = shipTransform.rotation;
        }

        public void ClearMovementInput()
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            jumpPressed = false;
            isSprinting = false;
        }

        private bool IsGrounded()
        {
            return Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);
        }
    }
}
