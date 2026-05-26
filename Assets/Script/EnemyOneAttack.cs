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
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
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
        Shoot();
    }

    private void Shoot()
    {


        anim.SetTrigger("enemyShoot");
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        GameObject projectile = Instantiate(enemyProjectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().setDirection(-direction);
    }

    private bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= range;
    }
}


