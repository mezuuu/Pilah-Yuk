using UnityEngine;

/// <summary>
/// Dulu script ini digunakan untuk menggerakkan objek menggunakan A/D atau Panah Kiri/Kanan.
/// Karena sekarang mekanik gamenya diubah (hanya Bin utama yang bergerak dengan kursor),
/// fungsi gerak di script ini dimatikan secara sengaja agar sampah tidak bisa dikontrol.
/// </summary>
public class MoveBin : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // KODE LAMA (dimatikan agar sampah tidak bisa dikendalikan oleh A/D)
        // float move = Input.GetAxis("Horizontal");
        // transform.Translate(new Vector2(move * speed * Time.deltaTime, 0));
    }
}