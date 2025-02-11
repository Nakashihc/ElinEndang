using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [Header("References")]
    public Slider staminaSlider; // Referensi ke UI Slider

    [Header("Settings")]
    [SerializeField] private float drainAmount = 0.1f; // Jumlah stamina yang dikurangi
    [SerializeField] private float gainAmount = 0.4f; // Jumlah stamina yang ditambahkan

    /// <summary>
    /// Mengaktifkan atau menonaktifkan objek slider stamina.
    /// </summary>
    /// <param name="isActive">True untuk mengaktifkan, false untuk menonaktifkan.</param>
    public void ToggleStaminaUI(bool isActive)
    {
        staminaSlider.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// Mengurangi nilai stamina.
    /// </summary>
    public void DrainStamina()
    {
        staminaSlider.value -= drainAmount;
    }

    /// <summary>
    /// Menambahkan nilai stamina.
    /// </summary>
    public void GainStamina()
    {
        staminaSlider.value += gainAmount;

        // Nonaktifkan slider jika stamina penuh
        if (staminaSlider.value >= staminaSlider.maxValue)
        {
            ToggleStaminaUI(false);
        }
    }
}
