using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private WaveManager waveManager;
    private MerchantCartManager cartManager;
    private bool inBreak = false;

    private void Awake()
    {
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
        if (waveManager.HasMoreWaves())
        {
            waveManager.StartNextWave();
            SwitchToWaveMusic();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            HandleWaveKey();
    }

    public void HandleWaveKey()
    {
        if (inBreak && waveManager.HasMoreWaves())
        {
            inBreak = false;

            if (cartManager != null)
                cartManager.HideCart();

            waveManager.StartNextWave();
            SwitchToWaveMusic();
            Debug.Log("Yeni wave başlatıldı!");
        }
        else if (!inBreak && waveManager.HasMoreWaves())
        {
            waveManager.StartNextWave();
            Debug.Log("Erken wave başlatıldı!");
        }
        else
        {
            Debug.Log("Başlatılacak wave kalmadı.");
        }
    }

    public void OnWaveFinished()
    {
        inBreak = true;
        Debug.Log("Dalga bitti! Mola evresi başladı. Market açılıyor...");

        if (cartManager != null)
            cartManager.ShowCart();

        SwitchToChillMusic();
    }

    public bool IsInBreak() => inBreak;

    // === MUSIC HELPERS ===
    private void SwitchToWaveMusic()
    {
        MusicManager.Instance?.PlayWaveMusic();
    }

    private void SwitchToChillMusic()
    {
        MusicManager.Instance?.PlayChillMusic();
    }
}
