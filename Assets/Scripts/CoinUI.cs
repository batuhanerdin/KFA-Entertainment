using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void Start()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.OnBalanceChanged -= UpdateUI;
    }

    private void TrySubscribe()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnBalanceChanged -= UpdateUI; // tekrar eklenmesin
            CoinManager.Instance.OnBalanceChanged += UpdateUI;
            UpdateUI(CoinManager.Instance.Coins);
        }
    }

    private void UpdateUI(int newAmount)
    {
        if (coinText != null)
            coinText.text = newAmount.ToString();
    }
}
