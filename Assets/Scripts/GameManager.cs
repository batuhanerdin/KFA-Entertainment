using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private WaveManager waveManager;
    private bool inBreak = false; // mola evresi

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        waveManager = FindObjectOfType<WaveManager>();
    }

    private void Start()
    {
        // İstersen ilk dalgayı otomatik başlat
        if (waveManager.HasMoreWaves())
            waveManager.StartNextWave();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // İSTENEN DAVRANIŞ:
            // P tuşu HER ZAMAN sıradaki dalgayı başlatır (aktif dalga varken de).
            if (waveManager.HasMoreWaves())
            {
                inBreak = false; // mola iptal
                waveManager.StartNextWave();
            }
            else
            {
                Debug.Log("Tüm wave'ler başlatıldı.");
            }
        }
    }

    // WaveManager sahne temizlendiğinde çağırır
    public void OnWaveFinished()
    {
        Debug.Log("Dalga bitti! Mola evresi başladı.");

        //waitingForNextWave = true;

        // ✅ MerchantCart’ı çağır
        var cartManager = FindObjectOfType<MerchantCartManager>();
        if (cartManager != null)
            cartManager.ShowCart();
    }
}
