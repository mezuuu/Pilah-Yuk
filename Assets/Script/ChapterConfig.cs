using UnityEngine;

/// <summary>
/// ScriptableObject untuk menyimpan konfigurasi tiap chapter.
/// Buat di Unity: Right-click di Project > Create > Game > Chapter Config
/// 
/// Contoh Chapter 1 - Taman Kota:
///   - chapterName: "Taman Kota"
///   - targetScore: 150
///   - spawnDelay: 2f
///   - trashSpeed: 3f
/// </summary>
[CreateAssetMenu(fileName = "Chapter", menuName = "Game/Chapter Config")]
public class ChapterConfig : ScriptableObject
{
    [Header("Info Chapter")]
    public string chapterName = "Taman Kota";
    public string chapterDescription = "Bersihkan taman kota dari sampah!";
    public int chapterNumber = 1;

    [Header("Gameplay")]
    [Tooltip("Skor yang harus dicapai untuk menang")]
    public int targetScore = 150;

    [Tooltip("Delay antar spawn sampah (detik)")]
    public float spawnDelay = 2f;

    [Tooltip("Kecepatan jatuh sampah")]
    public float trashSpeed = 3f;

    [Tooltip("Jumlah nyawa pemain")]
    public int startingLives = 3;

    [Header("Visual")]
    [Tooltip("Background scene untuk chapter ini")]
    public Sprite backgroundSprite;

    [Tooltip("Sprite board untuk overlay Victory")]
    public Sprite victoryBoardSprite;

    [Tooltip("Sprite board untuk overlay Defeat")]
    public Sprite defeatBoardSprite;

    [Header("Trash Prefabs (opsional, jika chapter punya sampah khusus)")]
    [Tooltip("Kosongkan jika menggunakan trash prefab default dari Spawner")]
    public GameObject[] customTrashPrefabs;
}
