using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System.Collections;

namespace FishSlop
{
    public class SteamLobby : MonoBehaviour
    {
        public static SteamLobby Instance { get; private set; }
        public GameObject hostButton = null;
        public ulong lobbyID;
        public NetworkManager networkManager;
        public PanelSwapper panelSwapper;

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;
        protected Callback<LobbyChatUpdate_t> lobbyChatUpdate;

        private const string HostAddressKey = "HostAddress";
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            networkManager = GetComponent<NetworkManager>();
            if (!SteamManager.Initialized)
            {
                Debug.LogWarning("SteamManager reports NOT initialized. That is NOT a docking station. Attempting manual override...");
                try
                {
                    bool success = SteamAPI.Init();
                    Debug.Log($"Manual SteamAPI.Init fallback result: {success}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Hard crash trying to run SteamAPI.Init: {e.Message}");
                }
            }
            else
            {
                Debug.Log("Steam successfully initialized on Start!");
            }

            panelSwapper.gameObject.SetActive(true);
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        }

        public void HostLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
        }

        void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogError("Cannot host: Steam API is not running!");
                return;
            }

            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("Failed to create lobby: " + callback.m_eResult);
                return;
            }

            Debug.Log("Lobby created successfully!" + callback.m_ulSteamIDLobby);
            networkManager.StartHost();

            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
            lobbyID = callback.m_ulSteamIDLobby;
        }

        void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("Join request received for lobby: " + callback.m_steamIDLobby);
            
            if (NetworkClient.isConnected || NetworkClient.active)
            {
                Debug.LogWarning("Already connected to a lobby. Disconnecting first.");
                NetworkManager.singleton.StopClient();
                NetworkClient.Shutdown();
            }

            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        void OnLobbyEntered(LobbyEnter_t callback)
        {
            if (NetworkServer.active)
            {
                Debug.Log("Already in lobby as host. Ignoring join request.");
                return;
            }

            lobbyID = callback.m_ulSteamIDLobby;
            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
            networkManager.networkAddress = hostAddress;

            networkManager.StartClient();
            panelSwapper.SwapUIPanel("Lobby");
        }

        void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            if (callback.m_ulSteamIDLobby != lobbyID) { return; }

            EChatMemberStateChange stateChange = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;
            Debug.Log($"LobbyChatUpdate: {stateChange} for user {callback.m_ulSteamIDUserChanged}");

            bool shouldUpdate = stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeEntered) ||
                                stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeLeft) ||
                                stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeDisconnected) ||
                                stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeKicked) ||
                                stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeBanned);

            if (shouldUpdate)
            {
                StartCoroutine(DelayedNameUpdate(0.5f));
                LobbyUIManager.Instance?.CheckAllPlayersReady();
            }
        }

        private IEnumerator DelayedNameUpdate(float delay)
        {
            if (LobbyUIManager.Instance == null)
            {
                Debug.LogWarning("LobbyUIManager instance is null. Cannot update player lobby UI.");
                yield break;
            }

            yield return new WaitForSeconds(delay);
            LobbyUIManager.Instance?.UpdatePlayerLobbyUI();
        }

        public void LeaveLobby()
        {
            CSteamID currentOwner = SteamMatchmaking.GetLobbyOwner(new CSteamID(lobbyID));
            CSteamID local = SteamUser.GetSteamID();
            
            var lobby = new CSteamID(lobbyID);
            List<CSteamID> members = new List<CSteamID>();

            int count = SteamMatchmaking.GetNumLobbyMembers(lobby);

            for (int i = 0; i < count; i++)
            {
                members.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobby, i));
            }

            if (lobbyID != 0)
            {
                SteamMatchmaking.LeaveLobby(new CSteamID(lobbyID));
                Debug.Log("Left lobby");
                lobbyID = 0;
            }
            else
            {
                Debug.LogWarning("Not in a lobby to leave.");
            }

            if (NetworkServer.active && currentOwner == local)
            {
                networkManager.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                networkManager.StopClient();
            }

            panelSwapper.gameObject.SetActive(true);
            this.gameObject.SetActive(true);
        }
    }
}
