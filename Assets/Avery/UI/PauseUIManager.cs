using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishSlop
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        public static bool IsPaused { get; private set; }

        private string previousMap = "Player";

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) SetPaused(!IsPaused);
        }

        private void Start()
        {
            panel.SetActive(false);
        }

        public void Resume()
        {
            SetPaused(false);
        }

        private void SetPaused(bool paused)
        {
            IsPaused = paused;
            panel.SetActive(paused);

            var player = NetworkClient.localPlayer;
            if (player != null)
            {
                var input = player.GetComponent<PlayerInput>();
                var move = player.GetComponent<PlayerMovementHandler>();

                if (paused)
                {
                    previousMap = input.currentActionMap != null ? input.currentActionMap.name : "Player";
                    input.SwitchCurrentActionMap("UI");
                    move.ClearMovementInput();
                }
                else
                {
                    input.SwitchCurrentActionMap(previousMap);
                }
            }

            Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = IsPaused;
        }

        public void LeaveGame()
        {
            IsPaused = false;
            if (SteamLobby.Instance != null) SteamLobby.Instance.LeaveLobby();
        }
    }
}