using UnityEngine;

public class StatSystem : MonoBehaviour
{
    [Header("Base Stats (Default Değerler)")]
    [SerializeField] private int baseAttackPower = 10;
    [SerializeField] private float baseAttackSpeed = 1f;
    [SerializeField] private float baseMoveSpeed = 5f;

    [Header("Current Stats (Runtime Değerler)")]
    [SerializeField] private int attackPower;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float moveSpeed;

    // Public getter → başka scriptler okuyabilir ama değiştiremez
    public int AttackPower => attackPower;
    public float AttackSpeed => attackSpeed;
    public float MoveSpeed => moveSpeed;

    private void Awake()
    {
        ResetStats();
    }

    // === Statları base değerlerine sıfırla ===
    public void ResetStats()
    {
        attackPower = baseAttackPower;
        attackSpeed = baseAttackSpeed;
        moveSpeed = baseMoveSpeed;
    }

    // === Upgrade Fonksiyonları ===
    public void UpgradeAttack(int amount)
    {
        attackPower += amount;
    }

    public void UpgradeAttackSpeed(float multiplier)
    {
        attackSpeed *= multiplier;
    }

    public void UpgradeMoveSpeed(float multiplier)
    {
        moveSpeed *= multiplier;
    }
}
