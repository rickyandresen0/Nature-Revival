using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int totalEnemies;
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
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
        winPanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Lose()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
