using System.Data.Common;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private HealthBar healthBar;
    private float currentHealth;
    private Animator anim;
    private bool dead;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = startingHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth((int)startingHealth);
    }

    public void TakeDamage(float damage)
    {   
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        if (healthBar != null)
            healthBar.SetHealth((int)currentHealth);

        if (currentHealth > 0) 
        {
            anim.SetTrigger("hurt");
        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("die");
                dead = true;
                PlayerMovement pm = GetComponent<PlayerMovement>();
                if (pm != null) pm.enabled = false;
                Die();

            }
        }
    }

    private void Die()
    {
        Destroy(gameObject, 0.5f);
    }
}