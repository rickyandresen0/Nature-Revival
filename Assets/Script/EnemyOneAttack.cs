using UnityEngine;
using System.Collections;

public class EnemyOneAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float shootDelay;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject enemyProjectilePrefab;

    private Animator anim;
    private Transform player;
    private EnemyMovement enemyMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("[EnemyOneAttack] Player tidak ditemukan!", this);
    }

    private void Update()
    {
        if (player == null) return;

        cooldownTimer += Time.deltaTime;

        if (PlayerInRange())
        {
            if (enemyMovement != null)
                enemyMovement.enabled = false;

            FacePlayer();

            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                StartCoroutine(ShootWithDelay());
            }
        }
        else
        {
            if (enemyMovement != null)
                enemyMovement.enabled = true;
        }
    }

    private void FacePlayer()
    {
        float directionToPlayer = player.position.x - transform.position.x;
        Vector3 scale = transform.localScale;

        if (directionToPlayer > 0 && scale.x > 0)
            transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        else if (directionToPlayer < 0 && scale.x < 0)
            transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
    }

    private IEnumerator ShootWithDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        if (player != null)
            Shoot();
    }

    private void Shoot()
    {
        if (player == null) return;

        anim.SetTrigger("enemyShoot");
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        GameObject projectile = Instantiate(enemyProjectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().setDirection(-direction);
    }

    private bool PlayerInRange()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= range;
    }

    // Visualisasi radius di Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}