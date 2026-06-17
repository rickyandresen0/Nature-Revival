using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzletwoManager : MonoBehaviour
{
    public static PuzzletwoManager instance;
    [SerializeField] private int totalTrashItems = 6;
    [SerializeField] private string mainGameSceneName = "Level01";

    private int itemSorted = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ItemSorted()
    {
        Debug.Log("Items sorted: " + itemSorted + " / " + totalTrashItems);
        itemSorted++;
        if (itemSorted >= totalTrashItems)
        {
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        PlayerPrefs.SetInt("TrashPuzzleCompleted", 1);
        PlayerPrefs.Save();
        Time.timeScale = 1f;

        if (GameManager.instance != null)
        {
            GameManager.instance.CheckWinCondition();
        }

        SceneManager.UnloadSceneAsync("Puzzle2");
    }
}
