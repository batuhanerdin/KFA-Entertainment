using UnityEngine;
using System.Collections.Generic;

public class AttackSystem : MonoBehaviour
{
    private StatSystem stats;
    private Animator animator;
    private PlayerMovement playerMovement;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject attackHitbox;

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
        cooldownTimer -= Time.deltaTime;

        // Menzilde düşman var mı?
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        if (enemiesInRange.Length > 0 && cooldownTimer <= 0f)
        {
            StartAttack(enemiesInRange[0].transform.position);
        }
    }

    private void StartAttack(Vector3 targetPos)
    {
        cooldownTimer = 1f / stats.AttackSpeed;

        // Flip yönünü hedefe göre ayarla + kilitle
        Vector3 dir = (targetPos - transform.position).normalized;
        playerMovement.SetFacingDirection(dir);
        playerMovement.LockFlip(true);

        if (animator != null)
            animator.SetTrigger("attack");
    }

    // === Animation Events ===
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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
