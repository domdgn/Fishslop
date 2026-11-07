using UnityEngine;
using Mirror;

namespace FishSlop
{
    public class PlayerLookHandler : NetworkBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Transform headPivot;

        [SerializeField] private float lookSens = 0.2f;
        [SerializeField] private float maxLookY = 80f;

        private float yaw;
        private float pitch;

        private PlayerMovementHandler movement;

        private void Awake()
        {
            movement = GetComponent<PlayerMovementHandler>();

            if (cameraPivot != null) cameraPivot.gameObject.SetActive(false);
        }

        public void EnableLook()
        {
            yaw = transform.eulerAngles.y;
            pitch = 0f;

            if (cameraPivot != null) cameraPivot.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void DisableLook()
        {
            if (cameraPivot != null) cameraPivot.gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void HandleLook(Vector2 input)
        {
            if (!isOwned) return;

            yaw += input.x * lookSens;
            pitch -= input.y * lookSens;
            pitch = Mathf.Clamp(pitch, -maxLookY, maxLookY);

            if (cameraPivot != null) cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);

            if (headPivot != null) headPivot.localEulerAngles = new Vector3(pitch - 90f, 0f, 0f);

            movement.CmdSetYaw(yaw);
        }
    }
}
