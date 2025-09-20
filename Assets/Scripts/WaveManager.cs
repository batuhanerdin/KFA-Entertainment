using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveConfig[] waves;

    private PathSystem pathSystem;
    private GameManager gameManager;

    private int nextWaveIndex = 0;     // sırada başlatılacak wave
    private int runningSpawners = 0;   // şu an spawn eden kaç wave var
    private int aliveEnemies = 0;      // sahnedeki toplam düşman

    private void Awake()
    {
        pathSystem = FindObjectOfType<PathSystem>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Her çağrıda sıradaki dalgayı BAŞLATIR (aktif dalga varken de çağrılabilir)
    public void StartNextWave()
    {
        if (nextWaveIndex >= waves.Length)
        {
            Debug.Log("Başlatılacak wave kalmadı.");
            return;
        }

        int startedWaveIndex = nextWaveIndex;
        nextWaveIndex++;

        StartCoroutine(RunWave(waves[startedWaveIndex], startedWaveIndex));
    }

    private IEnumerator RunWave(WaveConfig wave, int waveIndex)
    {
        // Bu wave için toplam düşman sayısını aliveEnemies'e ekle
        int waveEnemyTotal = 0;
        foreach (var e in wave.enemies) waveEnemyTotal += Mathf.Max(0, e.enemyCount);
        aliveEnemies += waveEnemyTotal;

        runningSpawners++;

        // Spawn sırası (bu wave için)
        foreach (var e in wave.enemies)
        {
            for (int i = 0; i < e.enemyCount; i++)
            {
                SpawnEnemy(e.enemyPrefab, wave.pathIndex);
                yield return new WaitForSeconds(e.spawnInterval);
            }
        }

        // Bu wave’in spawn’ı tamamlandı
        runningSpawners--;
        MaybeNotifyAllCleared();
    }

    private void SpawnEnemy(GameObject enemyPrefab, int pathIndex)
    {
        PathNode startNode = pathSystem.GetPath(pathIndex);
        if (startNode == null || enemyPrefab == null) return;

        GameObject enemy = Instantiate(enemyPrefab, startNode.transform.position, Quaternion.identity);

        // Path
        PathFollower follower = enemy.GetComponent<PathFollower>();
        if (follower != null)
            follower.SetStartNode(startNode);

        // Health → ölünce WaveManager’a haber ver
        HealthSystem health = enemy.GetComponent<HealthSystem>();
        if (health != null)
            health.OnDeath += OnEnemyDeath;
    }

    public void OnEnemyDeath()
    {
        OnEnemyRemoved();
    }

    // Kaçarak sona ulaşanlar da buradan eksiltilir (PathFollower çağırır)
    public void OnEnemyRemoved()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        MaybeNotifyAllCleared();
    }

    private void MaybeNotifyAllCleared()
    {
        // Sahne tamamen temizlendi mi? (Spawn eden wave yok & canlı düşman yok)
        if (runningSpawners <= 0 && aliveEnemies <= 0)
        {
            gameManager.OnWaveFinished(); // mola evresi
        }
    }

    // UI / kontrol için
    public bool HasActiveEnemies() => aliveEnemies > 0;
    public bool HasMoreWaves() => nextWaveIndex < waves.Length;
}
