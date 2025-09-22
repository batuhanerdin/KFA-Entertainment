using UnityEngine;
using TMPro;

public class KillCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killCountText;

    private int killCount = 0;
    private WaveManager wm;

    private void OnEnable()
    {
        wm = FindObjectOfType<WaveManager>();
        if (wm != null)
            wm.OnEnemyDeath += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        if (wm != null)
            wm.OnEnemyDeath -= HandleEnemyDeath;
    }

    private void Start()
    {
        if (killCountText != null)
            killCountText.text = killCount.ToString();
    }
    private void HandleEnemyDeath()
    {
        killCount++;
        if (killCountText != null)
            killCountText.text = killCount.ToString();
    }
}
