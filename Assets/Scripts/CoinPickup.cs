using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private int coinValue = 1;

    public void SetValue(int value) => coinValue = value;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CoinManager.Instance != null)
                CoinManager.Instance.AddCoins(coinValue);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayCoinPickup();

            Destroy(gameObject);
        }
    }
}
