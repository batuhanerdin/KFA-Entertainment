using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private PathNode currentNode;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float nodeReachThreshold = 0.2f;
    [SerializeField] private float spreadRadius = 0.5f;

    private Vector3 targetPos;

    // ✅ Stun sistemi
    private bool isStunned = false;
    private float stunTimer = 0f;

    private void Start()
    {
        if (currentNode != null)
            SetTarget(currentNode);
    }

    private void Update()
    {
        HandleStunTimer();
    }

    private void FixedUpdate()
    {
        if (isStunned || currentNode == null) return;

        MoveTowardsTarget();
        CheckNodeReached();
    }

    // === Fonksiyonlar ===
    private void HandleStunTimer()
    {
        if (!isStunned) return;

        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
            isStunned = false;
    }

    private void MoveTowardsTarget()
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.fixedDeltaTime;
    }

    private void CheckNodeReached()
    {
        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist < nodeReachThreshold)
        {
            if (currentNode.nextNodes.Count > 0)
            {
                int index = Random.Range(0, currentNode.nextNodes.Count);
                currentNode = currentNode.nextNodes[index];
                SetTarget(currentNode);
            }
            else
            {
                OnPathCompleted();
            }
        }
    }

    public void SetStartNode(PathNode startNode)
    {
        currentNode = startNode;
        transform.position = startNode.transform.position;
        SetTarget(startNode);
    }

    private void SetTarget(PathNode node)
    {
        Vector3 offset = Random.insideUnitSphere * spreadRadius;
        offset.y = 0;
        targetPos = node.transform.position + offset;
    }

    private void OnPathCompleted()
    {
        // ✅ Obelisk hasar alsın
        if (ObeliskManager.Instance != null)
            ObeliskManager.Instance.TakeDamage(1);

        // ✅ Ses çalsın
        AudioManager.Instance?.PlayObeliskHit();

        // WaveManager’a bildir
        WaveManager wm = FindObjectOfType<WaveManager>();
        if (wm != null)
            wm.OnEnemyRemoved();

        Destroy(gameObject);
    }

    public void ApplyStun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
    }
}
