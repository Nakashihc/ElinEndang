using UnityEngine;

public class Tali : MonoBehaviour
{
    private Hook hook;
    private float waktuIlang;

    void Start()
    {
        hook = GameObject.FindGameObjectWithTag("Player").GetComponent<Hook>();
        Invoke("Finish", 2);
    }

    private void Update()
    {
        waktuIlang += Time.deltaTime;

        if(waktuIlang > 2f)
        {
            Destroy(gameObject);
        }
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
