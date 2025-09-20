using UnityEngine;

public class MerchantCartInteraction : MonoBehaviour
{
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Oyuncu marketin yanında");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Market açıldı deneme");

            // 🎵 ShopOpen sesi (UI açılıyor)
            AudioManager.Instance?.PlayShopOpen();

            // TODO: Burada market UI açma kodu gelecek
        }
    }
}
