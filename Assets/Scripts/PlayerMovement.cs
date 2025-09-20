using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 moveInput;

    private StatSystem stats;
    private Animator animator;
    private bool flipLocked = false;
    private Transform spriteTransform;

    [Header("Footstep Settings")]
    [SerializeField] private float footstepInterval = 0.4f;
    private float footstepTimer;
    private int footstepIndex = 0; // ✅ kaldığı yerden devam etsin

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        stats = GetComponent<StatSystem>();
        animator = GetComponentInChildren<Animator>();
        spriteTransform = animator.transform;
    }

    private void Update()
    {
        HandleInput();
        HandleAnimations();
        HandleSpriteFlip();
        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;
    }

    private void HandleMovement()
    {
        float moveSpeed = stats.MoveSpeed;
        Vector3 moveVelocity = moveInput * moveSpeed;
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    private void HandleAnimations()
    {
        bool isMoving = moveInput.magnitude > 0.1f;
        if (animator != null)
        {
            animator.SetBool("isRunning", isMoving);
        }
    }

    private void HandleSpriteFlip()
    {
        if (flipLocked) return;

        Vector3 scale = spriteTransform.localScale;

        if (moveInput.x > 0.1f)
            spriteTransform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        else if (moveInput.x < -0.1f)
            spriteTransform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
    }

    private void HandleFootsteps()
    {
        bool isMoving = moveInput.magnitude > 0.1f;

        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayNextFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f; // durunca resetlenmesin, kaldığı yerden devam etsin
        }
    }

    private void PlayNextFootstep()
    {
        if (AudioManager.Instance == null) return;
        if (AudioManager.Instance == null) return;

        var clips = AudioManager.Instance.GetFootstepClips();
        if (clips == null || clips.Length == 0) return;

        // sıradaki sesi çal
        AudioManager.Instance.PlayFootstep();

        // index ilerlet
        footstepIndex++;
        if (footstepIndex >= clips.Length)
            footstepIndex = 0;
    }

    // AttackSystem çağıracak
    public void LockFlip(bool locked)
    {
        flipLocked = locked;
    }

    public void SetFacingDirection(Vector3 dir)
    {
        Vector3 scale = spriteTransform.localScale;

        if (dir.x > 0)
            spriteTransform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        else if (dir.x < 0)
            spriteTransform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
    }
}
