using Mirror;
using UnityEngine;

namespace FishSlop
{
    public class ShipHealth : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnHealthChanged))]
        [SerializeField] private float currentHealth = 100f;

        [SerializeField] private float maxHealth = 100f;
        public float impactMulti = 3f;
        public override void OnStartClient()
        {
            UpdateUI(currentHealth);
        }

        [Server]
        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        }

        [Server]
        private void CheckHealth()
        {
            if (currentHealth <= 0f)
            {
                Debug.Log("Ship died rip");
                Destroy(gameObject, 1f);
            }
        }

        private void OnHealthChanged(float oldHealth, float newHealth)
        {
            CheckHealth();
            UpdateUI(newHealth);
        }

        private void UpdateUI(float health)
        {
            if (ShipUIHandler.Instance == null) return;

            ShipUIHandler.Instance.SetHealth(health, maxHealth);
        }
    }
}