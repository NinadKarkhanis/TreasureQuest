using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputSystem controls;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    private float dirX = 0f;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private LayerMask jumpableGround;

    [SerializeField] private AudioSource jumpSoundEffect;

    void Awake()
    {
        controls = new InputSystem();
        controls.Enable();

        controls.Land.Movement.performed += ctx => dirX = ctx.ReadValue<float>();
        controls.Land.Movement.canceled += ctx => dirX = 0f;

        controls.Land.Jump.performed += ctx => Jump();
    }

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>(); // collision
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        rb.velocity = new Vector2(dirX * moveSpeed * Time.deltaTime, rb.velocity.y);

        if (IsGrounded() && rb.velocity.y == 0)
        {
            anim.SetBool("IsJumping", false);
        }
        else
        {
            anim.SetBool("IsJumping", true);
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state;
        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        anim.SetInteger("state", (int)state);
    }

    void Jump()
{
    if (IsGrounded())
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpSoundEffect.Play();
    }
    else
    {
        // Check if colliding with ladder
        Collider2D[] colliders = Physics2D.OverlapBoxAll(coll.bounds.center, coll.bounds.size, 0f, LayerMask.GetMask("Ladder"));
        if (colliders.Length > 0)
        {
            // Change jump force to 500
            rb.velocity = new Vector2(rb.velocity.x, 20f);
            jumpSoundEffect.Play();
        }
    }
}

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        return raycastHit.collider != null;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private enum MovementState { idle, running, jumping, falling }
}