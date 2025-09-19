using UnityEngine;

public class HitboxSensor : MonoBehaviour
{
    private AttackSystem attackSystem;

    private void Awake()
    {
        attackSystem = GetComponentInParent<AttackSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
        if (enemyHealth != null)
        {
            attackSystem.ApplyDamage(enemyHealth);
        }
    }
}
