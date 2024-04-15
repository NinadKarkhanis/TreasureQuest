using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputSystem controls; // Reference to InputSystem actions
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private BoxCollider2D coll; // Reference to the BoxCollider2D component
    private SpriteRenderer sprite; // Reference to the SpriteRenderer component
    private Animator anim; // Reference to the Animator component
    private float dirX = 0f; // Movement direction along the X-axis

    [SerializeField] private float moveSpeed = 7f; // Movement speed
    [SerializeField] private float jumpForce = 14f; // Force applied when jumping
    [SerializeField] private LayerMask jumpableGround; // Layers considered as ground for jumping

    [SerializeField] private AudioSource jumpSoundEffect; // Sound effect for jumping

    void Awake()
    {
        // Initialize InputSystem controls
        controls = new InputSystem();
        controls.Enable();

        // Assign actions to events
        controls.Land.Movement.performed += ctx => dirX = ctx.ReadValue<float>();
        controls.Land.Movement.canceled += ctx => dirX = 0f;

        controls.Land.Jump.performed += ctx => Jump();
    }

    private void Start()
    {
        // Initialize component references
        coll = GetComponent<BoxCollider2D>(); // Get the BoxCollider2D component
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        sprite = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        anim = GetComponent<Animator>(); // Get the Animator component
    }

    private void Update()
    {
        // Move the player horizontally
        rb.velocity = new Vector2(dirX * moveSpeed * Time.deltaTime, rb.velocity.y);

        // Check if the player is on the ground and not moving vertically
        if (IsGrounded() && rb.velocity.y == 0)
        {
            anim.SetBool("IsJumping", false); // Set the "IsJumping" parameter in the animator to false
        }
        else
        {
            anim.SetBool("IsJumping", true); // Set the "IsJumping" parameter in the animator to true
        }

        // Update animation state based on movement direction
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state; // Variable to store the current movement state
        if (dirX > 0f)
        {
            state = MovementState.running; // Player is moving right
            sprite.flipX = false; // Flip the sprite to face right
        }
        else if (dirX < 0f)
        {
            state = MovementState.running; // Player is moving left
            sprite.flipX = true; // Flip the sprite to face left
        }
        else
        {
            state = MovementState.idle; // Player is not moving horizontally
        }

        // Set the animation state in the animator
        anim.SetInteger("state", (int)state);
    }

    void Jump()
    {
        if (IsGrounded()) // Check if the player is grounded
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply upward force to jump
            jumpSoundEffect.Play(); // Play the jump sound effect
        }
        else
        {
            // Check if colliding with a ladder
            Collider2D[] colliders = Physics2D.OverlapBoxAll(coll.bounds.center, coll.bounds.size, 0f, LayerMask.GetMask("Ladder"));
            if (colliders.Length > 0)
            {
                // Apply a higher jump force if colliding with a ladder
                rb.velocity = new Vector2(rb.velocity.x, 20f);
                jumpSoundEffect.Play(); // Play the jump sound effect
            }
        }
    }

    private bool IsGrounded()
    {
        // Check if there's ground beneath the player using a boxcast
        RaycastHit2D raycastHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        return raycastHit.collider != null; // Return true if ground is detected
    }

    private void OnEnable()
    {
        controls.Enable(); // Enable InputSystem controls
    }

    private void OnDisable()
    {
        controls.Disable(); // Disable InputSystem controls
    }

    private enum MovementState { idle, running, jumping, falling } // Enumeration representing different movement states
}
