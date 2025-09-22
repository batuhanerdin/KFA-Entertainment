using UnityEngine;

public class WaveCallObelisk : MonoBehaviour
{
    [SerializeField] private GameObject pressEIndicator;
    private bool playerInRange = false;
    private GameManager gameManager;

    private void Awake()
    {
        if (pressEIndicator != null)
            pressEIndicator.SetActive(false);
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressEIndicator != null)
                pressEIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressEIndicator != null)
                pressEIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (gameManager != null)
            {
                gameManager.HandleWaveKey(); // ✅ Artık GameManager’daki fonksiyonu çağırıyor
                if (pressEIndicator != null)
                    pressEIndicator.SetActive(false);
            }
        }
    }
}
