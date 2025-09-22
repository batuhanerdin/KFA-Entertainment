using UnityEngine;
using TMPro;

public class ObeliskManager : MonoBehaviour
{
    public static ObeliskManager Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateUI();

        // ✅ Ses efekti
        AudioManager.Instance?.PlayObeliskHit();

        if (currentHealth <= 0)
        {
            Debug.Log("❌ Obelisk yok oldu! Game Over!");
            // TODO: GameManager.GameOver() çağrısı buraya eklenebilir
        }
    }

    private void UpdateUI()
    {
        if (healthText != null)
            healthText.text = currentHealth + "/" + maxHealth;
    }
}
