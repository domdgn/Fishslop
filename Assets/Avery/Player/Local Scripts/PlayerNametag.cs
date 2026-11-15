using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;

namespace FishSlop
{
    public class PlayerNameTag : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private GameObject nameTag;

        private const ulong DeveloperSteamID = 76561198406385504;

        [SyncVar(hook = nameof(OnNameChanged))]
        private string playerName;

        [SyncVar(hook = nameof(OnColourChanged))]
        private Color nameTagColour = Color.white;

        public override void OnStartLocalPlayer()
        {
            string persona = SteamFriends.GetPersonaName();
            bool isDev = SteamUser.GetSteamID().m_SteamID == DeveloperSteamID;

            if (string.IsNullOrEmpty(persona)) persona = "Player " + netId;

            if (isDev) persona = "[DEV] avery 🦴";

            Color colour = isDev ? Color.pink : Color.white;

            CmdSetName(persona, colour);
        }

        [Command]
        private void CmdSetName(string newName, Color colour)
        {
            playerName = newName;
            nameTagColour = colour;
        }

        private void OnNameChanged(string oldName, string newName)
        {
            nameText.text = newName;
        }

        private void OnColourChanged(Color oldColour, Color newColour)
        {
            nameText.color = newColour;
        }

        private void Start()
        {
            if (isLocalPlayer && nameTag != null) nameTag.SetActive(false);
        }
    }
}