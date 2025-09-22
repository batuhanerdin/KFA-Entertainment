using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [SerializeField] private int startingCoins = 0; // oyun baþýnda coin
    [SerializeField] private int currentCoins; // mevcut coin

    public event Action<int> OnBalanceChanged; // bakiye deðiþtiðinde çaðrýlýr

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentCoins = startingCoins;
        InvokeChange();
    }

    public int Coins => currentCoins; // sadece okunabilir coin deðeri

    public bool CanAfford(int amount) => currentCoins >= amount; // parasý yeter mi

    public bool TrySpend(int amount) // harcama
    {
        if (amount <= 0) return false;
        if (!CanAfford(amount)) return false;

        currentCoins -= amount;
        InvokeChange();
        return true;
    }

    public void AddCoins(int amount) // coin ekleme
    {
        if (amount == 0) return;
        currentCoins += amount;
        InvokeChange();
    }

    public void SetCoins(int amount) // coin doðrudan ayarla
    {
        currentCoins = Mathf.Max(0, amount);
        InvokeChange();
    }

    public void ResetToStarting() // sýfýrla
    {
        currentCoins = startingCoins;
        InvokeChange();
    }

    private void InvokeChange()
    {
        OnBalanceChanged?.Invoke(currentCoins);
    }
}
