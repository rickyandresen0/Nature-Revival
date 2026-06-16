using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    public static GameManager instance; // Singleton dari Main

    [Header("Core Game Elements (Main)")]
    [SerializeField] private int totalEnemies;
    [SerializeField] private Slider progressBar;
    private int enemiesKilled;

    [Header("Puzzle Elements (Puzzle Branch)")]
    [Range(2, 6)]
    [SerializeField] private int difficulty = 4;
    [SerializeField] private Transform gameHolder;
    [SerializeField] private Transform piecePrefab;

    [Header("Puzzle UI Elements")]
    [SerializeField] private List<Texture2D> imageTextures;
    [SerializeField] private Transform levelSelectPanel;
    [SerializeField] private Image levelSelectPrefab;
    [SerializeField] private GameObject playAgainButton;

    private List<Transform> pieces;
    private Vector2Int dimensions;
    private float width;
    private float height;

    private Transform draggingPiece = null;
    private Vector3 offset;
    private int piecesCorrect;

    private void Awake()
    {
        instance = this; // Inisialisasi Singleton
    }

    void Start() {
        // Validasi agar tidak error jika dijalankan di scene tanpa sistem puzzle
        if (levelSelectPanel != null && imageTextures != null && levelSelectPrefab != null) {
            foreach (Texture2D texture in imageTextures) {
                Image image = Instantiate(levelSelectPrefab, levelSelectPanel);
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                image.GetComponent<Button>().onClick.AddListener(delegate { StartGame(texture); });
            }
        }
    }

    // --- CORE GAME LOGIC (MAIN) ---
    public void EnemyKilled()
    {
        enemiesKilled++;
        if (progressBar != null)
            progressBar.value = (float)enemiesKilled / totalEnemies;

        if (enemiesKilled >= totalEnemies)
            Win();
    }

    private void Win()
    {
        Debug.Log("You Win!");
    }


    // --- PUZZLE LOGIC (PUZZLE2 BRANCH) ---
    public void StartGame(Texture2D jigsawTexture) {
        if (levelSelectPanel != null) levelSelectPanel.gameObject.SetActive(false);

        pieces = new List<Transform>();
        dimensions = GetDimensions(jigsawTexture, difficulty);
        CreateJigsawPieces(jigsawTexture);
        Scatter();
        UpdateBorder();
        piecesCorrect = 0;
    }

    Vector2Int GetDimensions(Texture2D jigsawTexture, int difficulty) {
        Vector2Int dimensions = Vector2Int.zero;
        if (jigsawTexture.width < jigsawTexture.height) {
            dimensions.x = difficulty;
            dimensions.y = (difficulty * jigsawTexture.height) / jigsawTexture.width;
        } else {
            dimensions.x = (difficulty * jigsawTexture.width) / jigsawTexture.height;
            dimensions.y = difficulty;
        }
        return dimensions;
    }

    void CreateJigsawPieces(Texture2D jigsawTexture) {
        height = 1f / dimensions.y;
        float aspect = (float)jigsawTexture.width / jigsawTexture.height;
        width = aspect / dimensions.x;

        for (int row = 0; row < dimensions.y; row++) {
            for (int col = 0; col < dimensions.x; col++) {
                Transform piece = Instantiate(piecePrefab, gameHolder);
                piece.localPosition = new Vector3(
                    (-width * dimensions.x / 2) + (width * col) + (width / 2),
                    (-height * dimensions.y / 2) + (height * row) + (height / 2),
                    -1);
                piece.localScale = new Vector3(width, height, 1f);

                piece.name = $"Piece {(row * dimensions.x) + col}";
                pieces.Add(piece);

                float width1 = 1f / dimensions.x;
                float height1 = 1f / dimensions.y;
                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(width1 * col, height1 * row);
                uv[1] = new Vector2(width1 * (col + 1), height1 * row);
                uv[2] = new Vector2(width1 * col, height1 * (row + 1));
                uv[3] = new Vector2(width1 * (col + 1), height1 * (row + 1));
                
                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                mesh.uv = uv;
                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", jigsawTexture);
            }
        }
    }

    private void Scatter() {
        if (Camera.main == null || pieces == null) return;

        float orthoHeight = Camera.main.orthographicSize;
        float screenAspect = (float)Screen.width / Screen.height;
        float orthoWidth = (screenAspect * orthoHeight);

        float pieceWidth = width * (gameHolder != null ? gameHolder.localScale.x : 1f);
        float pieceHeight = height * (gameHolder != null ? gameHolder.localScale.y : 1f);

        orthoHeight -= pieceHeight;
        orthoWidth -= pieceWidth;

        foreach (Transform piece in pieces) {
            float x = Random.Range(-orthoWidth, orthoWidth);
            float y = Random.Range(-orthoHeight, orthoHeight);
            piece.position = new Vector3(x, y, -1);
        }
    }

    private void UpdateBorder() {
        if (gameHolder == null) return;
        LineRenderer lineRenderer = gameHolder.GetComponent<LineRenderer>();
        if (lineRenderer == null) return;

        float halfWidth = (width * dimensions.x) / 2f;
        float halfHeight = (height * dimensions.y) / 2f;
        float borderZ = 0f;

        lineRenderer.SetPosition(0, new Vector3(-halfWidth, halfHeight, borderZ));
        lineRenderer.SetPosition(1, new Vector3(halfWidth, halfHeight, borderZ));
        lineRenderer.SetPosition(2, new Vector3(halfWidth, -halfHeight, borderZ));
        lineRenderer.SetPosition(3, new Vector3(-halfWidth, -halfHeight, borderZ));

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.enabled = true;
    }

    void Update() {
        // Mencegah error jika dijalankan di scene non-puzzle
        if (pieces == null || Camera.main == null) return; 

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            // Proteksi: Pastikan yang diklik BENAR-BENAR kepingan puzzle, bukan objek game lain
            if (hit && pieces.Contains(hit.transform)) { 
                draggingPiece = hit.transform;
                offset = draggingPiece.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                offset += Vector3.back;
            }
        }

        if (draggingPiece && Input.GetMouseButtonUp(0)) {
            SnapAndDisableIfCorrect();
            draggingPiece.position += Vector3.forward;
            draggingPiece = null;
        }

        if (draggingPiece) {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition += offset;
            draggingPiece.position = newPosition;
        }
    }

    private void SnapAndDisableIfCorrect() {
        if (pieces == null || draggingPiece == null) return;
        int pieceIndex = pieces.IndexOf(draggingPiece);

        int col = pieceIndex % dimensions.x;
        int row = pieceIndex / dimensions.x;

        Vector2 targetPosition = new((-width * dimensions.x / 2) + (width * col) + (width / 2),
                                     (-height * dimensions.y / 2) + (height * row) + (height / 2));

        if (Vector2.Distance(draggingPiece.localPosition, targetPosition) < (width / 2)) {
            draggingPiece.localPosition = targetPosition;
            if (draggingPiece.GetComponent<BoxCollider2D>() != null)
                draggingPiece.GetComponent<BoxCollider2D>().enabled = false;

            piecesCorrect++;
            if (piecesCorrect == pieces.Count && playAgainButton != null) {
                playAgainButton.SetActive(true);
            }
        }
    }

    public void RestartGame() {
        if (pieces == null) return;
        foreach (Transform piece in pieces) {
            if (piece != null) Destroy(piece.gameObject);
        }
        pieces.Clear();
        if (gameHolder != null && gameHolder.GetComponent<LineRenderer>() != null)
            gameHolder.GetComponent<LineRenderer>().enabled = false;
        
        if (playAgainButton != null) playAgainButton.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.gameObject.SetActive(true);
    }
}