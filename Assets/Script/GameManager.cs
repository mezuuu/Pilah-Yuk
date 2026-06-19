using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Lama (Akan disembunyikan)")]
    public GameObject gameOverPanel;
    
    [Header("UI HUD Utama")]
    public Image[] hearts;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI highScoreText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip gameOverSound;
    public AudioClip victorySound;

    [Header("Gambar Custom UI (Opsional)")]
    public Sprite victoryBoardBackground;
    public Sprite gameOverBoardBackground;
    public Sprite btnLanjutBg;
    public Sprite btnRestartBg;
    public Sprite btnMainMenuBg;
    public Sprite starColored;
    public Sprite starUncolored;

    [Header("Pengaturan Bintang (Story Mode)")]
    public Vector2 starSize = new Vector2(80f, 80f);
    public float starSpacing = 100f; // Jarak antar bintang (Kiri, Tengah, Kanan)
    public float starContainerY = 10f; // Posisi Naik-Turun bintang

    [Header("Font Settings")]
    public TMP_FontAsset customFont;

    [Header("Background In-Game Settings")]
    [Tooltip("Drag objek background kamu (bisa berupa komponen Image di UI atau SpriteRenderer di scene 2D)")]
    public UnityEngine.UI.Image uiBackground;
    public SpriteRenderer spriteBackground;
    [Space]
    public Sprite endlessBackground;
    public Sprite chapter1Background;
    public Sprite chapter2Background;
    public Sprite chapter3Background;

    [Header("Pengaturan Tata Letak Board (Ukuran & Posisi)")]
    public Vector2 customBoardSize = new Vector2(700f, 400f);
    public float buttonsContainerX = 0f; // Posisi X (Geser Kiri/Kanan) kumpulan tombol
    public float buttonsContainerY = 50f; // Jarak posisi kumpulan tombol dari bawah/tengah
    public Vector2 buttonSize = new Vector2(250f, 85f);

    [Header("Posisi Tombol saat Menang (X, Y)")]
    public Vector2 winBtnLanjutPos = new Vector2(-260f, 0f);
    public Vector2 winBtnRestartPos = new Vector2(0f, 0f);
    public Vector2 winBtnMainMenuPos = new Vector2(260f, 0f);

    [Header("Posisi Tombol saat Kalah (X, Y)")]
    public Vector2 loseBtnRestartPos = new Vector2(-150f, 0f);
    public Vector2 loseBtnMainMenuPos = new Vector2(150f, 0f);

    [HideInInspector] public int score = 0;
    [HideInInspector] public int highScore = 0;
    [HideInInspector] public int life = 3;
    [HideInInspector] public string title = "Pemula";
    
    private bool isGameOverOrWin = false;

    void Awake()
    {
        instance = this;
    }

    public GameObject floatingTextPrefab;

    public void ApplyFontAndOutline(TMP_Text tmp)
    {
        if (tmp == null) return;
        if (customFont != null) tmp.font = customFont;
        
        tmp.fontStyle |= FontStyles.Bold;

        try
        {
            if (tmp.fontSharedMaterial != null)
            {
                Material outlineMat = new Material(tmp.fontSharedMaterial);
                outlineMat.SetFloat("_OutlineWidth", 0.2f);
                outlineMat.SetColor("_OutlineColor", Color.black);
                tmp.fontSharedMaterial = outlineMat;
            }
        }
        catch { }
    }

    public void ShowFloatingText(Vector3 pos)
    {
        GameObject floater = Instantiate(floatingTextPrefab, pos, Quaternion.identity);
        TMP_Text tmp = floater.GetComponentInChildren<TMP_Text>();
        ApplyFontAndOutline(tmp);
    }

    void Start()
    {
        // Fitur ini dimatikan agar tidak mengubah font teks lain (seperti Dialog) secara otomatis
        // TMP_Text[] allTexts = FindObjectsOfType<TMP_Text>(true);
        // foreach(var t in allTexts)
        // {
        //     ApplyFontAndOutline(t);
        // }

        ApplyBackground();

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();
        
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void ApplyBackground()
    {
        Sprite bgToUse = null;

        if (GameModeManager.currentMode == GameModeManager.GameMode.Endless)
        {
            bgToUse = endlessBackground;
        }
        else if (GameModeManager.currentMode == GameModeManager.GameMode.Chapter)
        {
            if (GameModeManager.currentChapter == 1) bgToUse = chapter1Background;
            else if (GameModeManager.currentChapter == 2) bgToUse = chapter2Background;
            else if (GameModeManager.currentChapter == 3) bgToUse = chapter3Background;
            // Jika butuh lebih banyak chapter, bisa ditambah di sini
        }

        if (bgToUse != null)
        {
            if (uiBackground != null) uiBackground.sprite = bgToUse;
            if (spriteBackground != null) 
            {
                spriteBackground.sprite = bgToUse;
                spriteBackground.sortingOrder = -50; // Paksa render di belakang semua objek game
            }
        }
    }

    public void AddScore(int value)
    {
        if (isGameOverOrWin) return;

        score += value;

        if (audioSource != null && correctSound != null)
            audioSource.PlayOneShot(correctSound);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        UpdateTitle();
        UpdateUI();

        if (GameModeManager.currentMode == GameModeManager.GameMode.Chapter)
        {
            int targetScore = GameModeManager.currentChapter * 150; // Ch 1 = 150, Ch 2 = 300, dst.
            if (score >= targetScore)
            {
                Victory();
            }
        }
    }

    public void LoseLife()
    {
        if (isGameOverOrWin) return;

        life--;

        if (audioSource != null && wrongSound != null)
            audioSource.PlayOneShot(wrongSound);

        if (life >= 0 && life < hearts.Length)
        {
            hearts[life].gameObject.SetActive(false);
        }

        if (life <= 0)
        {
            GameOver();
        }
    }

    void UpdateTitle()
    {
        if (score >= 100) title = "Pahlawan Bumi";
        else if (score >= 50) title = "Peduli Lingkungan";
        else title = "Pemula";
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            if (GameModeManager.currentMode == GameModeManager.GameMode.Chapter)
            {
                int targetScore = GameModeManager.currentChapter * 150;
                scoreText.text = "Score: " + score + " / " + targetScore;
            }
            else
                scoreText.text = "Score: " + score;
        }
        
        if (titleText != null) titleText.text = title;
        if (highScoreText != null) highScoreText.text = "High Score: " + highScore;
    }

    void GameOver()
    {
        isGameOverOrWin = true;
        StopBackgroundMusic();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (audioSource != null && gameOverSound != null) audioSource.PlayOneShot(gameOverSound);
        CreateModernOverlayUI(false);
        Time.timeScale = 0f;
    }

    void Victory()
    {
        isGameOverOrWin = true;
        StopBackgroundMusic();
        if (audioSource != null && victorySound != null) audioSource.PlayOneShot(victorySound);
        CreateModernOverlayUI(true);
        Time.timeScale = 0f;
    }

    void StopBackgroundMusic()
    {
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudio)
        {
            if (source != audioSource && source.isPlaying)
                source.Stop();
        }
    }

    // =========================================================================
    // AUTO GENERATE OVERLAY 
    // =========================================================================
    
    void CreateModernOverlayUI(bool isWin)
    {
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null) return;

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
        }

        GameObject overlayObj = new GameObject("Overlay_WinLose");
        overlayObj.transform.SetParent(mainCanvas.transform, false);
        RectTransform overlayRect = overlayObj.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        
        Image overlayBg = overlayObj.AddComponent<Image>();
        overlayBg.color = new Color(0f, 0f, 0f, 0.8f);

        // -----------------------------------------------------------------
        // BOARD UTAMA DISET 700x400 UNTUK SEMUA PANEL (WIN/LOSE/MODE)
        // -----------------------------------------------------------------
        GameObject boardObj = new GameObject("Board");
        boardObj.transform.SetParent(overlayObj.transform, false);
        RectTransform boardRect = boardObj.AddComponent<RectTransform>();
        boardRect.sizeDelta = customBoardSize;
        boardRect.anchoredPosition = Vector2.zero;

        Sprite currentBoardBg = isWin ? victoryBoardBackground : gameOverBoardBackground;

        Image boardBgImg = boardObj.AddComponent<Image>();
        if (currentBoardBg != null)
        {
            boardBgImg.sprite = currentBoardBg;
            boardBgImg.type = Image.Type.Sliced;
            boardBgImg.color = Color.white; // Pakai warna gambar asli
        }
        else
        {
            boardBgImg.color = new Color(0.12f, 0.15f, 0.22f, 1f); // Warna default
        }

        if (currentBoardBg == null)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(boardObj.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0f, -40f);
            titleRect.sizeDelta = new Vector2(0f, 70f);

            TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
            titleTMP.text = isWin ? "CHAPTER SELESAI!" : "GAME OVER";
            ApplyFontAndOutline(titleTMP);
            titleTMP.fontSize = 55f;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = isWin ? new Color(0.2f, 0.9f, 0.4f, 1f) : new Color(0.95f, 0.3f, 0.3f, 1f);
        }

        if (GameModeManager.currentMode == GameModeManager.GameMode.Chapter)
        {
            // Tampilkan Bintang untuk Story Mode
            GameObject starsContainer = new GameObject("StarsContainer");
            starsContainer.transform.SetParent(boardObj.transform, false);
            RectTransform starsRect = starsContainer.AddComponent<RectTransform>();
            starsRect.anchoredPosition = new Vector2(0f, starContainerY); // Menggunakan settingan inspector
            starsRect.sizeDelta = new Vector2(300f, 100f);

            int starsEarned = isWin ? life : 0; 
            if (starsEarned < 0) starsEarned = 0;
            if (starsEarned > 3) starsEarned = 3;

            for (int i = 0; i < 3; i++)
            {
                GameObject starObj = new GameObject("Star_" + (i + 1));
                starObj.transform.SetParent(starsContainer.transform, false);
                RectTransform starRect = starObj.AddComponent<RectTransform>();
                
                // Menata 3 bintang berjejer: -spacing, 0, +spacing
                starRect.anchoredPosition = new Vector2(-starSpacing + (i * starSpacing), 0f); 
                starRect.sizeDelta = starSize; // Menggunakan settingan inspector

                Image starImg = starObj.AddComponent<Image>();
                starImg.sprite = (i < starsEarned) ? starColored : starUncolored;
                starImg.preserveAspect = true; // Mencegah gambar gepeng
            }
        }
        else
        {
            // Tampilkan Score untuk Endless Mode
            GameObject scoreObj = new GameObject("ScoreInfo");
            scoreObj.transform.SetParent(boardObj.transform, false);
            RectTransform scoreRect = scoreObj.AddComponent<RectTransform>();
            scoreRect.anchoredPosition = new Vector2(0f, 10f); // Posisi text agak ke tengah
            scoreRect.sizeDelta = new Vector2(500f, 100f);

            TextMeshProUGUI scoreTMP = scoreObj.AddComponent<TextMeshProUGUI>();
            scoreTMP.text = "Skor Kamu: <color=#FFD700>" + score + "</color>\nHigh Skor: <color=#FFA500>" + highScore + "</color>";
            ApplyFontAndOutline(scoreTMP);
            scoreTMP.fontSize = 32f;
            scoreTMP.alignment = TextAlignmentOptions.Center;
            scoreTMP.color = Color.white;
        }

        GameObject btnContainer = new GameObject("Buttons");
        btnContainer.transform.SetParent(boardObj.transform, false);
        RectTransform btnRect = btnContainer.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0f);
        btnRect.anchorMax = new Vector2(0.5f, 0f);
        btnRect.pivot = new Vector2(0.5f, 0f);
        btnRect.anchoredPosition = new Vector2(buttonsContainerX, buttonsContainerY);
        btnRect.sizeDelta = new Vector2(650f, 70f);

        if (isWin)
        {
            CreateButton(btnContainer.transform, "LANJUT", new Color(0.2f, 0.8f, 0.2f), winBtnLanjutPos, buttonSize, btnLanjutBg, NextChapter);
            CreateButton(btnContainer.transform, "RESTART", new Color(0.8f, 0.5f, 0.1f), winBtnRestartPos, buttonSize, btnRestartBg, RestartGame);
            CreateButton(btnContainer.transform, "MAIN MENU", new Color(0.2f, 0.5f, 0.9f), winBtnMainMenuPos, buttonSize, btnMainMenuBg, GoToMainMenu);
        }
        else
        {
            CreateButton(btnContainer.transform, "RESTART", new Color(0.8f, 0.5f, 0.1f), loseBtnRestartPos, buttonSize, btnRestartBg, RestartGame);
            CreateButton(btnContainer.transform, "MAIN MENU", new Color(0.2f, 0.5f, 0.9f), loseBtnMainMenuPos, buttonSize, btnMainMenuBg, GoToMainMenu);
        }
    }

    void CreateButton(Transform parent, string text, Color fallbackColor, Vector2 anchoredPos, Vector2 size, Sprite customSprite, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = new GameObject("Button_" + text);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        Image bg = btnObj.AddComponent<Image>();
        if (customSprite != null)
        {
            bg.sprite = customSprite;
            bg.type = Image.Type.Sliced;
            bg.color = Color.white;
        }
        else
        {
            bg.color = fallbackColor;
        }

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(action);

        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.8f, 0.8f, 0.8f);
        colors.pressedColor = new Color(0.6f, 0.6f, 0.6f);
        btn.colors = colors;

        if (customSprite == null)
        {
            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            RectTransform txtRect = txtObj.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            ApplyFontAndOutline(tmp);
            tmp.fontSize = 26f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;
            tmp.color = Color.white;
        }
    }

    public void NextChapter()
    {
        Time.timeScale = 1f;
        GameModeManager.currentChapter++; // Naikkan angka chapter (ke 2, 3, dst)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Muat ulang scene dengan setting chapter baru
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}