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

        [SyncVar(hook = nameof(OnDeadChanged))]
        private bool isDead;

        [SerializeField] private GameObject collision;
        [SerializeField] private GameObject[] hideOnDeath;
        [SerializeField] private Behaviour[] disableOnDeath;
        [SerializeField] private GameObject plankPrefab;
        [SerializeField] private int plankCount = 20;
        [SerializeField] private float explosionSpeed = 8f;

        private Vector3 startPos;
        private Quaternion startRot;

        private Rigidbody rb;
        private void Awake() => rb = GetComponent<Rigidbody>();

        public override void OnStartServer()
        {
            startPos = transform.position;
            startRot = transform.rotation;
        }
        public override void OnStartClient()
        {
            UpdateUI(currentHealth);
            ApplyDead(isDead);
        }

        [Server]
        private void ShipDie()
        {
            SpawnPlanks();

            isDead = true;
            ApplyDead(true);
        }

        [Server]
        private void SpawnPlanks()
        {
            if (plankPrefab == null) return;

            for (int i = 0; i < plankCount; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * 2f;
                GameObject plank = Instantiate(plankPrefab, pos, Random.rotation);
                NetworkServer.Spawn(plank);

                if (plank.TryGetComponent(out Rigidbody prb))
                {
                    prb.linearVelocity = (pos - transform.position).normalized * explosionSpeed + Vector3.up * 4f;
                    prb.angularVelocity = Random.onUnitSphere * 5f;
                }
            }
        }
        private void OnDeadChanged(bool oldValue, bool newValue)
        {
            ApplyDead(newValue);
        }
        private void ApplyDead(bool dead)
        {
            if (collision != null) collision.SetActive(!dead);

            foreach (var go in hideOnDeath) if (go != null) go.SetActive(!dead);

            foreach (var b in disableOnDeath) if (b != null) b.enabled = !dead;

            if (isServer && rb != null)
            {
                if (dead)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }
                else
                {
                    rb.isKinematic = false;
                }
            }
        }

        [Server]
        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
            if (isClient) UpdateUI(currentHealth);

            if (currentHealth <= 0f) ShipDie();
        }

        [Server]
        public void Repair(float amount)
        {
            if (isDead) return;

            currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);

            if (isClient) UpdateUI(currentHealth);
        }

        [Server]
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            if (isClient) UpdateUI(currentHealth);

            if (isDead)
            {
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.position = startPos;
                    rb.rotation = startRot;
                }
                transform.SetPositionAndRotation(startPos, startRot);
                Physics.SyncTransforms();

                if (TryGetComponent(out NetworkTransformBase nt)) nt.RpcTeleport(startPos, startRot);
            }

            isDead = false;
            ApplyDead(false);
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