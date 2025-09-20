using UnityEngine;
using System.Collections;

public class MerchantCartManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cart;            // MerchantCart objesi
    [SerializeField] private Transform entryPoint;       // sahneye gireceði nokta (sol/sað)
    [SerializeField] private Transform stopPoint;        // çitlerin önündeki durma noktasý
    [SerializeField] private GameObject[] marketObjects; // ateþ, balta, fýrýn vs.

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 2f;

    private bool isActive = false;

    private void Awake()
    {
        cart.SetActive(false);
        foreach (var obj in marketObjects)
            obj.SetActive(false);
    }

    public void ShowCart()
    {
        StartCoroutine(CartRoutine());
    }

    private IEnumerator CartRoutine()
    {
        // Cart'ý sahneye sok
        cart.transform.position = entryPoint.position;
        cart.SetActive(true);
        isActive = true;

        while (Vector3.Distance(cart.transform.position, stopPoint.position) > 0.1f)
        {
            cart.transform.position = Vector3.MoveTowards(
                cart.transform.position,
                stopPoint.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        cart.transform.position = stopPoint.position;

        // Market objelerini aç
        foreach (var obj in marketObjects)
            obj.SetActive(true);

        Debug.Log("MerchantCart yerine geldi, market açýldý!");
    }
}
