using UnityEngine;
using DG.Tweening;

public class MerchantCartManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cart;
    [SerializeField] private Transform entryPoint;
    [SerializeField] private Transform stopPoint;
    [SerializeField] private GameObject[] marketObjects;

    [Header("Cart Settings")]
    [SerializeField] private float moveDuration = 3f;
    [SerializeField] private float squashFactor = 0.85f;
    [SerializeField] private float stretchFactor = 1.2f;

    [Header("Market Settings")]
    [SerializeField] private float riseDuration = 1f;
    [SerializeField] private float riseOffset = 2f;

    private Vector3 originalScale;

    private void Awake()
    {
        cart.SetActive(false);
        foreach (var obj in marketObjects)
            obj.SetActive(false);

        if (cart != null)
            originalScale = cart.transform.localScale;
    }

    // ========================
    // ✅ Market açma (wave bitince)
    // ========================
    public void ShowCart()
    {
        if (cart == null) return;

        cart.transform.position = entryPoint.position;
        cart.SetActive(true);

        // 🎵 Cart hareket sesi başlasın
        AudioManager.Instance?.StartCartMovement();

        Sequence seq = DOTween.Sequence();
        seq.Append(cart.transform.DOMove(stopPoint.position, moveDuration)
            .SetEase(Ease.InOutSine));

        cart.transform.DOScale(
            new Vector3(originalScale.x * stretchFactor, originalScale.y * squashFactor, originalScale.z),
            0.3f
        )
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine)
        .SetId("CartSquash");

        seq.OnComplete(() =>
        {
            // 🎵 Cart hareket sesi dursun
            AudioManager.Instance?.StopCartMovement();

            DOTween.Kill("CartSquash");
            cart.transform.localScale = originalScale;

            foreach (var obj in marketObjects)
            {
                obj.SetActive(true);

                Vector3 targetPos = obj.transform.position;
                Vector3 startPos = targetPos - Vector3.up * riseOffset;
                obj.transform.position = startPos;

                obj.transform.DOMoveY(targetPos.y, riseDuration)
                    .SetEase(Ease.OutBack);
            }

            // 🎵 ShopSetup sesi (kurulma)
            AudioManager.Instance?.PlayShopSetup();

            Debug.Log("MerchantCart kuruldu, market açıldı!");
        });
    }

    // ========================
    // ✅ Market kapatma (wave başlayınca)
    // ========================
    public void HideCart()
    {
        if (cart == null) return;

        // Market objelerini yerin içine sok
        foreach (var obj in marketObjects)
        {
            if (!obj.activeSelf) continue;

            Vector3 startPos = obj.transform.position;
            Vector3 targetPos = startPos - Vector3.up * riseOffset;

            obj.transform.DOMoveY(targetPos.y, riseDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => obj.SetActive(false));
        }

        // 🎵 ShopSetup sesi (kapanma)
        AudioManager.Instance?.PlayShopSetup();

        // Cart geri dönsün (sessiz)
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(riseDuration); // önce market kapanışını bekle
        seq.Append(cart.transform.DOMove(entryPoint.position, moveDuration)
            .SetEase(Ease.InOutSine))
            .OnComplete(() =>
            {
                cart.SetActive(false);
                Debug.Log("MerchantCart sahneden çıktı, market kapandı!");
            });
    }
}
