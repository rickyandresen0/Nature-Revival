using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject confirmQuitPanel;
    public void PlayGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);   
    }

    public void QuitGame ()
    {
        Debug.Log ("Quit Game");
        Application.Quit();

    }

    public void Yes()
    {
        Application.Quit();
    }

        public void No()
    {
        confirmQuitPanel.SetActive(false);
    }
}
