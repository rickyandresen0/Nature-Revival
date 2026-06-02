using UnityEngine;
using System.Collections;

public class EnemyOneAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject enemyProjectilePrefab;
    [SerializeField] private float range;
    [SerializeField] private float shootDelay;
    private Animator anim;
    private Transform player;
    private float cooldownTimer = Mathf.Infinity;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning($"[EnemyOneAttack] GameObject dengan tag 'Player' tidak ditemukan di Scene saat Awake!", this);
        }
    }

    private void Update()
    {
        if (player == null) return;

        cooldownTimer += Time.deltaTime;

        if (PlayerInRange() && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0;
            StartCoroutine(ShootWithDelay());
        }
    }

    private IEnumerator ShootWithDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        if (player != null)
        {
            Shoot();
        }
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
}