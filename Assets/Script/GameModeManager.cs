/// <summary>
/// Static manager yang menyimpan mode permainan dan chapter yang sedang aktif.
/// Data ini digunakan untuk komunikasi antar scene tanpa perlu DontDestroyOnLoad.
/// </summary>
public static class GameModeManager
{
    public enum GameMode { Endless, Chapter }

    public static GameMode currentMode = GameMode.Endless;
    public static int currentChapter = 1;

    // Jumlah chapter yang tersedia saat ini
    public static int totalChapters = 1;

    public static void Reset()
    {
        currentMode = GameMode.Endless;
        currentChapter = 1;
    }
}
