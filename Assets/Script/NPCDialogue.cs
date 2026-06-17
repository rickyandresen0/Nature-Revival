using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject interactPrompt;
    private bool playerInRange;

    private void Update()
    {
      if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryStartPuzzle();
        }  
    }

    private void TryStartPuzzle()
    {
        //ngecek udh belum
        int isCompleted = PlayerPrefs.GetInt("TrashPuzzleCompleted", 0);

        if (isCompleted == 1)
        {
            ShowDialogue("Thank you for cleaning up!");
            Invoke("HideDialogue", 2f);
        }
        else
        {
            ShowDialogue("Please help me!");
            Invoke("LoadPuzzle", 1.5f);
        }
    }

    private void ShowDialogue(string message)
    {
        dialogueText.text = message;
        dialogueBox.SetActive(true);
    }

    private void HideDialogue()
    {
        dialogueBox.SetActive(false);
    }

        private void LoadPuzzle()
    {
        HideDialogue();
        SceneManager.LoadScene("Puzzle2");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            HideDialogue();
            if (interactPrompt != null)
                interactPrompt.SetActive(false);               
        }
    }
}
