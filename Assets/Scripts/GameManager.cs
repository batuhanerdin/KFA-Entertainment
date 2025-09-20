using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private WaveManager waveManager;
    private MerchantCartManager cartManager;
    private bool inBreak = false; // mola evresinde miyiz?

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        waveManager = FindObjectOfType<WaveManager>();
        cartManager = FindObjectOfType<MerchantCartManager>();
    }

    private void Start()
    {
        // İlk dalgayı başlat
        if (waveManager.HasMoreWaves())
            waveManager.StartNextWave(); // ✅ düzeltildi
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (inBreak && waveManager.HasMoreWaves())
            {
                inBreak = false;
                if (cartManager != null)
                    cartManager.HideCart(); // marketi kapat
                waveManager.StartNextWave();
                Debug.Log("Yeni wave başlatıldı!");
            }
            else if (!inBreak && waveManager.HasMoreWaves())
            {
                waveManager.StartNextWave(); // ✅ düzeltildi
                Debug.Log("Erken wave başlatıldı!");
            }
            else
            {
                Debug.Log("Başlatılacak wave kalmadı.");
            }
        }
    }

    // ✅ WaveManager sahne tamamen temizlenince çağırır
    public void OnWaveFinished()
    {
        inBreak = true;
        Debug.Log("Dalga bitti! Mola evresi başladı. Market açılıyor...");

        if (cartManager != null)
            cartManager.ShowCart();
    }

    public bool IsInBreak() => inBreak;
}
