using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public Slider slide;

    public void TurnOnOffObject(bool aValue)
    {
        slide.gameObject.SetActive(aValue);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StaminaDrain()
    {
        slide.value -= 0.1f;
    }
    public void StaminaGain()
    {
        slide.value += 0.4f;
        if (slide.value == slide.maxValue)
        {
            TurnOnOffObject(false);
        }
    }
}
