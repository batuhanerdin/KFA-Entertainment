using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("General Attack Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField][Range(0f, 1f)] private float attackChance = 0.3f;
    [SerializeField] private float attackDelay = 0.5f;

    [Header("Projectile Attack")]
    [SerializeField] private bool meleeAttack = false;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 3f;

    [Header("Melee Attack")]
    [SerializeField] private float meleeDashDistance = 3f;
    [SerializeField] private float meleeDashSpeed = 10f;
    [SerializeField] private float meleeHitRange = 1f;
    [SerializeField] private int meleeDamage = 10;
    [SerializeField] private LayerMask playerLayer;

    private Transform player;
    private Animator animator;
    private PathFollower pathFollower;

    private float cooldownTimer = 0f;
    private bool isAttacking = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        pathFollower = GetComponent<PathFollower>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        if (player == null || isAttacking) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            TryAttack();
            cooldownTimer = attackCooldown;
        }
    }

    private void TryAttack()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            if (Random.value <= attackChance)
            {
                StartAttack();
            }
        }
    }

    private void StartAttack()
    {
        isAttacking = true;

        if (pathFollower != null) pathFollower.enabled = false;
        if (animator != null) animator.SetTrigger("attack");

        Invoke(nameof(PerformAttack), attackDelay);
    }

    private void PerformAttack()
    {
        if (meleeAttack)
            PerformMeleeAttack();
        else
            PerformRangedAttack();
    }

    private void PerformRangedAttack()
    {
        if (projectilePrefab != null && player != null && firePoint != null)
        {
            Vector3 dir = (player.position - firePoint.position).normalized;

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = dir * projectileSpeed;
            }

            Destroy(proj, projectileLifetime);
        }

        EndAttack();
    }

    private void PerformMeleeAttack()
    {
        if (player == null) { EndAttack(); return; }

        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 targetPos = transform.position + dir * meleeDashDistance;

        StartCoroutine(MeleeDash(targetPos));
    }

    private System.Collections.IEnumerator MeleeDash(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, meleeDashSpeed * Time.deltaTime);
            yield return null;
        }

        // Dash bitti → player yakın mı kontrol et
        Collider[] hits = Physics.OverlapSphere(transform.position, meleeHitRange, playerLayer);
        foreach (Collider hit in hits)
        {
            HealthSystem hs = hit.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.TakeDamage(meleeDamage);

                // ✅ Player’a özel vurma sesi
                AudioManager.Instance?.PlayPlayerHit();
            }
        }

        EndAttack();
    }

    private void EndAttack()
    {
        isAttacking = false;
        if (pathFollower != null) pathFollower.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(firePoint.position, 0.1f);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeHitRange);
    }
}
