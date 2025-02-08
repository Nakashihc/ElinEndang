using UnityEngine;

public class FrontObject : MonoBehaviour
{
    public Transform player;
    public float fadeDistance = 3f;
    public float Alpa;
    public Transform objek;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        float distance = Vector2.Distance(objek.position, player.position);

        float alpha = Mathf.Clamp01(distance / fadeDistance);
        Color newColor = originalColor;
        newColor.a = Mathf.Lerp(Alpa / 255f, originalColor.a, alpha);
        spriteRenderer.color = newColor;
    }
}
