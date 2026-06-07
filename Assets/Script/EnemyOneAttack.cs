using UnityEngine;
using System.Collections;

public class EnemyOneAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject enemyProjectilePrefab;
    [SerializeField] private float range = 5f;
    [SerializeField] private float shootDelay = 0.3f;

    public float Range => range;

    private Animator anim;
    private Transform player;
    private float cooldownTimer = Mathf.Infinity;
    private bool attackEnabled = false; 
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        anim = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("[EnemyOneAttack] Tag 'Player' tidak ditemukan!", this);
    }

    private void Update()
    {
        if (player == null || !attackEnabled) return;

        cooldownTimer += Time.deltaTime;

        if (PlayerInRange() && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0f;
            StartCoroutine(ShootWithDelay());
        }
    }

    public void SetAttackEnabled(bool enabled)
    {
        attackEnabled = enabled;
    }

    private IEnumerator ShootWithDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        if (player != null && attackEnabled)
            Shoot();
    }

    private void Shoot()
    {
        if (player == null) return;

        anim.SetTrigger("enemyShoot");

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        GameObject projectile = Instantiate(enemyProjectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().setDirection(-direction);
        audioManager.PlaySFX(audioManager.enemyShoot);
    }

    private bool PlayerInRange()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= range;
    }
}