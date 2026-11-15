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

        private const ulong developerSteamID = 76561198406385504;
        private const string devName = "[DEV] avery 🦴";

        private static readonly ulong[] playtesterSteamIDs =
        {
            76561198879304215,
            76561198978020021,
        };
        private const string playtesterPrefix = "[TESTER] ";

        private const int maxNameLength = 24;

        private static readonly Color devColour = new Color(1f, 0.5f, 0.8f);
        private static readonly Color playtesterColour = new Color(0.5f, 0.9f, 1f);

        [SyncVar(hook = nameof(OnNameChanged))]
        private string playerName = "";

        [SyncVar(hook = nameof(OnColourChanged))]
        private Color nameTagColour = Color.white;
        public string PlayerName => playerName;

        public override void OnStartClient()
        {
            ApplyName(playerName);
            ApplyColour(nameTagColour);
        }

        public override void OnStartLocalPlayer()
        {
            if (nameTag != null) nameTag.SetActive(false);

            bool steamReady = SteamManager.Initialized;

            string persona = steamReady ? SteamFriends.GetPersonaName() : null;
            if (string.IsNullOrWhiteSpace(persona)) persona = "Player " + netId;

            ulong steamId = steamReady ? SteamUser.GetSteamID().m_SteamID : 0;

            CmdSetIdentity(persona, steamId);
        }

        [Command]
        private void CmdSetIdentity(string requestedName, ulong steamId)
        {
            bool isDev = steamId == developerSteamID;
            bool isTester = !isDev && System.Array.IndexOf(playtesterSteamIDs, steamId) >= 0;

            string clean = Sanitise(requestedName);
            if (clean.Length == 0) clean = "Player " + netId;

            if (isDev)
            {
                playerName = devName;
                nameTagColour = devColour;
            }
            else if (isTester)
            {
                playerName = playtesterPrefix + clean;
                nameTagColour = playtesterColour;
            }
            else
            {
                playerName = clean;
                nameTagColour = Color.white;
            }

            if (isClient)
            {
                ApplyName(playerName);
                ApplyColour(nameTagColour);
            }
        }

        private static string Sanitise(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";

            s = s.Replace("<", "").Replace(">", "").Trim();
            return s.Length > maxNameLength ? s.Substring(0, maxNameLength) : s;
        }

        private void OnNameChanged(string oldName, string newName) => ApplyName(newName);
        private void OnColourChanged(Color oldColour, Color newColour) => ApplyColour(newColour);

        private void ApplyName(string value)
        {
            if (nameText != null) nameText.text = value;
        }

        private void ApplyColour(Color value)
        {
            if (nameText != null) nameText.color = value;
        }
    }
}