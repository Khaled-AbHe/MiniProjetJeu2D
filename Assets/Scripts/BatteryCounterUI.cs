using TMPro;
using UnityEngine;

/// <summary>
/// Displays the battery collection progress (e.g. "2 / 3") using a
/// TextMeshProUGUI label. Subscribes to Collecteur.OnBatteryCountChanged.
/// </summary>
public class BatteryCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    [Tooltip("{0} is replaced with the collected count, {1} with the objective.")]
    [SerializeField] private string format = "{0} / {1}";

    private void OnEnable()
    {
        Collecteur.OnBatteryCountChanged += UpdateCounter;
    }

    private void OnDisable()
    {
        Collecteur.OnBatteryCountChanged -= UpdateCounter;
    }

    private void UpdateCounter(int collected, int objective)
    {
        if (counterText != null)
        {
            counterText.text = string.Format(format, collected, objective);
        }
    }
}
