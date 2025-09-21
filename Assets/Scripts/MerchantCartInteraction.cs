using UnityEngine;

public class MerchantCartInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject marketUI;       // Market paneli (başta kapalı)
    [SerializeField] private GameObject interactionHint; // "E'ye bas" göstergesi (başta kapalı)

    private bool playerInRange = false;

    private void Start()
    {
        if (marketUI != null)
            marketUI.SetActive(false);

        if (interactionHint != null)
            interactionHint.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionHint != null)
                interactionHint.SetActive(true);

            Debug.Log("Oyuncu marketin yanında");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionHint != null)
                interactionHint.SetActive(false);

            // Oyuncu uzaklaşınca marketi otomatik kapatmak istersen:
            if (marketUI != null)
                marketUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (marketUI != null)
            {
                bool isActive = marketUI.activeSelf;
                marketUI.SetActive(!isActive);

                if (!isActive)
                {
                    Debug.Log("Market açıldı");
                    AudioManager.Instance?.PlayShopOpen();
                }
                else
                {
                    Debug.Log("Market kapatıldı");
                }
            }
        }
    }
}
