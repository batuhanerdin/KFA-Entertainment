using UnityEngine;

public class ObjectDropper : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoinValue = 1;
    [SerializeField] private int maxCoinValue = 10;

    [Header("Coin Sprites")]
    [SerializeField] private Sprite lowValueSprite;   // 1–2
    [SerializeField] private Sprite midValueSprite;   // 3–5
    [SerializeField] private Sprite highValueSprite;  // 6–10

    [Header("Death Particle")]
    [SerializeField] private GameObject deathParticle;

    [SerializeField] private Transform dropPoint;

    public void DropObjects()
    {
        Vector3 pos = dropPoint != null ? dropPoint.position : transform.position;

        // === Coin ===
        if (coinPrefab != null)
        {
            int value = Random.Range(minCoinValue, maxCoinValue + 1);

            GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity);

            // Sprite seç
            SpriteRenderer sr = coin.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (value <= 2) sr.sprite = lowValueSprite;
                else if (value <= 5) sr.sprite = midValueSprite;
                else sr.sprite = highValueSprite;
            }

            // CoinPickup’a deðer setle
            CoinPickup pickup = coin.GetComponent<CoinPickup>();
            if (pickup != null) pickup.SetValue(value);
        }

        // === Particle ===
        if (deathParticle != null)
        {
            GameObject particle = Instantiate(deathParticle, pos, Quaternion.identity);
            Destroy(particle, 1f);
        }
    }
}
