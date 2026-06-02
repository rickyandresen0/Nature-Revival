using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int totalEnemies;
    [SerializeField] private Slider progressBar;
    private int enemiesKilled;

    private void Awake()
    {
        instance = this;
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        progressBar.value = (float)enemiesKilled / totalEnemies;

        if (enemiesKilled >= totalEnemies)
            Win();
    }

    private void Win()
    {
        Debug.Log("You Win!");
    }
}
