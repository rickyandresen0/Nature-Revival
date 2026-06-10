using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmationPanel;
    private bool isRestarting;
    private bool isPaused;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        Time.timeScale = 1;
        isPaused = false;
        //sama jg buat kursor
        Cursor.lockState = CursorLockMode.Locked; // lock cursor
        Cursor.visible = false;
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
        if (isRestarting)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene("Menu");
    }

    public void No()
    {
        confirmationPanel.SetActive(false);
    }
}

