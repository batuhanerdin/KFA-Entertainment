using UnityEngine;
using DG.Tweening;

public class MerchantCartManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cartTransform;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform stopPoint;
    [SerializeField] private Transform shopObjectsParent;
    [SerializeField] private Animator cartAnimator;

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float shopAnimDuration = 1f;
    public float shopAnimOffset = 2f;
    public float cartMoveDelay = 0.4f; // ✅ shop kapandıktan sonra ekstra bekleme

    private bool moving = false;
    private bool goingToStop = false;
    private Vector3[] originalPositions;

    private void Awake()
    {
        InitShopObjects();
        InitCartPosition();
    }

    private void Update()
    {
        HandleCartMovement();
    }

    private void InitShopObjects()
    {
        if (shopObjectsParent != null)
        {
            shopObjectsParent.gameObject.SetActive(false);

            int childCount = shopObjectsParent.childCount;
            originalPositions = new Vector3[childCount];
            for (int i = 0; i < childCount; i++)
                originalPositions[i] = shopObjectsParent.GetChild(i).localPosition;
        }
    }

    private void InitCartPosition()
    {
        if (cartTransform != null && startPoint != null)
            cartTransform.position = startPoint.position;
    }

    private void HandleCartMovement()
    {
        if (!moving) return;

        Transform target = goingToStop ? stopPoint : startPoint;
        MoveCartTowards(target);

        if (ReachedTarget(target))
            OnCartReachedTarget();
    }

    private void MoveCartTowards(Transform target)
    {
        cartTransform.position = Vector3.MoveTowards(
            cartTransform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );
    }

    private bool ReachedTarget(Transform target)
    {
        return Vector3.Distance(cartTransform.position, target.position) < 0.05f;
    }

    private void OnCartReachedTarget()
    {
        moving = false;
        cartAnimator.SetBool("isMoving", false);

        AudioManager.Instance?.StopCartMovement();

        if (goingToStop)
            SetupShop();
        else
            CloseShopImmediate();
    }

    private void SetupShop()
    {
        if (shopObjectsParent != null)
        {
            shopObjectsParent.gameObject.SetActive(true);
            RaiseShopObjects();
        }
    }

    private void CloseShopImmediate()
    {
        if (shopObjectsParent != null)
            shopObjectsParent.gameObject.SetActive(false);
    }

    // === Public API ===
    public void ShowCart()
    {
        if (cartTransform == null || stopPoint == null) return;

        goingToStop = true;
        moving = true;
        cartAnimator.SetBool("isMoving", true);

        AudioManager.Instance?.PlayCartMovement();
    }

    public void HideCart()
    {
        if (cartTransform == null || startPoint == null) return;

        // ✅ Önce market objelerini indir + setup sfx
        if (shopObjectsParent != null && shopObjectsParent.gameObject.activeSelf)
            LowerShopObjects();

        // ✅ Shop animasyonu bittikten 0.4s sonra cart hareket etmeye başlasın
        DOVirtual.DelayedCall(shopAnimDuration + cartMoveDelay, () =>
        {
            goingToStop = false;
            moving = true;
            cartAnimator.SetBool("isMoving", true);

            AudioManager.Instance?.PlayCartMovement();
        });
    }

    // === Shop Objeleri ===
    private void RaiseShopObjects()
    {
        for (int i = 0; i < shopObjectsParent.childCount; i++)
        {
            Transform obj = shopObjectsParent.GetChild(i);
            Vector3 startPos = originalPositions[i] - new Vector3(0, shopAnimOffset, 0);
            obj.localPosition = startPos;
            obj.DOLocalMove(originalPositions[i], shopAnimDuration).SetEase(Ease.OutBack);
        }

        AudioManager.Instance?.PlayShopSetup();
    }

    private void LowerShopObjects()
    {
        for (int i = 0; i < shopObjectsParent.childCount; i++)
        {
            Transform obj = shopObjectsParent.GetChild(i);
            Vector3 targetPos = originalPositions[i] - new Vector3(0, shopAnimOffset, 0);
            obj.DOLocalMove(targetPos, shopAnimDuration).SetEase(Ease.InBack);
        }

        // kapanış tamamlanınca parent disable et
        DOVirtual.DelayedCall(shopAnimDuration, () =>
        {
            if (shopObjectsParent != null)
                shopObjectsParent.gameObject.SetActive(false);
        });

        AudioManager.Instance?.PlayShopSetup();
    }
}
