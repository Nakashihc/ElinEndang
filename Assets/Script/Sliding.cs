using UnityEngine;

public class Sliding : MonoBehaviour
{
    public Jongkok jong;
    [Header("Sliding")]
    public float slideSpeed = 10f;
    public float slideDuration = 0.5f;
    private bool isSliding = false;
    private float slideTimer = 0f;
    private Vector3 slideStartPos;
    private Vector3 slideEndPos;
    [SerializeField] private LayerMask wallSlideLayer;

    [Header("Energy")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyCost = 20f;
    private float currentEnergy;
    [SerializeField] private float energyRegenRate = 5f;

    private Rigidbody2D rb;
    private Movement movement;

    public bool canSlide = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
        currentEnergy = maxEnergy;
    }

    private void Update()
    {
        ProcessSlide();

        if (currentEnergy < maxEnergy && !isSliding)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
    }

    public void StartSlide()
    {
        if (currentEnergy >= energyCost)
        {
            isSliding = true;
            slideTimer = 0f;
            movement.anim.SetBool("isCrouching", false);
            jong.UpdateColliderState(false);
            movement.enabled = false;
            currentEnergy -= energyCost;

            slideStartPos = transform.position;
            slideEndPos = slideStartPos + transform.right * movement.GetHorizontal() * slideSpeed;

            movement.DisableCollider(); // Disable movement collider if necessary
        }
        else
        {
            // Pass
        }
    }

    private void ProcessSlide()
    {
        if (isSliding)
        {
            slideTimer += Time.deltaTime;
            movement.anim.SetBool("isSliding", true);
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                RaycastHit2D hit = Physics2D.BoxCast(
                    boxCollider.bounds.center,
                    boxCollider.bounds.size,
                    0f,
                    Vector2.right * Mathf.Sign(slideEndPos.x - slideStartPos.x),
                    0.2f,
                    wallSlideLayer
                );

                if (hit.collider != null)
                {
                    isSliding = false;
                    return;
                }
            }

            transform.position = new Vector3(
                Mathf.Lerp(slideStartPos.x, slideEndPos.x, slideTimer / slideDuration),
                transform.position.y,
                transform.position.z
            );

            if (slideTimer >= slideDuration)
            {
                isSliding = false;
            }
        }
        else
        {
            movement.anim.SetBool("isSliding", false);
            jong.UpdateColliderState(true);
            movement.enabled = true;
        }
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NoSlideZone")) // Replace with tag or layer name
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                canSlide = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NoSlideZone")) // Replace with tag or layer name
        {
            canSlide = false;
        }
    }
}
