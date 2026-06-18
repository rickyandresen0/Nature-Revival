using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private string mainGameplaySceneName = "Level01";
    private bool isRestarting;
    private bool isPaused;

    // Scene-scene puzzle additive yang butuh cursor bebas (mouse) buat dimainkan.
    private static readonly string[] puzzleSceneNames = { "Puzzle2", "FixPuzzle" };

    private bool IsAnyPuzzleSceneActive()
    {
        foreach (string sceneName in puzzleSceneNames)
        {
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        if (IsAnyPuzzleSceneActive())
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
        bool isPuzzleActive = IsAnyPuzzleSceneActive();

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
        bool isPuzzleActive = IsAnyPuzzleSceneActive();
        if (isRestarting)
        {
            PlayerPrefs.DeleteKey("TrashPuzzleCompleted");
            PlayerPrefs.DeleteKey("FixPuzzleCompleted");
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
            PlayerPrefs.DeleteKey("FixPuzzleCompleted");
            PlayerPrefs.Save();
            SceneManager.LoadScene("Menu");
        }
    }

    public void No()
    {
        confirmationPanel.SetActive(false);
    }
}

