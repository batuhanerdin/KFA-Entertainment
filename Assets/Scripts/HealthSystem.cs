using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Reactions")]
    [SerializeField] private float stunOnHitDuration = 0.5f;
    [SerializeField] private float disableDelayOnDeath = 1f;

    private Animator animator;
    private Collider enemyCollider;
    private bool isDead = false;

    // Eventler
    public System.Action OnDeath; // WaveManager ve KillCounterUI dinleyecek
    public System.Action<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        enemyCollider = GetComponent<Collider>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        var path = GetComponent<PathFollower>();
        if (path != null) path.ApplyStun(stunOnHitDuration);

        if (currentHealth > 0)
        {
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

        if (animator != null) animator.SetTrigger("die");
        AudioManager.Instance?.PlayDie();

        if (enemyCollider != null)
            enemyCollider.enabled = false;

        var path = GetComponent<PathFollower>();
        if (path != null) path.enabled = false;

        var dropper = GetComponent<ObjectDropper>();
        if (dropper != null) dropper.DropObjects();

        // ✅ Ölüm eventini fırlat
        OnDeath?.Invoke();

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
