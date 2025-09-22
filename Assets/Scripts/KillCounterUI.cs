using UnityEngine;
using TMPro;

public class KillCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killCountText;

    private int killCount = 0;

    private void Start()
    {
        if (killCountText != null)
            killCountText.text = killCount.ToString();
    }

    private void OnEnable()
    {
        var wm = FindObjectOfType<WaveManager>();
        if (wm != null)
            wm.OnEnemyDeath += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        var wm = FindObjectOfType<WaveManager>();
        if (wm != null)
            wm.OnEnemyDeath -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        killCount++;
        if (killCountText != null)
            killCountText.text = killCount.ToString();
    }
}
