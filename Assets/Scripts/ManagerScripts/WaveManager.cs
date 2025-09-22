using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveConfig[] waves;
    [SerializeField] private GameObject portal; // ✅ Portal objesi (Inspector’dan atayacaksın)

    private PathSystem pathSystem;
    private GameManager gameManager;

    private int nextWaveIndex = 0;
    private int runningSpawners = 0;
    private int aliveEnemies = 0;

    // KillCounterUI için event
    public event System.Action OnEnemyDeath;

    private void Awake()
    {
        pathSystem = FindObjectOfType<PathSystem>();
        gameManager = FindObjectOfType<GameManager>();

        if (portal != null)
            portal.SetActive(false); // ✅ Başlangıçta kapalı
    }

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
        // ✅ Portal aç
        if (portal != null) portal.SetActive(true);

        int waveEnemyTotal = 0;
        foreach (var e in wave.enemies) waveEnemyTotal += Mathf.Max(0, e.enemyCount);
        aliveEnemies += waveEnemyTotal;

        runningSpawners++;

        foreach (var e in wave.enemies)
        {
            for (int i = 0; i < e.enemyCount; i++)
            {
                SpawnEnemy(e.enemyPrefab, wave.pathIndex);
                yield return new WaitForSeconds(e.spawnInterval);
            }
        }

        runningSpawners--;

        // ✅ Spawn bitince portal kapat
        if (portal != null) portal.SetActive(false);

        MaybeNotifyAllCleared();
    }

    private void SpawnEnemy(GameObject enemyPrefab, int pathIndex)
    {
        PathNode startNode = pathSystem.GetPath(pathIndex);
        if (startNode == null || enemyPrefab == null) return;

        GameObject enemy = Instantiate(enemyPrefab, startNode.transform.position, Quaternion.identity);

        PathFollower follower = enemy.GetComponent<PathFollower>();
        if (follower != null)
            follower.SetStartNode(startNode);

        HealthSystem health = enemy.GetComponent<HealthSystem>();
        if (health != null)
            health.OnDeath += OnEnemyDeathHandler;
    }

    private void OnEnemyDeathHandler()
    {
        // KillCounterUI için event
        OnEnemyDeath?.Invoke();

        OnEnemyRemoved();
    }

    public void OnEnemyRemoved()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        MaybeNotifyAllCleared();
    }

    private void MaybeNotifyAllCleared()
    {
        if (runningSpawners <= 0 && aliveEnemies <= 0)
        {
            gameManager.OnWaveFinished();
        }
    }

    public bool HasActiveEnemies() => aliveEnemies > 0;
    public bool HasMoreWaves() => nextWaveIndex < waves.Length;
}
