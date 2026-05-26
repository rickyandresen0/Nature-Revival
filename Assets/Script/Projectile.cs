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

private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

private void Update()
    {
        if(hit) return;
        float movementSpeed = speed * Time.deltaTime * -direction;
        transform.Translate(movementSpeed, 0, 0);
        Debug.Log("Projectile position: " + transform.position);

        lifetime += Time.deltaTime;
        if(lifetime > 5) gameObject.SetActive(false);
    }

private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag(targetTag))
        collision.GetComponent<Health>().TakeDamage(damage);

    hit = true;
    boxCollider.enabled = false;
    anim.SetTrigger("explode");
}


public void setDirection(float _direction)
    {
        lifetime = 0; 
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        //set starmagic direction
        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3 (localScaleX, transform.localScale.y, transform.localScale.z);
    }

private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
