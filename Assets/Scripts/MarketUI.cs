using UnityEngine;

public class MarketUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatSystem playerStats;
    [SerializeField] private HealthSystem playerHealth;

    [Header("Upgrade Costs")]
    [SerializeField] private int attackPowerCost = 10;
    [SerializeField] private int attackSpeedCost = 15;
    [SerializeField] private int moveSpeedCost = 20;
    [SerializeField] private int attackRangeCost = 25;
    [SerializeField] private int maxHealthCost = 30;

    [Header("Upgrade Values")]
    [SerializeField] private int attackPowerIncrease = 5;     // +5 attack power
    [SerializeField] private float moveSpeedIncrease = 0.5f;  // +0.5 hýz
    [SerializeField] private float attackSpeedPercent = 0.1f; // %10
    [SerializeField] private float attackRangePercent = 0.1f; // %10
    [SerializeField] private int healthIncreaseAmount = 25;   // +25 Max Health

    private void Awake()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<StatSystem>();
        if (playerHealth == null)
            playerHealth = FindObjectOfType<HealthSystem>();
    }

    public void BuyAttackPower()
    {
        if (CoinManager.Instance.TrySpend(attackPowerCost))
        {
            playerStats.AddAttackPower(attackPowerIncrease);
            Debug.Log($"+{attackPowerIncrease} Attack Power alýndý!");
        }
        else Debug.Log("Yetersiz coin!");
    }

    public void BuyMoveSpeed()
    {
        if (CoinManager.Instance.TrySpend(moveSpeedCost))
        {
            playerStats.AddMoveSpeed(moveSpeedIncrease);
            Debug.Log($"+{moveSpeedIncrease} Move Speed alýndý!");
        }
        else Debug.Log("Yetersiz coin!");
    }

    public void BuyAttackSpeed()
    {
        if (CoinManager.Instance.TrySpend(attackSpeedCost))
        {
            playerStats.IncreaseAttackSpeed(attackSpeedPercent);
            Debug.Log("Attack Speed upgrade alýndý!");
        }
        else Debug.Log("Yetersiz coin!");
    }

    public void BuyAttackRange()
    {
        if (CoinManager.Instance.TrySpend(attackRangeCost))
        {
            playerStats.IncreaseAttackRange(attackRangePercent);
            Debug.Log("Attack Range upgrade alýndý!");
        }
        else Debug.Log("Yetersiz coin!");
    }

    public void BuyMaxHealth()
    {
        if (CoinManager.Instance.TrySpend(maxHealthCost))
        {
            if (playerHealth != null)
            {
                playerHealth.IncreaseMaxHealth(healthIncreaseAmount, healToFull: false);
                playerHealth.Heal(healthIncreaseAmount);
                Debug.Log($"+{healthIncreaseAmount} Max Health alýndý!");
            }
        }
        else Debug.Log("Yetersiz coin!");
    }
}
