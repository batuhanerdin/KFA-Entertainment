using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Reactions")]
    [SerializeField] private float stunOnHitDuration = 0.5f; // hasar alınca stun süresi
    [SerializeField] private float disableDelayOnDeath = 1f; // ölünce kaç saniye sonra kapanacak

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

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // hasar alınca kısa stun
        var path = GetComponent<PathFollower>();
        if (path != null) path.ApplyStun(stunOnHitDuration);

        if (currentHealth > 0)
        {
            // ✅ doğru tetik adları
            if (animator != null) animator.SetTrigger("hit");
            AudioManager.Instance?.PlayHit();
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null) animator.SetTrigger("die"); // ✅ doğru tetik
        AudioManager.Instance?.PlayDie();

        // hareketi hemen durdur
        var path = GetComponent<PathFollower>();
        if (path != null) path.enabled = false;

        OnDeath?.Invoke();

        // 1 sn sonra gizle (pool için uygun)
        Invoke(nameof(DisableObject), disableDelayOnDeath);
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void IncreaseMaxHealth(int amount, bool healToFull = false)
    {
        maxHealth += amount;
        if (healToFull) currentHealth = maxHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}
