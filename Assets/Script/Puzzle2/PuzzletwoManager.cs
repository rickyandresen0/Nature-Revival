using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PuzzletwoManager : MonoBehaviour
{
    public static PuzzletwoManager instance;
    [SerializeField] private int totalTrashItems = 6;
    [SerializeField] private string mainGameSceneName = "Level01";

    private int itemSorted = 0;

    private void Awake()
    {
        instance = this;
        RemoveDuplicateEventSystemAndAudioListener();
    }

    // Scene Puzzle2 ini di-load secara additive di atas Level01, yang sudah punya
    // EventSystem & AudioListener sendiri. Hapus salinan yang datang bersama scene
    // ini supaya tidak dobel (Camera tetap dipertahankan).
    private void RemoveDuplicateEventSystemAndAudioListener()
    {
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (eventSystems.Length > 1)
        {
            foreach (EventSystem es in eventSystems)
            {
                if (es.gameObject.scene == gameObject.scene)
                {
                    // SetActive(false) is immediate (unlike Destroy, which is deferred to
                    // end of frame), so it stops this duplicate EventSystem's OnEnable from
                    // ever firing and complaining about "only one active Event System".
                    es.gameObject.SetActive(false);
                    Destroy(es.gameObject);
                }
            }
        }

        AudioListener[] audioListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        if (audioListeners.Length > 1)
        {
            foreach (AudioListener al in audioListeners)
            {
                if (al.gameObject.scene == gameObject.scene)
                {
                    al.enabled = false;
                    Destroy(al);
                }
            }
        }
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
