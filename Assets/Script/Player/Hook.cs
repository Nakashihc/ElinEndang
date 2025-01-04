using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header ("Garisnya")]
    public Transform Karakter;
    public GameObject Hooknya;
    public Transform ShootPoint;
    public LineRenderer line;

    [Header ("Kekuatan + Kecepatan")]
    public float KecepatanHook;
    public SpringJoint2D spring;
    public float pullSpeed = 2f;

    private GameObject hookInstance; // Menyimpan instance dari hook yang ditembakkan
    private GameObject target;       // Target yang terkena hook
    public bool isHookActive = false;
    private bool isHookAttached = false;

    public Movement playerMov;
    private Tali tali;

    void Start()
    {
        line.enabled = false;
        spring.enabled = false;
    }

    void Update()
    {
        if(hookInstance != null)
        {
            tali = GameObject.FindGameObjectWithTag("Tali").GetComponent<Tali>();
        }

        if (!isHookActive)
        {
            playerMov.canmove = true;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!isHookActive)
            {
                playerMov.canmove = false;
                Shoot();
            }
            else
            {
                ResetHook();
            }
        }

        if (Input.GetKey(KeyCode.P) && isHookAttached)
        {
            PullTarget();
        }

        if (isHookActive)
        {
            if (isHookAttached && target != null)
            {
                line.SetPosition(0, Karakter.position);
                line.SetPosition(1, target.transform.position);
            }
            else if (hookInstance != null)
            {
                line.enabled = true;
                line.SetPosition(0, Karakter.position);
                line.SetPosition(1, hookInstance.transform.position);
            }
        }
    }

    void Shoot()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector2 direction = (mousePosition - ShootPoint.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        ShootPoint.rotation = Quaternion.Euler(0, 0, angle);

        hookInstance = Instantiate(Hooknya, ShootPoint.position, ShootPoint.rotation);
        hookInstance.GetComponent<Rigidbody2D>().AddForce(direction * KecepatanHook, ForceMode2D.Impulse);
        isHookActive = true;
    }


    public void TargetHit(GameObject hit)
    {
        target = hit;
        line.enabled = true;
        isHookAttached = true;

        Debug.Log("Target Hit: " + hit.name);
    }

    void PullTarget()
    {
        if (target != null)
        {
            Vector2 targetPosition = Vector2.MoveTowards(target.transform.position, (Vector2)transform.position, pullSpeed * Time.deltaTime);
            target.transform.position = targetPosition;
        }
    }

    public void ResetHook()
    {
        line.enabled = false;
        spring.enabled = false;
        target = null;
        isHookActive = false;
        isHookAttached = false;
        tali.Finish();
    }
}
