using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 3f;

    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab;

    private void Start()
    {
        Destroy(gameObject, lifeTime); // sahnede unutulmasın
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ✅ sadece Player’a çarpar
        {
            HealthSystem hs = other.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.TakeDamage(damage);

                // ✅ Player hasar aldığında özel ses
                AudioManager.Instance?.PlayPlayerHit();
            }

            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }
}
