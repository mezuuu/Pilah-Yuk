using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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
    public DialogLine[] prologLines; 
    public DialogLine[] epilogLines; 

    // ==========================================
    // VARIABEL MEMORI UNTUK RESTART
    // ==========================================
    public static bool hasSeenProlog = false; 

    private DialogLine[] currentLines; 
    private int index = 0;
    private bool isTyping = false;

    void Start()
    {
        if (GameModeManager.currentMode == GameModeManager.GameMode.Chapter)
        {
            // KONDISI A: JALANKAN PROLOG (HANYA JIKA CHAPTER 1 & BELUM PERNAH DILIHAT)
            if (GameModeManager.currentChapter == 1 && !hasSeenProlog)
            {
                hasSeenProlog = true; // Langsung tandai bahwa prolog sudah dilihat
                currentLines = prologLines;
                StartDialogSequence();
            }
            // KONDISI B: JALANKAN EPILOG (CHAPTER 3)
            else if (GameModeManager.currentChapter == 3)
            {
                currentLines = epilogLines;
                StartDialogSequence();
            }
            // KONDISI C: LANGSUNG MAIN (RESTART CH 1, ATAU MASUK CH 2)
            else 
            {
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
        if (playerBin != null) playerBin.SetActive(false);
        if (spawner != null) spawner.SetActive(false);
        
        if (dialogPanel != null) dialogPanel.SetActive(true);
        if (ekoImage != null) ekoImage.gameObject.SetActive(true);

        Time.timeScale = 0f; 
        index = 0;
        StartCoroutine(TypeLine());
    }

    void StartGameplay()
    {
        Time.timeScale = 1f; 
        if (playerBin != null) playerBin.SetActive(true);
        if (spawner != null) spawner.SetActive(true);

        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (ekoImage != null) ekoImage.gameObject.SetActive(false);

        gameObject.SetActive(false); 
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
            if (GameModeManager.currentChapter == 3)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu"); 
            }
            else
            {
                StartGameplay();
            }
        }
    }
}