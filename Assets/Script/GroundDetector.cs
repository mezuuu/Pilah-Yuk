using UnityEngine;

/// <summary>
/// Mendeteksi sampah yang jatuh ke tanah (melewati batas bawah layar).
/// Letakkan GameObject dengan BoxCollider2D (trigger) di bagian bawah layar.
/// Ketika sampah menyentuh tanah, pemain kehilangan nyawa.
/// </summary>
public class GroundDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah objek yang masuk adalah sampah
        Trash trash = other.GetComponent<Trash>();
        if (trash != null)
        {
            // Pemain kehilangan nyawa karena sampah jatuh ke tanah
            if (GameManager.instance != null)
            {
                GameManager.instance.LoseLife();
            }

            // Hancurkan sampah
            Destroy(other.gameObject);
        }
    }
}
