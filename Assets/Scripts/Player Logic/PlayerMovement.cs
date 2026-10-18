using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    private InputSystem controls;
    private Rigidbody2D body;
    private BoxCollider2D box;
    private SpriteRenderer sprite;
    private Animator animator;
    private float horizontal;
    private float jumpQueuedUntil = -1f;
    private float coyoteUntil = -1f;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField, Range(0f, .25f)] private float jumpBufferSeconds = .12f;
    [SerializeField, Range(0f, .25f)] private float coyoteSeconds = .10f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        controls = new InputSystem();
        controls.Land.Movement.performed += ctx => horizontal = ctx.ReadValue<float>();
        controls.Land.Movement.canceled += ctx => horizontal = 0f;
        controls.Land.Jump.performed += ctx => jumpQueuedUntil = Time.time + jumpBufferSeconds;
    }

    private void OnEnable() { if (controls != null) controls.Enable(); }
    private void OnDisable() { if (controls != null) controls.Disable(); horizontal = 0f; }
    private void OnDestroy() { if (controls != null) controls.Dispose(); }

    private void Update()
    {
        bool grounded = IsGrounded();
        if (grounded) coyoteUntil = Time.time + coyoteSeconds;
        if (jumpQueuedUntil >= Time.time && coyoteUntil >= Time.time)
        {
            PerformJump(jumpForce);
            jumpQueuedUntil = -1f;
            coyoteUntil = -1f;
        }
        else if (jumpQueuedUntil >= Time.time && IsOnLadder())
        {
            PerformJump(20f);
            jumpQueuedUntil = -1f;
        }

        if (horizontal > 0) sprite.flipX = false;
        else if (horizontal < 0) sprite.flipX = true;
        if (animator != null)
        {
            animator.SetInteger("state", Mathf.Abs(horizontal) > .01f ? 1 : 0);
            animator.SetBool("IsJumping", !grounded);
        }
    }

    private void FixedUpdate()
    {
        // Earlier prefabs stored speed 300 because movement multiplied velocity by deltaTime.
        float unitsPerSecond = moveSpeed > 50f ? moveSpeed * .02f : moveSpeed;
        body.velocity = new Vector2(horizontal * unitsPerSecond, body.velocity.y);
    }

    private void PerformJump(float force)
    {
        body.velocity = new Vector2(body.velocity.x, force);
        if (jumpSoundEffect != null) jumpSoundEffect.Play();
    }
    private bool IsOnLadder()
    {
        return Physics2D.OverlapBox(box.bounds.center, box.bounds.size, 0,
            LayerMask.GetMask("Ladder")) != null;
    }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.down,
            .12f, jumpableGround).collider != null;
    }
}
