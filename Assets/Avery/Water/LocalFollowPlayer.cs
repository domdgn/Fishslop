using UnityEngine;
using Mirror;

namespace FishSlop
{
    public class LocalFollowPlayer : MonoBehaviour
    {
        private Transform player;

        private void LateUpdate()
        {
            if (player == null)
            {
                if (NetworkClient.localPlayer == null) return;

                player = NetworkClient.localPlayer.transform;
            }

            Vector3 position = player.position;
            position.y = transform.position.y;

            transform.position = position;
        }
    }
}