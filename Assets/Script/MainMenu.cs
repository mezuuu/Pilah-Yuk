using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Panels Lama")]
    public GameObject panelPetunjuk;
    public GameObject panelSetting;
    
    [Header("Audio")]
    public AudioSource musicSource;
    public bool isMusicOn = true;

    [Header("Gambar Custom UI (Opsional)")]
    public Sprite boardBackground;
    public Sprite btnStoryBg;
    public Sprite btnEndlessBg;
    public Sprite btnBackBg;

    [Header("Pengaturan Tata Letak (Ubah Posisi & Ukuran)")]
    public Vector2 customBoardSize = new Vector2(700f, 400f);

    [Header("Posisi Tombol (X, Y)")]
    public Vector2 storyButtonPos = new Vector2(-2f, 20f);
    public Vector2 endlessButtonPos = new Vector2(3.4f, -49f);
    public Vector2 backButtonPos = new Vector2(-5f, -114f);

    [Header("Ukuran Tombol (Width, Height)")]
    public Vector2 storyButtonSize = new Vector2(400f, 90f);
    public Vector2 endlessButtonSize = new Vector2(400f, 90f);
    public Vector2 backButtonSize = new Vector2(250f, 80f);

    [Header("Skala Tombol (Scale X, Y)")]
    public Vector2 storyButtonScale = new Vector2(0.80002f, 1f);
    public Vector2 endlessButtonScale = new Vector2(0.79999f, 1.13f);
    public Vector2 backButtonScale = new Vector2(0.96f, 0.74f);

    [Header("Font Settings")]
    public TMP_FontAsset customFont;

    private GameObject modeSelectionOverlay;

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

    void Start()
    {
        TMP_Text[] allTexts = FindObjectsOfType<TMP_Text>(true);
        foreach(var t in allTexts)
        {
            ApplyFontAndOutline(t);
        }

        int music = PlayerPrefs.GetInt("Music", 1);
        isMusicOn = music == 1;

        if (musicSource != null)
            musicSource.mute = !isMusicOn;
    }

    public void StartGame()
    {
        CreateModeSelectionUI();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        if (musicSource != null)
            musicSource.mute = !isMusicOn;
        PlayerPrefs.SetInt("Music", isMusicOn ? 1 : 0);
    }

    public void OpenTutorial()
    {
        if (panelPetunjuk != null) panelPetunjuk.SetActive(true);
    }

    public void CloseTutorial()
    {
        if (panelPetunjuk != null) panelPetunjuk.SetActive(false);
    }

    public void OpenSetting()
    {
        if (panelSetting != null) panelSetting.SetActive(true);
    }

    public void CloseSetting()
    {
        if (panelSetting != null) panelSetting.SetActive(false);
    }

    // =========================================================================
    // AUTO GENERATE MODE SELECTION UI
    // =========================================================================

    void CreateModeSelectionUI()
    {
        if (modeSelectionOverlay != null) return;

        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null) return;

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
        }

        modeSelectionOverlay = new GameObject("ModeSelectionOverlay");
        modeSelectionOverlay.transform.SetParent(mainCanvas.transform, false);
        modeSelectionOverlay.transform.SetAsLastSibling();

        RectTransform overlayRect = modeSelectionOverlay.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        Image overlayBg = modeSelectionOverlay.AddComponent<Image>();
        overlayBg.color = new Color(0f, 0f, 0f, 0.8f);

        // -----------------------------------------------------------------
        // BOARD UTAMA DISET 700x400 AGAR SAMA DENGAN WIN/LOSE
        // -----------------------------------------------------------------
        GameObject boardObj = new GameObject("Board");
        boardObj.transform.SetParent(modeSelectionOverlay.transform, false);
        RectTransform boardRect = boardObj.AddComponent<RectTransform>();
        boardRect.sizeDelta = customBoardSize; // Menggunakan variabel yang bisa diatur di inspector
        boardRect.anchoredPosition = Vector2.zero;

        Image boardBgImg = boardObj.AddComponent<Image>();
        if (boardBackground != null)
        {
            boardBgImg.sprite = boardBackground;
            boardBgImg.type = Image.Type.Sliced; // Diubah ke Sliced agar mematuhi ukuran mutlak di inspector
            boardBgImg.color = Color.white;
        }
        else
        {
            boardBgImg.color = new Color(0.12f, 0.15f, 0.22f, 1f);
        }

        if (boardBackground == null)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(boardObj.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0f, -40f);
            titleRect.sizeDelta = new Vector2(0f, 60f);

            TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "PILIH MODE GAME";
            ApplyFontAndOutline(titleTMP);
            titleTMP.fontSize = 45f;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = Color.white;
        }

        // Posisi, Ukuran, & Skala dari variabel inspector
        CreateButton(boardObj.transform, "STORY MODE", new Color(0.8f, 0.4f, 0.1f), storyButtonPos, storyButtonSize, storyButtonScale, btnStoryBg, PlayStoryMode);
        CreateButton(boardObj.transform, "ENDLESS MODE", new Color(0.2f, 0.6f, 0.8f), endlessButtonPos, endlessButtonSize, endlessButtonScale, btnEndlessBg, PlayEndlessMode);
        CreateButton(boardObj.transform, "KEMBALI", new Color(0.4f, 0.4f, 0.4f), backButtonPos, backButtonSize, backButtonScale, btnBackBg, CloseModeSelection);
    }

    void CreateButton(Transform parent, string text, Color fallbackColor, Vector2 anchoredPos, Vector2 size, Vector2 scale, Sprite customSprite, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = new GameObject("Button_" + text);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.sizeDelta = size; // Dari parameter Vector2
        rect.anchoredPosition = anchoredPos;
        btnObj.transform.localScale = new Vector3(scale.x, scale.y, 1f); // Terapkan scale

        Image bg = btnObj.AddComponent<Image>();
        if (customSprite != null)
        {
            bg.sprite = customSprite;
            bg.type = Image.Type.Sliced; // Diubah ke Sliced agar ukurannya pas 100% mengikuti Width & Height
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
            tmp.fontSize = 24f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;
            tmp.color = Color.white;
        }
    }

    void PlayStoryMode()
    {
        GameModeManager.currentMode = GameModeManager.GameMode.Chapter;
        GameModeManager.currentChapter = 1;
        SceneManager.LoadScene("milahsampah");
    }

    void PlayEndlessMode()
    {
        GameModeManager.currentMode = GameModeManager.GameMode.Endless;
        SceneManager.LoadScene("milahsampah");
    }

    void CloseModeSelection()
    {
        if (modeSelectionOverlay != null)
        {
            Destroy(modeSelectionOverlay);
        }
    }
}