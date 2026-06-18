using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject interactPrompt;
    private bool playerInRange;

    [Header("Puzzle Settings")]
    [Tooltip("Nama scene puzzle yang akan di-load (additive) saat player interact.")]
    [SerializeField] private string puzzleSceneName = "Puzzle2";

    [Tooltip("PlayerPrefs key yang dipakai untuk menandai puzzle ini sudah selesai.")]
    [SerializeField] private string completedPlayerPrefsKey = "TrashPuzzleCompleted";

    [Tooltip("Khusus puzzle jigsaw (scene FixPuzzle): index gambar yang langsung dimainkan, skip layar pilih puzzle. " +
             "0 = puzzle paling kiri, 1 = puzzle berikutnya (kanan), dst. Set -1 kalau puzzle tidak punya mode ini (misalnya Puzzle2).")]
    [SerializeField] private int jigsawStartIndex = -1;

    [Header("Dialogue Text")]
    [SerializeField] private string helpMessage = "Please help me!";
    [SerializeField] private string completedMessage = "Thank you for cleaning up!";

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
        int isCompleted = PlayerPrefs.GetInt(completedPlayerPrefsKey, 0);

        if (isCompleted == 1)
        {
            ShowDialogue(completedMessage);
            Invoke("HideDialogue", 2f);
        }
        else
        {
            ShowDialogue(helpMessage);
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

        // Kalau ini puzzle jigsaw dan kita mau langsung mulai gambar tertentu
        // (skip layar pilih puzzle), titip pesan ke Puzzle.cs lewat static field
        // sebelum scene-nya di-load.
        if (jigsawStartIndex >= 0)
        {
            Puzzle.RequestedIndex = jigsawStartIndex;
            Puzzle.CompletionPlayerPrefsKey = completedPlayerPrefsKey;
        }

        // -----------------------------------------------------------------------
        // FIX: Dulu Time.timeScale = 0f di-set di sini, SEBELUM scene puzzle
        //      di-load.  Akibatnya Physics2D simulation ter-pause, sehingga
        //      Physics2D.Raycast (maupun OverlapPoint yang butuh broadphase
        //      sudah ter-populate) tidak mendeteksi klik pada puzzle pieces.
        //
        //      Solusi: gunakan SceneManager.sceneLoaded callback untuk
        //      men-set timeScale = 0 SETELAH scene puzzle benar-benar siap,
        //      sehingga semua collider sudah ter-register ke Physics2D.
        // -----------------------------------------------------------------------
        SceneManager.sceneLoaded += OnPuzzleSceneLoaded;
        SceneManager.LoadScene(puzzleSceneName, LoadSceneMode.Additive);

        // Jangan set timeScale = 0 di sini — dipindah ke callback di bawah.
    }

    // Callback: dipanggil Unity tepat setelah scene selesai di-load secara additive.
    // Di sini baru aman untuk mem-pause Time karena semua Awake() dan OnEnable()
    // pada scene baru sudah selesai dijalankan, termasuk registrasi collider ke
    // broadphase Physics2D.
    private void OnPuzzleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == puzzleSceneName)
        {
            // Unsubscribe segera supaya callback ini tidak terpanggil lagi
            // bila scene lain di-load di kemudian hari.
            SceneManager.sceneLoaded -= OnPuzzleSceneLoaded;

            // Baru sekarang pause game; puzzle sudah siap dan Physics2D sudah
            // mengenal semua collider dari scene baru.
            Time.timeScale = 0f;
        }
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
