using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private string targetTag;
    private float direction;
    private bool hit;
    private float lifetime;

    private BoxCollider2D boxCollider;
    private Animator anim;
    private Rigidbody2D body;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        hit = false;
    }

    private void Update()
    {
        if (hit) return;

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (hit) return;

        // Gunakan Rigidbody2D.velocity bukan transform.Translate
        // agar collision detection physics engine berjalan dengan benar
        body.linearVelocity = new Vector2(speed * -direction, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hit) return; // cegah double trigger

        if (collision.CompareTag(targetTag))
        {
            collision.GetComponent<Health>().TakeDamage(damage);
            hit = true;
            boxCollider.enabled = false;
            body.linearVelocity = Vector2.zero;
            anim.SetTrigger("explode");
        }
        else if (!collision.isTrigger)
        {
            hit = true;
            boxCollider.enabled = false;
            body.linearVelocity = Vector2.zero;
            anim.SetTrigger("explode");
        }
    }

    public void setDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        // Flip sprite arah
        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}