
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image healthBarFill;

    private void Start()
    {
        if (playerHealth == null || healthBarFill == null)
        {
            Debug.LogError("HealthUI references are missing!");
            return;
        }

        UpdateHealthUI(
            playerHealth.CurrentHealth,
            playerHealth.MaxHealth
        );

        playerHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(float current, float max)
    {
        healthBarFill.fillAmount = current / max;
    }
}
