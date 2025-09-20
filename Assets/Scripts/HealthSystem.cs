using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    private Animator animator;
    private bool isDead = false;

    public System.Action OnDeath;
    public System.Action<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        AudioManager.Instance.PlayHit();

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth > 0)
        {
            // ✅ Hit animasyonu + ses
            if (animator != null)
                animator.SetTrigger("hit");

            AudioManager.Instance?.PlayHit();
        }
        else
        {
            // ✅ Ölüm
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        AudioManager.Instance.PlayDie();

        Debug.Log($"{gameObject.name} öldü!");
        OnDeath?.Invoke();

        if (animator != null)
            animator.SetTrigger("die");

        AudioManager.Instance?.PlayDie();

        // 1 saniye sonra kapat
        Invoke(nameof(DisableObject), 1f);
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void IncreaseMaxHealth(int amount, bool healToFull = false)
    {
        maxHealth += amount;
        if (healToFull)
            currentHealth = maxHealth;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}
