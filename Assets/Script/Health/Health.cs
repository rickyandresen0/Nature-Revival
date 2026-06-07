using System.Data.Common;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private HealthBar healthBar;
    private float currentHealth;
    private Animator anim;
    private bool dead;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
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

            //player hurt sfx
            if (GetComponent<PlayerMovement>() != null)
                audioManager.PlaySFX(audioManager.takeDamage);
            // enemy hurt sfx
            else
                audioManager.PlaySFX(audioManager.enemyTakeDamage);
        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("die");
                dead = true;
                PlayerMovement pm = GetComponent<PlayerMovement>();
                if (pm != null) pm.enabled = false;
                // enemy die sfx
                if (GetComponent<PlayerMovement>() == null)
                audioManager.PlaySFX(audioManager.enemyDie);
                Die();

            }
        }
    }

    private void Die()
    {
        if (GetComponent<PlayerMovement>() == null)
        {
            GameManager.instance.EnemyKilled();
        }  
        Destroy(gameObject, 0.8f);
    }
}