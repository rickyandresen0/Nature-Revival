using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private string mainGameplaySceneName = "Level01";
    private bool isRestarting;
    private bool isPaused;

    private void Start()
    {
        if (SceneManager.GetSceneByName("Puzzle2").isLoaded)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    //tombol for pause menu
    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        //buat kursorrrrr
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        confirmationPanel.SetActive(false); 
        isPaused = false;
        bool isPuzzleActive = SceneManager.GetSceneByName("Puzzle2").isLoaded;

        if (isPuzzleActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f; 
            Cursor.lockState = CursorLockMode.Locked; // lock cursor
            Cursor.visible = false;
        }
    }

    public void ConfirmRestart()
    {
        isRestarting = true;
        confirmationPanel.SetActive(true);
    }

    public void ConfirmQuit()
    {
        isRestarting = false;
        confirmationPanel.SetActive(true);
    }

    //untuk rombol confirmation yes or no
    public void Yes()
    {
        Time.timeScale = 1;
        bool isPuzzleActive = SceneManager.GetSceneByName("Puzzle2").isLoaded;
        if (isRestarting)
        {
            PlayerPrefs.DeleteKey("TrashPuzzleCompleted");
            PlayerPrefs.Save();

            if (isPuzzleActive)
            {
                SceneManager.LoadScene(mainGameplaySceneName);
            }
            else 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            PlayerPrefs.DeleteKey("TrashPuzzleCompleted");
            PlayerPrefs.Save();
            SceneManager.LoadScene("Menu");
        }
    }

    public void No()
    {
        confirmationPanel.SetActive(false);
    }
}

