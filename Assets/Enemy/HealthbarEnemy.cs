using UnityEngine;
using UnityEngine.UI;

public class HealthbarEnemy : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform target;
    public Color Low;
    public Color High;
    [SerializeField] private Vector3 offset;

    public void SetHealth(int health, int maxHealthh)
    {
        slider.gameObject.SetActive(health < maxHealthh);
        slider.value = health;
        slider.maxValue = maxHealthh;

        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Low, High, slider.normalizedValue);
    }

    void Update()
    {
        transform.position = target.position + offset;
    }

}
