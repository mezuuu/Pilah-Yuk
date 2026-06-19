using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BinSlotUI : MonoBehaviour
{
    public static BinSlotUI instance;

    private Image[] slotIcons;
    private RectTransform[] slotTransforms;
    private TextMeshProUGUI[] slotKeyLabels;
    private Image[] slotBackgrounds;

    [Header("Scale Settings")]
    public float normalScale = 1.0f;
    public float selectedScale = 1.35f;
    public float scaleSpeed = 12f;

    private int currentSelected = 0;
    private float[] targetScales;
    private bool isReady = false;

    private static readonly Color organikColor = new Color(0.18f, 0.72f, 0.28f, 1f);
    private static readonly Color anorganikColor = new Color(0.20f, 0.50f, 0.85f, 1f);
    private static readonly Color b3Color = new Color(0.85f, 0.22f, 0.22f, 1f);

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (!isReady) return;
        AnimateSlots();
    }

    public void BuildUI(Canvas parentCanvas, Sprite[] sprites)
    {
        GameObject panelObj = new GameObject("BinSlotPanel");
        panelObj.transform.SetParent(parentCanvas.transform, false);

        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0f, 15f);
        panelRect.sizeDelta = new Vector2(300f, 90f);

        Image panelBg = panelObj.AddComponent<Image>();
        panelBg.color = new Color(0f, 0f, 0f, 0.45f);

        HorizontalLayoutGroup layout = panelObj.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 12f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.padding = new RectOffset(15, 15, 8, 8);
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        // ---------------------------------------------------------------------
        // CEK JUMLAH SLOT BERDASARKAN MODE
        // ---------------------------------------------------------------------
        int slotCount = 3;
        if (GameModeManager.currentMode == GameModeManager.GameMode.Chapter && GameModeManager.currentChapter == 1)
        {
            slotCount = 2; // Chapter 1 hanya ada Organik dan Anorganik
        }

        string[] slotNames = { "Organik", "Anorganik", "B3" };
        string[] keyLabels = { "1", "2", "3" };
        Color[] slotColors = { organikColor, anorganikColor, b3Color };

        slotIcons = new Image[slotCount];
        slotTransforms = new RectTransform[slotCount];
        slotKeyLabels = new TextMeshProUGUI[slotCount];
        slotBackgrounds = new Image[slotCount];
        targetScales = new float[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = new GameObject("Slot_" + slotNames[i]);
            slotObj.transform.SetParent(panelObj.transform, false);

            RectTransform slotRect = slotObj.AddComponent<RectTransform>();
            slotRect.sizeDelta = new Vector2(75f, 75f);
            slotTransforms[i] = slotRect;

            Image slotBg = slotObj.AddComponent<Image>();
            slotBg.color = slotColors[i] * 0.7f;
            slotBackgrounds[i] = slotBg;

            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(slotObj.transform, false);

            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.2f);
            iconRect.anchorMax = new Vector2(0.9f, 0.95f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            Image iconImg = iconObj.AddComponent<Image>();
            slotIcons[i] = iconImg;

            if (sprites != null && i < sprites.Length && sprites[i] != null)
            {
                iconImg.sprite = sprites[i];
                iconImg.preserveAspect = true;
                iconImg.color = Color.white;
            }
            else
            {
                iconImg.sprite = null;
                iconImg.color = slotColors[i];
            }

            GameObject labelObj = new GameObject("KeyLabel");
            labelObj.transform.SetParent(slotObj.transform, false);

            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 0.25f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            TextMeshProUGUI labelTMP = labelObj.AddComponent<TextMeshProUGUI>();
            labelTMP.text = (i + 1).ToString();
            labelTMP.fontSize = 24f;
            labelTMP.fontStyle = FontStyles.Bold;
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.color = Color.white;
            if (GameManager.instance != null) GameManager.instance.ApplyFontAndOutline(labelTMP);
            slotKeyLabels[i] = labelTMP;

            targetScales[i] = normalScale;
        }

        isReady = true;
        SelectSlot(0);
    }

    public void SelectSlot(int index)
    {
        if (!isReady || index < 0 || index >= slotTransforms.Length) return;

        currentSelected = index;

        for (int i = 0; i < slotTransforms.Length; i++)
        {
            bool isSelected = (i == index);

            targetScales[i] = isSelected ? selectedScale : normalScale;

            if (slotIcons[i] != null)
            {
                if (slotIcons[i].sprite != null)
                    slotIcons[i].color = isSelected ? Color.white : new Color(1f, 1f, 1f, 0.45f);
                else
                {
                    Color[] colors = { organikColor, anorganikColor, b3Color };
                    slotIcons[i].color = isSelected ? colors[i] : colors[i] * 0.5f;
                }
            }

            if (slotBackgrounds[i] != null)
            {
                Color[] colors = { organikColor, anorganikColor, b3Color };
                slotBackgrounds[i].color = isSelected ? colors[i] : colors[i] * 0.5f;
            }

            if (slotKeyLabels[i] != null)
            {
                slotKeyLabels[i].color = isSelected ? new Color(1f, 0.92f, 0.3f, 1f) : new Color(1f, 1f, 1f, 0.45f);
            }
        }
    }

    void AnimateSlots()
    {
        for (int i = 0; i < slotTransforms.Length; i++)
        {
            if (slotTransforms[i] == null) continue;
            float current = slotTransforms[i].localScale.x;
            float target = targetScales[i];
            float newScale = Mathf.Lerp(current, target, scaleSpeed * Time.deltaTime);
            slotTransforms[i].localScale = new Vector3(newScale, newScale, 1f);
        }
    }
}
