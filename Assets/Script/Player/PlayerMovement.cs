using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Wall Slide")]
    [SerializeField] private float wallSlideGravity = 1.0f;
    [SerializeField] private float wallSlideMaxSpeed = 2.5f;

    [Header("Wall Jump Tuning")]
    [SerializeField] private float wallJumpUpForce = 8f;
    [SerializeField] private float wallJumpPushForce = 10f;

    AudioManager audioManager;
    private Rigidbody2D body;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;
    private PhysicsMaterial2D noFriction;
    private PhysicsMaterial2D fullFriction;

    private float wallJumpCooldown;
    private float horizontalInput;
    private Vector3 originalScale;
    private bool isRunning;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        originalScale = transform.localScale;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        noFriction = new PhysicsMaterial2D("NoFriction");
        noFriction.friction = 0f;
        noFriction.bounciness = 0f;

        fullFriction = new PhysicsMaterial2D("FullFriction");
        fullFriction.friction = 0.4f;
        fullFriction.bounciness = 0f;

        capsuleCollider.sharedMaterial = noFriction;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (wallJumpCooldown <= 0.2f)
        {
            wallJumpCooldown += Time.deltaTime;
        }

        // Flip player direction
        if (wallJumpCooldown > 0.2f)
            {
                if (horizontalInput < -0.01f)
                    transform.localScale = originalScale;
                else if (horizontalInput > 0.01f)
                    transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
            }

        // Animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        // Running SFX
        if (horizontalInput != 0 && isGrounded())
        {
            if (!isRunning)
            {
                audioManager.PlaySFX(audioManager.run);
                isRunning = true;
            }
        }
        else
            isRunning = false;

        // Movement & wall logic
        if (onWall() && !isGrounded() && wallJumpCooldown > 0.2f)
        {
            HandleWallSlide();
        }
        else
        {
            body.gravityScale = 2.5f;
            capsuleCollider.sharedMaterial = (isGrounded() && !onWall()) ? fullFriction : noFriction;

            if (wallJumpCooldown > 0.2f)
            {
                body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void HandleWallSlide()
    {
        capsuleCollider.sharedMaterial = noFriction;
        
        body.gravityScale = wallSlideGravity;
        float clampedY = Mathf.Max(body.linearVelocity.y, -wallSlideMaxSpeed);
        body.linearVelocity = new Vector2(0f, clampedY);
        
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("jump");
            audioManager.PlaySFX(audioManager.jump); 
        }
        else if (onWall() && !isGrounded())
        {
            float wallDir = -Mathf.Sign(transform.localScale.x);
            float jumpDirX = -wallDir; 
            body.linearVelocity = new Vector2(jumpDirX * wallJumpPushForce, wallJumpUpForce);

        if (jumpDirX < 0)
            transform.localScale = originalScale;

        else
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

        wallJumpCooldown = 0f;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        Vector2 boxSize = new Vector2(capsuleCollider.bounds.size.x, capsuleCollider.bounds.size.y * 0.9f);
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            capsuleCollider.bounds.center,
            boxSize,
            0,
            new Vector2(-transform.localScale.x, 0),
            0.15f,
            wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}