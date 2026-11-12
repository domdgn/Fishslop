using Mirror;
using UnityEngine;
using UnityEngine.Audio;

namespace FishSlop
{
    public class NoseBoop : NetworkBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        [Server]
        public void PlaySound()
        {
            RpcPlaySound();
        }

        [ClientRpc]
        private void RpcPlaySound()
        {
            audioSource.Play();
        }
    }
}