using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Masukkan 3 Prefab Sampah (Organik, Anorganik, B3) di sini")]
    public GameObject[] trashPrefabs;

    [Header("Pengaturan Waktu")]
    [Tooltip("Waktu tunggu (detik) antar setiap sampah yang jatuh")]
    public float spawnDelay = 2f;

    [Header("Batas Posisi Jatuh (Kiri - Kanan)")]
    [Tooltip("Diberi jarak aman agar tidak tertutup UI Nyawa (Kiri) & Setting (Kanan)")]
    public float minX = -5.5f;
    public float maxX = 5.5f;
    
    public float spawnY = 6f;

    [Header("Endless Mode - Tingkat Kesulitan")]
    [Tooltip("Kecepatan awal jatuhnya sampah")]
    public float initialTrashSpeed = 3f;
    [Tooltip("Kecepatan maksimum jatuhnya sampah")]
    public float maxTrashSpeed = 10f;
    [Tooltip("Waktu (detik) yang dibutuhkan untuk menambah kecepatan")]
    public float speedIncreaseInterval = 10f;
    [Tooltip("Seberapa besar penambahan kecepatannya?")]
    public float speedIncreaseAmount = 0.5f;

    private float currentTrashSpeed;
    private float speedIncreaseTimer = 0f;

    void Start()
    {
        // ---------------------------------------------------------------------
        // FILTER UNTUK STORY MODE CHAPTER 1: HANYA ORGANIK & ANORGANIK
        // ---------------------------------------------------------------------
        if (GameModeManager.currentMode == GameModeManager.GameMode.Chapter && GameModeManager.currentChapter == 1)
        {
            System.Collections.Generic.List<GameObject> filteredPrefabs = new System.Collections.Generic.List<GameObject>();
            
            foreach (GameObject prefab in trashPrefabs)
            {
                if (prefab != null)
                {
                    Trash trashComponent = prefab.GetComponent<Trash>();
                    // Jangan masukkan sampah tipe B3 ke dalam list spawn
                    if (trashComponent != null && trashComponent.type != Trash.TrashType.B3)
                    {
                        filteredPrefabs.Add(prefab);
                    }
                }
            }
            // Ganti array utama dengan array yang sudah difilter (tanpa B3)
            trashPrefabs = filteredPrefabs.ToArray();
        }

        currentTrashSpeed = initialTrashSpeed;
        InvokeRepeating("SpawnTrash", 1f, spawnDelay);
    }

    void Update()
    {
        if (GameModeManager.currentMode == GameModeManager.GameMode.Endless)
        {
            speedIncreaseTimer += Time.deltaTime;
            if (speedIncreaseTimer >= speedIncreaseInterval)
            {
                speedIncreaseTimer = 0f;
                if (currentTrashSpeed < maxTrashSpeed)
                {
                    currentTrashSpeed += speedIncreaseAmount;
                    if (currentTrashSpeed > maxTrashSpeed) 
                        currentTrashSpeed = maxTrashSpeed;
                        
                    Debug.Log("Kecepatan sampah meningkat jadi: " + currentTrashSpeed);
                }
            }
        }
    }

    void SpawnTrash()
    {
        if (trashPrefabs == null || trashPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, trashPrefabs.Length);
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        GameObject spawned = Instantiate(trashPrefabs[randomIndex], spawnPosition, Quaternion.identity);

        if (GameModeManager.currentMode == GameModeManager.GameMode.Endless)
        {
            Trash trashComp = spawned.GetComponent<Trash>();
            if (trashComp != null)
            {
                trashComp.speed = currentTrashSpeed;
            }
        }
    }
}