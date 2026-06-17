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

        private void Start()
    {
        UpdateProgressBar();
        CheckWinCondition();
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        progressBar.value = (float)enemiesKilled / totalEnemies;

        UpdateProgressBar();
        CheckWinCondition();
    }
    private void UpdateProgressBar()
    {
        if (progressBar == null) return;

        float enemyProgress = 0f;
        if (totalEnemies > 0)
        {
            enemyProgress = ((float)enemiesKilled / totalEnemies) * 0.8f;
        }
        float puzzleProgress = (PlayerPrefs.GetInt("TrashPuzzleCompleted", 0) == 1) ? 0.2f : 0f;

        // Total gabungan nilainya akan pas 1.0 (100%) di Slider
        progressBar.value = enemyProgress + puzzleProgress;
    }
    public void CheckWinCondition()
    {
        UpdateProgressBar();
        bool allEnemiesDead = enemiesKilled >= totalEnemies;
        bool puzzleCompleted = PlayerPrefs.GetInt("TrashPuzzleCompleted", 0) == 1;

        if (allEnemiesDead && puzzleCompleted)
        {
            Win();
        }
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
