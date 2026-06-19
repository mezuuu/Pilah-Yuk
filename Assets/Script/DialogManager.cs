using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // Wajib ditambahkan untuk memuat Main Menu

[System.Serializable]
public class DialogLine
{
    [TextArea(3, 10)]
    public string text;
    public Sprite expression;
}

public class DialogManager : MonoBehaviour
{
    [Header("UI Komponen")]
    public GameObject dialogPanel; 
    public TextMeshProUGUI dialogText;
    public Image ekoImage;
    
    [Header("Gameplay Elements (Sembunyikan saat dialog)")]
    public GameObject playerBin;
    public GameObject spawner;

    [Header("Pengaturan Dialog")]
    public float typingSpeed = 0.04f;
    public DialogLine[] prologLines; // Wadah khusus naskah awal (Chapter 1)
    public DialogLine[] epilogLines; // Wadah khusus naskah akhir (Chapter 3)

    private DialogLine[] currentLines; // Penunjuk naskah yang sedang aktif
    private int index = 0;
    private bool isTyping = false;

    void Start()
    {
        if (GameModeManager.currentMode == GameModeManager.GameMode.Chapter)
        {
            if (GameModeManager.currentChapter == 1)
            {
                // ==========================================
                // KONDISI A: JALANKAN PROLOG (CHAPTER 1)
                // ==========================================
                currentLines = prologLines;
                StartDialogSequence();
            }
            else if (GameModeManager.currentChapter == 3)
            {
                // ==========================================
                // KONDISI B: JALANKAN EPILOG (CHAPTER 3)
                // ==========================================
                currentLines = epilogLines;
                StartDialogSequence();
            }
            else 
            {
                // ==========================================
                // KONDISI C: LANGSUNG MAIN (CHAPTER 2)
                // ==========================================
                StartGameplay();
            }
        }
        else
        {
            // JIKA ENDLESS MODE, LANGSUNG MAIN
            StartGameplay();
        }
    }

    void StartDialogSequence()
    {
        // Matikan gameplay, nyalakan UI Eko
        if (playerBin != null) playerBin.SetActive(false);
        if (spawner != null) spawner.SetActive(false);
        
        if (dialogPanel != null) dialogPanel.SetActive(true);
        if (ekoImage != null) ekoImage.gameObject.SetActive(true);

        Time.timeScale = 0f; // Bekukan waktu
        index = 0;
        StartCoroutine(TypeLine());
    }

    void StartGameplay()
    {
        Time.timeScale = 1f; // Jalankan waktu
        if (playerBin != null) playerBin.SetActive(true);
        if (spawner != null) spawner.SetActive(true);

        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (ekoImage != null) ekoImage.gameObject.SetActive(false);

        gameObject.SetActive(false); // Matikan script manager ini
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        ekoImage.sprite = currentLines[index].expression;
        dialogText.text = "";
        
        foreach (char c in currentLines[index].text.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed); 
        }
        
        isTyping = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogText.text = currentLines[index].text;
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    void NextLine()
    {
        if (index < currentLines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            // ==========================================
            // LOGIKA SAAT TEKS TERAKHIR SELESAI DIKLIK
            // ==========================================
            if (GameModeManager.currentChapter == 3)
            {
                // JIKA EPILOG SELESAI: Waktu dinormalkan & Kembali ke Main Menu
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu"); 
            }
            else
            {
                // JIKA PROLOG SELESAI: Langsung mulai permainan
                StartGameplay();
            }
        }
    }
}