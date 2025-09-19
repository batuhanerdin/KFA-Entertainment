using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 moveInput;

    private StatSystem stats;
    private Animator animator;
    private bool flipLocked = false; // ✅ saldırı sırasında yön kilidi
    private Transform spriteTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        stats = GetComponent<StatSystem>();
        animator = GetComponentInChildren<Animator>();
        spriteTransform = animator.transform; // animator child objesi
    }

    private void Update()
    {
        HandleInput();
        HandleAnimations();
        HandleSpriteFlip();
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
        if (flipLocked) return; // saldırı sırasında flip kapalı

        Vector3 scale = spriteTransform.localScale;

        if (moveInput.x > 0.1f)
            spriteTransform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        else if (moveInput.x < -0.1f)
            spriteTransform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
    }

   

    // ✅ AttackSystem buradan çağıracak
    public void LockFlip(bool locked)
    {
        flipLocked = locked;
    }

    // ✅ AttackSystem çağıracak
    public void SetFacingDirection(Vector3 dir)
    {
        Vector3 scale = spriteTransform.localScale;

        if (dir.x > 0)
            spriteTransform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        else if (dir.x < 0)
            spriteTransform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
    }
}
