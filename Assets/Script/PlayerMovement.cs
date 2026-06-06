using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    AudioManager audioManager;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    private Vector3 originalScale;
    private bool isRunning;

    private void Awake()
    {   
        //get reference for rigidbody and animator from game object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        originalScale = transform.localScale; // because the scale is 0.3
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }   
    

    private void Update()
    {
        //Running
        horizontalInput = Input.GetAxis("Horizontal");

        //to flip player direction when walking
        if (horizontalInput < -0.01f)
            transform.localScale = originalScale;
        else if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

        //to set animator parameters for animation
        anim.SetBool("run", horizontalInput !=0);
        anim.SetBool("grounded", isGrounded());

        //Running SFX
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
            
        //wall jump
        if(wallJumpCooldown > 0.2f)
        {
        body.linearVelocity = new Vector2( horizontalInput * speed, body.linearVelocity.y);

        if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }
        else 
            body.gravityScale = 2.5f;

        if(Input.GetKeyDown(KeyCode.Space))
            Jump(); 
        }
        else 
            wallJumpCooldown += Time.deltaTime;
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
            if (horizontalInput == 0)
            {
                float facing = Mathf.Sign(transform.localScale.x); 
                body.linearVelocity = new Vector2(facing * 10, 6);
                transform.localScale = new Vector3(-Mathf.Sign(-transform.localScale.x) *originalScale.x, transform.localScale.y, transform.localScale.z);
            }
            else
                body.linearVelocity = new Vector2(-Mathf.Sign(-transform.localScale.x) * 3, 6);
            wallJumpCooldown = 0;
        }
    }
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
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
