using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthSystem playerHealth;
    [SerializeField] private Image healthFill;        // Fill Image (UI)
    [SerializeField] private TextMeshProUGUI healthText; // "100/100" yazýsý

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<HealthSystem>();

        if (playerHealth != null)
        {
            // Event’e abone ol
            playerHealth.OnHealthChanged += UpdateUI;
            // Baþlangýç güncelle
            UpdateUI(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateUI;
    }

    private void UpdateUI(int current, int max)
    {
        if (healthFill != null)
            healthFill.fillAmount = (float)current / max;

        if (healthText != null)
            healthText.text = $"{current}/{max}";
    }
}
