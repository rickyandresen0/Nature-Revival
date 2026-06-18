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

    // PlayerPrefs keys for each puzzle in the game. Add more here if a new puzzle
    // is introduced later - the progress bar / win check will pick it up automatically.
    private static readonly string[] puzzleCompletionKeys = { "TrashPuzzleCompleted", "FixPuzzleCompleted" };

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

        // Total gabungan nilainya akan pas 1.0 (100%) di Slider
        progressBar.value = enemyProgress + GetPuzzleProgress();
    }

    // Setiap puzzle yang sudah selesai menyumbang porsi yang sama dari 20% total,
    // jadi makin banyak puzzle yang ditambahkan, makin kecil porsi per puzzle-nya,
    // tapi totalnya tetap 20% kalau semua puzzle selesai.
    private float GetPuzzleProgress()
    {
        if (puzzleCompletionKeys.Length == 0) return 0f;

        int completedCount = 0;
        foreach (string key in puzzleCompletionKeys)
        {
            if (PlayerPrefs.GetInt(key, 0) == 1)
            {
                completedCount++;
            }
        }

        return ((float)completedCount / puzzleCompletionKeys.Length) * 0.2f;
    }

    private bool AllPuzzlesCompleted()
    {
        foreach (string key in puzzleCompletionKeys)
        {
            if (PlayerPrefs.GetInt(key, 0) != 1)
            {
                return false;
            }
        }
        return true;
    }

    public void CheckWinCondition()
    {
        UpdateProgressBar();
        bool allEnemiesDead = enemiesKilled >= totalEnemies;

        if (allEnemiesDead && AllPuzzlesCompleted())
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
