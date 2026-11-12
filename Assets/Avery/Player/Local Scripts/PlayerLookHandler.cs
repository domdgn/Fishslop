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

        [SyncVar] private float syncedPitch;
        private float pitch;

        private PlayerMovementHandler movement;

        private void Awake()
        {
            movement = GetComponent<PlayerMovementHandler>();

            if (cameraPivot != null) cameraPivot.gameObject.SetActive(false);
        }

        public void EnableLook()
        {
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

            movement.AddYaw(input.x * lookSens);
            pitch = Mathf.Clamp(pitch - input.y * lookSens, -maxLookY, maxLookY);

            if (Mathf.Abs(pitch - syncedPitch) > 1f) CmdSetPitch(pitch);
        }

        [Command(channel = Channels.Unreliable)]
        private void CmdSetPitch(float p) => syncedPitch = p;

        private void LateUpdate()
        {
            if (isOwned)
            {
                if (cameraPivot != null && cameraPivot.gameObject.activeSelf)
                {
                    cameraPivot.rotation = Quaternion.Euler(pitch, movement.Yaw, 0f);
                }

                if (headPivot != null)
                {
                    headPivot.localEulerAngles = new Vector3(pitch - 90f, 0f, 0f);
                }
            }
            else if (headPivot != null)
            {
                headPivot.localEulerAngles = new Vector3(syncedPitch - 90f, 0f, 0f);
            }
        }
    }
}