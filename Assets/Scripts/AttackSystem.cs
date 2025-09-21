using UnityEngine;
using System.Collections.Generic;

public class AttackSystem : MonoBehaviour
{
    private StatSystem stats;
    private Animator animator;
    private PlayerMovement playerMovement;

    [Header("Attack Settings")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject attackHitbox;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private Vector3 hitEffectOffset = new Vector3(0f, 1f, 0f); // ✅ Y offset

    private float cooldownTimer = 0f;
    private HashSet<HealthSystem> damagedEnemies = new HashSet<HealthSystem>();

    private void Awake()
    {
        stats = GetComponent<StatSystem>();
        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    private void Update()
    {
        HandleCooldown();
        HandleEnemyDetection();
    }

    private void HandleCooldown()
    {
        cooldownTimer -= Time.deltaTime;
    }

    private void HandleEnemyDetection()
    {
        float attackRange = stats.AttackRange;
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        if (enemiesInRange.Length > 0 && cooldownTimer <= 0f)
        {
            StartAttack(enemiesInRange[0].transform.position);
        }
    }

    private void StartAttack(Vector3 targetPos)
    {
        AudioManager.Instance?.PlayPlayerAttack();
        cooldownTimer = 1f / stats.AttackSpeed;

        Vector3 dir = (targetPos - transform.position).normalized;
        playerMovement.SetFacingDirection(dir);
        playerMovement.LockFlip(true);

        if (animator != null)
            animator.SetTrigger("attack");
    }

    public void EnableHitbox()
    {
        damagedEnemies.Clear();
        if (attackHitbox != null)
            attackHitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    public void AttackEnd()
    {
        playerMovement.LockFlip(false);
    }

    public void ApplyDamage(HealthSystem enemyHealth)
    {
        if (enemyHealth == null || damagedEnemies.Contains(enemyHealth)) return;

        enemyHealth.TakeDamage(stats.AttackPower);
        damagedEnemies.Add(enemyHealth);

        // ✅ Hit effect + offset
        if (hitEffectPrefab != null)
        {
            Vector3 spawnPos = enemyHealth.transform.position + hitEffectOffset;
            GameObject effect = Instantiate(hitEffectPrefab, spawnPos, Quaternion.identity);

            Destroy(effect, 1f); // 1 saniye sonra otomatik yok et
        }
    }

    public void ApplyHitboxUpgrade(float percent)
    {
        if (attackHitbox != null)
            attackHitbox.transform.localScale += attackHitbox.transform.localScale * percent;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (stats != null)
            Gizmos.DrawWireSphere(transform.position, stats.AttackRange);
        else
            Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}
