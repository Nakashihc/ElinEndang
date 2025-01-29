using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SlidingController : MonoBehaviour
{
    [Header("Sliding Settings")]
    public float slideSpeed = 10f;
    public float slideDuration = 0.5f;

    [Header("Energy Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyCost = 20f;
    [SerializeField] private float energyRegenRate = 5f;

    [Header("References")]
    [SerializeField] private LayerMask wallSlideLayer;
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerMovements playerMovement;

    public float currentEnergy;
    public bool isSliding = false;
    private float slideTimer;
    private Vector3 slideStartPos;
    private Vector3 slideEndPos;

    public bool CanSlide { get; private set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovements>();
        currentEnergy = maxEnergy;
    }

    private void Update()
    {
        RegenerateEnergy();
        if (isSliding)
        {
            ProcessSlide();
        }
    }

    private void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy && !isSliding)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
    }

    public void StartSlide()
    {
        if (!CanSlide || isSliding || currentEnergy < energyCost)
        {
            return;
        }

        isSliding = true;
        slideTimer = 0f;
        currentEnergy -= energyCost;
        slideStartPos = transform.position;
        slideEndPos = slideStartPos + transform.right * playerMovement.GetHorizontal() * slideSpeed;

        anim.SetBool("isSliding", true);
        playerMovement.enabled = false;
    }

    private void ProcessSlide()
    {
        slideTimer += Time.deltaTime;

        // Check for wall collision
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
                EndSlide();
                return;
            }
        }

        // Move player during slide
        transform.position = new Vector3(
            Mathf.Lerp(slideStartPos.x, slideEndPos.x, slideTimer / slideDuration),
            transform.position.y,
            transform.position.z
        );

        if (slideTimer >= slideDuration)
        {
            EndSlide();
        }
    }

    private void EndSlide()
    {
        isSliding = false;
        anim.SetBool("isSliding", false);
        playerMovement.enabled = true;
    }

    public float GetCurrentEnergy() => currentEnergy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NoSlideZone"))
        {
            CanSlide = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NoSlideZone"))
        {
            CanSlide = true;
        }
    }
}
