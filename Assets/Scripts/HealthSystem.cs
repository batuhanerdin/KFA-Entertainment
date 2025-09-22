using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Reactions")]
    [SerializeField] private float stunOnHitDuration = 0.5f;
    [SerializeField] private float disableDelayOnDeath = 1f;

    [Header("Player Only Settings")]
    [SerializeField] private bool isPlayer = false;       // ✅ sadece Player için
    [SerializeField] private float playerStunDuration = 0.2f;
    [SerializeField] private float iFrameDuration = 0.8f;
    [SerializeField] private float blinkInterval = 0.1f;  // yanıp sönme hızı

    private Animator animator;
    private Collider enemyCollider;
    private bool isDead = false;

    // Player özel değişkenler
    private bool isInvincible = false;
    private SpriteRenderer playerSprite; // sadece Player’da var
    private PlayerMovement playerMovement;
    private AttackSystem playerAttack;

    // Eventler
    public System.Action OnDeath;
    public System.Action<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        enemyCollider = GetComponent<Collider>();

        if (isPlayer)
        {
            playerSprite = GetComponentInChildren<SpriteRenderer>();
            playerMovement = GetComponent<PlayerMovement>();
            playerAttack = GetComponent<AttackSystem>();
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (isPlayer && isInvincible) return; // ✅ I-frame aktifken hasar yeme

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (isPlayer)
        {
            // Stun ve I-frame başlat
            StartCoroutine(PlayerStunRoutine());
            StartCoroutine(InvincibilityRoutine());
            AudioManager.Instance?.PlayPlayerHit(); // ✅ sadece Player hit sesi
        }
        else
        {
            var path = GetComponent<PathFollower>();
            if (path != null) path.ApplyStun(stunOnHitDuration);
            AudioManager.Instance?.PlayHit();
        }

        if (currentHealth > 0)
        {
            if (animator != null) animator.SetTrigger("hit");
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

    // === PLAYER ONLY ROUTINES ===
    private IEnumerator PlayerStunRoutine()
    {
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerAttack != null) playerAttack.enabled = false;

        yield return new WaitForSeconds(playerStunDuration);

        if (!isDead)
        {
            if (playerMovement != null) playerMovement.enabled = true;
            if (playerAttack != null) playerAttack.enabled = true;
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        float timer = 0f;
        bool visible = true;

        while (timer < iFrameDuration)
        {
            if (playerSprite != null)
            {
                visible = !visible;
                playerSprite.color = visible ? Color.white : new Color(1f, 0f, 0f, 0.5f); // kırmızı/yarı saydam
            }

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        if (playerSprite != null)
            playerSprite.color = Color.white;

        isInvincible = false;
    }
}
