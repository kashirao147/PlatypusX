/// <summary>
/// This object is UI, and it will be shown every time the player changes its health and will be hidden after a delay time.
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace PhoenixaStudio
{
    public class HealthPercent : MonoBehaviour
    {
        // Showing time
        public float delay = 1.5f;
        public Vector2 offset;

        // Health bar and text UI elements
        public Slider healthBar; // Assign this in the Inspector
        public CanvasGroup healthUI; // For fading the health UI in/out
        Text healthPercent;

        // Time and health tracking
        private float time;
        private int priHealth;
        private int originalHealth;

        void Start()
        {
            // Get the text object
            healthPercent = GetComponent<Text>();

            // Initialize health values
            priHealth = GameManager.Instance.Player.health;
            originalHealth = priHealth;

            // Initialize health UI
            if (healthBar != null)
            {
                healthBar.maxValue = originalHealth;
                healthBar.value = priHealth;
            }

            if (healthUI != null)
            {
                healthUI.alpha = 0; // Start hidden
            }
        }

        void Update()
        {
            // Follow the player
            transform.position = GameManager.Instance.Player.transform.position + (Vector3)offset;

            time += Time.deltaTime;
            if (time >= delay || GameManager.Instance.Player.health <= 0)
            {
                if (healthUI != null)
                    healthUI.alpha = Mathf.Lerp(healthUI.alpha, 0, Time.deltaTime * 5); // Smooth fade-out

                healthPercent.enabled = false;
            }

            if (priHealth != GameManager.Instance.Player.health)
            {
                time = 0;

                // Update health UI
                if (healthUI != null)
                    healthUI.alpha = 1; // Show UI immediately

                healthPercent.enabled = true;

                // Update percentage text
                healthPercent.text = (((float)GameManager.Instance.Player.health / (float)originalHealth) * 100).ToString("0") + "%";

                // Update health bar
                if (healthBar != null)
                    healthBar.value = GameManager.Instance.Player.health;
            }

            // Update previous health value
            priHealth = GameManager.Instance.Player.health;
        }
    }
}
