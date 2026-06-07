using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Wall Slide")]
    [SerializeField] private float wallSlideGravity = 1.0f;   // gravity saat menempel wall (lebih kecil = slide lambat)
    [SerializeField] private float wallSlideMaxSpeed = 2.5f;  // batas kecepatan jatuh saat slide
    [SerializeField] private float wallStickDuration = 0.12f; // detik player "nempel" sebelum mulai slide

    AudioManager audioManager;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private PhysicsMaterial2D noFriction;
    private PhysicsMaterial2D fullFriction;

    private float wallJumpCooldown;
    private float horizontalInput;
    private Vector3 originalScale;
    private bool isRunning;

    private float wallStickTimer;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        originalScale = transform.localScale;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        noFriction = new PhysicsMaterial2D("NoFriction");
        noFriction.friction = 0f;
        noFriction.bounciness = 0f;

        fullFriction = new PhysicsMaterial2D("FullFriction");
        fullFriction.friction = 0.4f;
        fullFriction.bounciness = 0f;

        boxCollider.sharedMaterial = noFriction;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // Flip player direction
        if (horizontalInput < -0.01f)
            transform.localScale = originalScale;
        else if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

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
        if (wallJumpCooldown > 0.2f)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (onWall() && !isGrounded())
            {
                HandleWallSlide();
            }
            else
            {
                wallStickTimer = 0f;
                body.gravityScale = 2.5f;

                // Kembalikan full friction hanya saat di tanah agar tidak slip
                boxCollider.sharedMaterial = isGrounded() ? fullFriction : noFriction;
            }

            if (Input.GetKeyDown(KeyCode.Space))
                Jump();
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void HandleWallSlide()
    {
        boxCollider.sharedMaterial = noFriction; // pastikan tidak ada friction samping

        wallStickTimer += Time.deltaTime;

        if (wallStickTimer < wallStickDuration)
        {
            body.gravityScale = 0f;
            body.linearVelocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = wallSlideGravity;
            float clampedY = Mathf.Max(body.linearVelocity.y, -wallSlideMaxSpeed);
            body.linearVelocity = new Vector2(0f, clampedY);
        }
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            wallStickTimer = 0f;

            if (horizontalInput == 0)
            {
                float facing = Mathf.Sign(transform.localScale.x);
                body.linearVelocity = new Vector2(facing * 10, 6);
                transform.localScale = new Vector3(
                    -Mathf.Sign(-transform.localScale.x) * originalScale.x,
                    transform.localScale.y,
                    transform.localScale.z);
            }
            else
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(-transform.localScale.x) * 3, 6);
            }

            wallJumpCooldown = 0;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            new Vector2(-transform.localScale.x, 0),
            0.1f,
            wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}