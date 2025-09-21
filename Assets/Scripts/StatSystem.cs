using UnityEngine;

public class StatSystem : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int attackPower = 10;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackRange = 0.4f;

    public float MoveSpeed => moveSpeed;
    public int AttackPower => attackPower;
    public float AttackSpeed => attackSpeed;
    public float AttackRange => attackRange;

    // --- Upgrade metodları ---
    public void AddMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

    public void AddAttackPower(int amount)
    {
        attackPower += amount;
    }

    public void IncreaseAttackSpeed(float percent)
    {
        attackSpeed += attackSpeed * percent;
    }

    // ✅ Artık StatSystem’in attackRange’ini büyütüyor
    public void IncreaseAttackRange(float percent)
    {
        float added = attackRange * percent;
        attackRange += added;

        // Hitbox boyutunu da güncelle
        var attackSystem = GetComponent<AttackSystem>();
        if (attackSystem != null)
            attackSystem.ApplyHitboxUpgrade(percent);
    }
}
