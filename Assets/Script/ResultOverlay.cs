using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Mengatur overlay hasil permainan (Victory / Defeat).
/// 
/// FITUR UTAMA:
/// - Board background yang lebih kecil dari layar penuh (bukan fullscreen)
/// - Sprite background board bisa diganti via Inspector ATAU via kode (tanpa edit Canvas manual)
/// - Victory: tombol Continue, Restart, Main Menu
/// - Defeat: tombol Restart, Main Menu
/// 
/// CARA PENGGUNAAN:
/// 1. Buat Canvas overlay dengan dark background (Image fullscreen, warna hitam alpha ~150)
/// 2. Di dalamnya buat Image "Board" yang lebih kecil (misal 600x400) sebagai background board
/// 3. Assign semua referensi di Inspector
/// 4. Untuk mengganti gambar board, cukup ubah victoryBoardSprite / defeatBoardSprite di Inspector
///    ATAU panggil SetVictoryBoardSprite() / SetDefeatBoardSprite() dari kode lain
/// </summary>
public class ResultOverlay : MonoBehaviour
{
    public static ResultOverlay instance;

    [Header("=== ROOT OVERLAY ===")]
    [Tooltip("GameObject root dari seluruh overlay (diaktifkan/dinonaktifkan)")]
    public GameObject overlayRoot;

    [Header("=== BACKGROUNDS ===")]
    [Tooltip("Image fullscreen semi-transparan gelap di belakang board")]
    public Image darkBackground;

    [Tooltip("Image board yang lebih kecil dari layar - menjadi background panel hasil")]
    public Image boardImage;

    [Header("=== BOARD SPRITES ===")]
    [Tooltip("Sprite background board saat VICTORY. Drag gambar ke sini untuk mengganti.")]
    public Sprite victoryBoardSprite;

    [Tooltip("Sprite background board saat DEFEAT. Drag gambar ke sini untuk mengganti.")]
    public Sprite defeatBoardSprite;

    [Header("=== VICTORY CONTENT ===")]
    [Tooltip("GameObject parent yang berisi semua elemen Victory")]
    public GameObject victoryContent;
    public TextMeshProUGUI victoryTitleText;
    public TextMeshProUGUI victoryScoreText;
    public TextMeshProUGUI victoryMessageText;

    [Header("=== DEFEAT CONTENT ===")]
    [Tooltip("GameObject parent yang berisi semua elemen Defeat")]
    public GameObject defeatContent;
    public TextMeshProUGUI defeatTitleText;
    public TextMeshProUGUI defeatScoreText;
    public TextMeshProUGUI defeatMessageText;

    [Header("=== BOARD SIZE SETTINGS ===")]
    [Tooltip("Ukuran board dalam pixel. Board akan di-center di layar.")]
    public Vector2 boardSize = new Vector2(700f, 500f);

    [Header("=== ANIMATION ===")]
    [Tooltip("Apakah board muncul dengan animasi scale")]
    public bool useScaleAnimation = true;
    public float animationDuration = 0.3f;

    private float animTimer = 0f;
    private bool isAnimating = false;
    private RectTransform boardRect;

    void Awake()
    {
        instance = this;

        if (overlayRoot != null)
            overlayRoot.SetActive(false);

        // Cache RectTransform board
        if (boardImage != null)
            boardRect = boardImage.GetComponent<RectTransform>();
    }

    void Update()
    {
        // Animasi scale-in untuk board
        if (isAnimating && boardRect != null)
        {
            animTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(animTimer / animationDuration);

            // Ease out elastic-like curve
            float scale = EaseOutBack(t);
            boardRect.localScale = Vector3.one * scale;

            if (t >= 1f)
            {
                isAnimating = false;
                boardRect.localScale = Vector3.one;
            }
        }
    }

    /// <summary>
    /// Menampilkan overlay VICTORY.
    /// </summary>
    /// <param name="score">Skor akhir pemain</param>
    /// <param name="customBoardSprite">Opsional: sprite board kustom (jika null, menggunakan victoryBoardSprite)</param>
    public void ShowVictory(int score, string chapterName = "", Sprite customBoardSprite = null)
    {
        if (overlayRoot == null) return;

        overlayRoot.SetActive(true);

        // Tampilkan Victory, sembunyikan Defeat
        if (victoryContent != null) victoryContent.SetActive(true);
        if (defeatContent != null) defeatContent.SetActive(false);

        // Set board background
        Sprite spriteToUse = customBoardSprite != null ? customBoardSprite : victoryBoardSprite;
        ApplyBoardSprite(spriteToUse);

        // Set board size
        ApplyBoardSize();

        // Set teks
        if (victoryTitleText != null)
            victoryTitleText.text = "VICTORY!";

        if (victoryScoreText != null)
            victoryScoreText.text = "Score: " + score;

        if (victoryMessageText != null)
        {
            if (!string.IsNullOrEmpty(chapterName))
                victoryMessageText.text = chapterName + " - Selesai!";
            else
                victoryMessageText.text = "Selamat! Kamu berhasil!";
        }

        // Mulai animasi
        StartBoardAnimation();

        // Pause game (gunakan timeScale 0 tapi animasi pakai unscaledDeltaTime)
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Menampilkan overlay DEFEAT.
    /// </summary>
    /// <param name="score">Skor akhir pemain</param>
    /// <param name="customBoardSprite">Opsional: sprite board kustom (jika null, menggunakan defeatBoardSprite)</param>
    public void ShowDefeat(int score, Sprite customBoardSprite = null)
    {
        if (overlayRoot == null) return;

        overlayRoot.SetActive(true);

        // Tampilkan Defeat, sembunyikan Victory
        if (victoryContent != null) victoryContent.SetActive(false);
        if (defeatContent != null) defeatContent.SetActive(true);

        // Set board background
        Sprite spriteToUse = customBoardSprite != null ? customBoardSprite : defeatBoardSprite;
        ApplyBoardSprite(spriteToUse);

        // Set board size
        ApplyBoardSize();

        // Set teks
        if (defeatTitleText != null)
            defeatTitleText.text = "GAME OVER";

        if (defeatScoreText != null)
            defeatScoreText.text = "Score: " + score;

        if (defeatMessageText != null)
            defeatMessageText.text = "Jangan menyerah! Coba lagi!";

        // Mulai animasi
        StartBoardAnimation();

        // Pause game
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Mengganti sprite board background secara runtime.
    /// Bisa dipanggil dari script lain tanpa harus mengedit Canvas manual.
    /// Contoh: ResultOverlay.instance.SetVictoryBoardSprite(mySprite);
    /// </summary>
    public void SetVictoryBoardSprite(Sprite newSprite)
    {
        victoryBoardSprite = newSprite;
    }

    /// <summary>
    /// Mengganti sprite board defeat secara runtime.
    /// Contoh: ResultOverlay.instance.SetDefeatBoardSprite(mySprite);
    /// </summary>
    public void SetDefeatBoardSprite(Sprite newSprite)
    {
        defeatBoardSprite = newSprite;
    }

    /// <summary>
    /// Mengganti ukuran board secara runtime.
    /// Contoh: ResultOverlay.instance.SetBoardSize(new Vector2(800, 600));
    /// </summary>
    public void SetBoardSize(Vector2 newSize)
    {
        boardSize = newSize;
        ApplyBoardSize();
    }

    // ==================== BUTTON HANDLERS ====================

    /// <summary>
    /// Tombol Continue - lanjut ke chapter berikutnya (hanya muncul di Victory).
    /// </summary>
    public void OnContinueClicked()
    {
        Time.timeScale = 1f;

        if (GameModeManager.currentChapter < GameModeManager.totalChapters)
        {
            GameModeManager.currentChapter++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // Semua chapter selesai, kembali ke main menu
            SceneManager.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// Tombol Restart - mengulang level/mode yang sama.
    /// </summary>
    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Tombol Main Menu - kembali ke menu utama.
    /// </summary>
    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // ==================== INTERNAL METHODS ====================

    /// <summary>
    /// Memasang sprite ke board Image. Sprite akan di-preserve aspect ratio
    /// sehingga gambar yang lebih kecil dari board akan ditampilkan dengan benar.
    /// </summary>
    private void ApplyBoardSprite(Sprite sprite)
    {
        if (boardImage == null) return;

        if (sprite != null)
        {
            boardImage.sprite = sprite;
            boardImage.preserveAspect = true;
            boardImage.type = Image.Type.Sliced;
            boardImage.color = Color.white;
        }
        else
        {
            // Jika tidak ada sprite, gunakan warna solid sebagai fallback
            boardImage.sprite = null;
            boardImage.color = new Color(0.15f, 0.15f, 0.25f, 0.95f);
        }
    }

    /// <summary>
    /// Mengatur ukuran board pada RectTransform.
    /// Board diposisikan di tengah layar dengan ukuran yang ditentukan (lebih kecil dari fullscreen).
    /// </summary>
    private void ApplyBoardSize()
    {
        if (boardRect == null) return;

        boardRect.anchorMin = new Vector2(0.5f, 0.5f);
        boardRect.anchorMax = new Vector2(0.5f, 0.5f);
        boardRect.pivot = new Vector2(0.5f, 0.5f);
        boardRect.anchoredPosition = Vector2.zero;
        boardRect.sizeDelta = boardSize;
    }

    /// <summary>
    /// Memulai animasi scale-in board dari kecil ke besar.
    /// </summary>
    private void StartBoardAnimation()
    {
        if (!useScaleAnimation || boardRect == null) return;

        animTimer = 0f;
        isAnimating = true;
        boardRect.localScale = Vector3.zero;
    }

    /// <summary>
    /// Ease Out Back curve untuk animasi yang lebih hidup (sedikit overshoot).
    /// </summary>
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
