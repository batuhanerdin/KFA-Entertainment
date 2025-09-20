using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "TowerDefense/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [System.Serializable]
    public class WaveEnemy
    {
        public GameObject enemyPrefab;
        public int enemyCount = 5;
        public float spawnInterval = 1f;
    }

    [Header("Wave Settings")]
    public WaveEnemy[] enemies;

    [Header("Path Settings")]
    public int pathIndex = 0; // PathSystem içinden hangi yol kullanılacak
}
