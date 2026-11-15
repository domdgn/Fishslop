using UnityEngine;
using UnityEngine.UI;

namespace FishSlop
{
    public class ShipUIHandler : MonoBehaviour
    {
        public static ShipUIHandler Instance { get; private set; }

        [SerializeField] private Slider healthBar;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void SetHealth(float currentHealth, float maxHealth)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }
}