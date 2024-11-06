using UnityEngine;

public class Tali : MonoBehaviour
{
    private Hook hook;

    void Start()
    {
        // Mendapatkan referensi ke script Hook di Player
        hook = GameObject.FindGameObjectWithTag("Player").GetComponent<Hook>();
        Invoke("Finish", 2); // Menghancurkan objek ini setelah 2 detik
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("box"))
        {
            hook.TargetHit(col.gameObject);

            Destroy(gameObject);
        }
    }

    public void Finish()
    {
        hook.line.enabled = false;
        hook.isHookActive = false;
    }
}
