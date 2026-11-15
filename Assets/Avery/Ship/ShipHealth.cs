using Mirror;
using UnityEngine;

namespace FishSlop
{
    public class ShipHealth : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnHealthChanged))]
        public float currentHealth = 100f;
        public float maxHealth = 100f;

        public float impactMulti = 3f;

        [SerializeField] private GameObject collision;
        [SerializeField] private GameObject plankPrefab;
        [SerializeField] private int plankCount = 20;
        [SerializeField] private float explosionSpeed = 8f;
        public override void OnStartClient()
        {
            UpdateUI(currentHealth);
        }

        [Server]
        private void CheckHealth()
        {
            if (currentHealth <= 0f)
            {
                ShipDie();
            }
        }

        [Server]
        private void ShipDie()
        {
            for (int i = 0; i < plankCount; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * 2f;
                GameObject plank = Instantiate(plankPrefab, pos, Random.rotation);
                NetworkServer.Spawn(plank);

                var prb = plank.GetComponent<Rigidbody>();
                prb.linearVelocity = (pos - transform.position).normalized * explosionSpeed + Vector3.up * 4f;
                prb.angularVelocity = Random.onUnitSphere * 5f;
            }

            collision.SetActive(false);
            RpcHide();
            gameObject.SetActive(false);
        }

        [ClientRpc]
        private void RpcHide()
        {
            if (isServer) return;
            collision.SetActive(false);
            gameObject.SetActive(false);
        }

        [ClientRpc]
        private void RpcShow()
        {
            if (isServer) return;
            collision.SetActive(true);
            gameObject.SetActive(true);
        }

        [Server]
        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);

            if (isClient) UpdateUI(currentHealth);
            CheckHealth();
        }

        [Server]
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            if (isClient) UpdateUI(currentHealth);
            RpcShow();
        }
        private void OnHealthChanged(float oldHealth, float newHealth)
        {
            UpdateUI(newHealth);
        }

        private void UpdateUI(float health)
        {
            if (ShipUIHandler.Instance == null) return;

            ShipUIHandler.Instance.SetHealth(health, maxHealth);
        }
    }
}