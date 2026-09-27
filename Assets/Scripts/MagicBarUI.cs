using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives a UI Slider to display the player's current magic charge.
/// Subscribes to PlayerMagic.OnMagicChanged, same pattern as a health bar
/// would subscribe to PlayerHealth.OnHealthChanged.
/// </summary>
public class MagicBarUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Slider component that visually represents the magic bar.")]
    [SerializeField] private Slider magicSlider;

    [Header("Optional Smoothing")]
    [SerializeField] private bool smoothFill = true;
    [SerializeField] private float fillSpeed = 6f;

    private float targetFill = 0f;

    private void OnEnable()
    {
        PlayerMagic.OnMagicChanged += HandleMagicChanged;
    }

    private void OnDisable()
    {
        PlayerMagic.OnMagicChanged -= HandleMagicChanged;
    }

    private void HandleMagicChanged(float current, int max)
    {
        targetFill = max > 0 ? current / max : 0f;

        if (!smoothFill)
        {
            magicSlider.value = targetFill;
        }
    }

    private void Update()
    {
        if (smoothFill && magicSlider.value != targetFill)
        {
            magicSlider.value = Mathf.Lerp(magicSlider.value, targetFill, Time.deltaTime * fillSpeed);
        }
    }
}
