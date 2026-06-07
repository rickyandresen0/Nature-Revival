using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private string targetTag = "Player";

    [Tooltip("Centang untuk EnemyProjectile (Instantiate). Kosongkan untuk StarMagic (object pool).")]
    [SerializeField] private bool destroyOnHit = false;

    [Tooltip("Durasi animasi explode sebelum object dihancurkan/dinonaktifkan. Sesuaikan dengan panjang clip.")]
    [SerializeField] private float explodeDuration = 0.25f;

    private float direction;
    private bool hit;
    private float lifetime;

    private BoxCollider2D boxCollider;
    private Animator anim;
    private Rigidbody2D body;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        anim        = GetComponent<Animator>();
        body        = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        hit      = false;
        lifetime = 0f;
        if (boxCollider != null) boxCollider.enabled = true;
        if (body != null)        body.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        if (hit) return;

        lifetime += Time.deltaTime;
        if (lifetime > 5f)
            TriggerExplode();
    }

    private void FixedUpdate()
    {
        if (hit) return;
        body.linearVelocity = new Vector2(speed * -direction, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hit) return;

        if (collision.CompareTag(targetTag))
        {
            Health health = collision.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damage);

            TriggerExplode();
        }
        else if (!collision.isTrigger)
        {
            TriggerExplode();
        }
    }

    private void TriggerExplode()
    {
        if (hit) return;

        hit = true;
        boxCollider.enabled = false;
        body.linearVelocity = Vector2.zero;
        body.gravityScale   = 0f;
        anim.SetTrigger("explode");

        if (destroyOnHit)
            Destroy(gameObject, explodeDuration);
        else
            Invoke(nameof(Deactivate), explodeDuration);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void setDirection(float _direction)
    {
        direction = _direction;
        hit       = false;
        lifetime  = 0f;

        gameObject.SetActive(true);

        if (boxCollider != null) boxCollider.enabled = true;

        // Flip sprite sesuai arah
        float sx = transform.localScale.x;
        if (Mathf.Sign(sx) != _direction)
            sx = -sx;
        transform.localScale = new Vector3(sx, transform.localScale.y, transform.localScale.z);
    }
}