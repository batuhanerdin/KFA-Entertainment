using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    // Eventler → UI, GameManager, AudioManager gibi sistemler abone olabilir
    public System.Action OnDeath;
    public System.Action<int, int> OnHealthChanged; // (current, max)

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // === Hasar Alma ===
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // === İyileşme ===
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // === Max Can Arttırma (Upgrade için) ===
    public void IncreaseMaxHealth(int amount, bool healToFull = false)
    {
        maxHealth += amount;

        if (healToFull)
            currentHealth = maxHealth;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // === Ölüm ===
    private void Die()
    {
        Debug.Log($"{gameObject.name} öldü!");
        OnDeath?.Invoke();
    }

    // === Getter Fonksiyonlar ===
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}
