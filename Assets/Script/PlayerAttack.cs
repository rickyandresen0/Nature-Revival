using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] starMagics;
    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    } 

    private void Update()
    {
        if(Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())
            Attack();

            cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        starMagics[FindStarMagic()].transform.position = firePoint.position; 
        starMagics[FindStarMagic()].GetComponent<Projectile>().setDirection(Mathf.Sign(transform.localScale.x)); 
    }

    private int FindStarMagic()
    {
        for (int i=0; i < starMagics.Length; i++)
        {
            if (!starMagics[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
