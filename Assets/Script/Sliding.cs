using UnityEngine;

public class Sliding : MonoBehaviour
{
    public float slideSpeed = 10f;
    public float slideDuration = 0.5f;
    private bool isSliding = false;
    private float slideTimer = 0f;
    private Vector3 slideStartPos;
    private Vector3 slideEndPos;

    [SerializeField] private LayerMask wallSlideLayer;
    [SerializeField] private BoxCollider2D atas;
    private Rigidbody2D rb;

    // Energy variables
    [SerializeField] private float maxEnergy = 100f; // Maximum energy
    [SerializeField] private float energyCost = 20f; // Energy cost per slide
    private float currentEnergy;
    [SerializeField] private float energyRegenRate = 5f; // Rate of energy regeneration per second

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentEnergy = maxEnergy; // Set energy to max at the start
    }

    private void Update()
    {
        // Check for slide input and sufficient energy
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C) && !isSliding && rb.velocity.x != 0 && currentEnergy >= energyCost)
        {
            StartSlide();
        }

        ProcessSlide();

        // Regenerate energy over time if it's below maxEnergy
        if (currentEnergy < maxEnergy && !isSliding)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy); // Ensure energy doesn't exceed max
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = 0f;
        atas.enabled = false;

        // Deduct energy for sliding
        currentEnergy -= energyCost;

        slideStartPos = transform.position;
        slideEndPos = slideStartPos + transform.right * Mathf.Sign(rb.velocity.x) * slideSpeed;
    }

    private void ProcessSlide()
    {
        if (isSliding)
        {
            slideTimer += Time.deltaTime;

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
                atas.enabled = true;
            }
        }
    }

    // Optionally, create a method to display the current energy (e.g., in a UI element)
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }
}
