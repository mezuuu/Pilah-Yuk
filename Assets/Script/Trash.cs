using UnityEngine;

public class Trash : MonoBehaviour
{
    public enum TrashType { Organik, Anorganik, B3 }
    public TrashType type;

    public float speed = 3f;

    [Tooltip("Batas posisi Y di mana sampah dianggap 'jatuh ke tanah' (gagal ditangkap)")]
    public float missYThreshold = -5.5f;

    private Rigidbody2D rb;
    private bool isProcessed = false; // Mencegah double trigger

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = 60; // Render di depan tong sampah (yang diset 50)
        
        // Sampah bergerak lurus ke bawah dengan sendirinya, 
        // tidak ada input pergerakan dari player di sini.
        rb.velocity = Vector2.down * speed;
    }

    void Update()
    {
        // Jika sampah jatuh melewati batas bawah (missYThreshold) tanpa mengenai tong
        if (!isProcessed && transform.position.y < missYThreshold)
        {
            isProcessed = true;
            Debug.Log("SAMPAH JATUH KE TANAH!");
            
            // Kurangi darah/nyawa player
            if (GameManager.instance != null)
            {
                GameManager.instance.LoseLife();
            }
            
            // Hancurkan sampah
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isProcessed) return;

        Bin bin = other.GetComponent<Bin>();

        if (bin != null)
        {
            isProcessed = true;

            if (bin.type == type)
            {
                Debug.Log("BENAR");

                GameManager.instance.AddScore(10);
                Vector3 hitPos = other.bounds.center;
                GameManager.instance.ShowFloatingText(hitPos + Vector3.up * 0.5f);
            }
            else
            {
                Debug.Log("SALAH");
                GameManager.instance.LoseLife();
            }

            Destroy(gameObject);
        }
    }
}