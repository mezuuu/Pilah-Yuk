using UnityEngine;

public class BinController : MonoBehaviour
{
    [Header("Sprite Tong Sampah")]
    public Sprite[] binSprites;
    public float minX = -5.5f;
    public float maxX = 5.5f;
    public float smoothSpeed = 15f;

    private SpriteRenderer binRenderer;
    private Bin binComponent;
    private Camera mainCam;
    private float fixedY;
    private int currentSlot = 0;
    private BinSlotUI slotUI;

    void Awake()
    {
        binComponent = GetComponent<Bin>();
        binRenderer = GetComponent<SpriteRenderer>();

        if (binRenderer != null)
        {
            binRenderer.sortingOrder = 50; // Pastikan selalu dirender di depan background
        }

        MoveBin oldMove = GetComponent<MoveBin>();
        if (oldMove != null) oldMove.enabled = false;
    }

    void Start()
    {
        mainCam = Camera.main;
        fixedY = transform.position.y;
        SetupSlotUI();
        SwitchBin(0);
    }

    void SetupSlotUI()
    {
        slotUI = FindObjectOfType<BinSlotUI>();
        if (slotUI != null) return;

        Canvas existingCanvas = FindObjectOfType<Canvas>();
        if (existingCanvas == null) return;

        GameObject slotUIObj = new GameObject("BinSlotUI_Auto");
        slotUIObj.transform.SetParent(existingCanvas.transform, false);
        slotUI = slotUIObj.AddComponent<BinSlotUI>();
        slotUI.BuildUI(existingCanvas, binSprites);
    }

    void Update()
    {
        MoveWithCursor();
        HandleSlotInput();
    }

    void MoveWithCursor()
    {
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        float targetX = Mathf.Clamp(mouseWorld.x, minX, maxX);
        float smoothX = Mathf.Lerp(transform.position.x, targetX, smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothX, fixedY, 0f);
    }

    void HandleSlotInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            SwitchBin(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            SwitchBin(1);

        // ---------------------------------------------------------------------
        // CEK MODE: B3 (Tombol 3) HANYA BISA DIAKSES JIKA BUKAN CHAPTER 1
        // ---------------------------------------------------------------------
        bool isChapter1 = (GameModeManager.currentMode == GameModeManager.GameMode.Chapter && GameModeManager.currentChapter == 1);
        
        if (!isChapter1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                SwitchBin(2);
        }
    }

    public void SwitchBin(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 2) return;

        currentSlot = slotIndex;

        if (binRenderer != null && binSprites != null && slotIndex < binSprites.Length && binSprites[slotIndex] != null)
        {
            binRenderer.sprite = binSprites[slotIndex];
        }

        if (binComponent != null)
        {
            switch (slotIndex)
            {
                case 0: binComponent.type = Trash.TrashType.Organik; break;
                case 1: binComponent.type = Trash.TrashType.Anorganik; break;
                case 2: binComponent.type = Trash.TrashType.B3; break;
            }
        }

        if (slotUI != null)
        {
            slotUI.SelectSlot(slotIndex);
        }
    }
}
