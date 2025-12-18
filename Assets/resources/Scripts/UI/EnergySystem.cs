using UnityEngine;
using UnityEngine.UI;

public class EnergySystem : MonoBehaviour
{
    [Header("Energy Settings")]
    public float maxEnergy = 100f;
    public float regenRate = 10f; // energy per second
    public float currentEnergy = 50f;

    [Header("UI")]
    public Slider energySlider;
    public TMPro.TextMeshProUGUI energyText;

    private void Start()
    {
        currentEnergy = 0f;
        UpdateUI();
        InvokeRepeating(nameof(Regenerate), 0f, 1.8f); // CR-style
    }

    void Regenerate()
    {
        currentEnergy = Mathf.Clamp(currentEnergy + regenRate, 0f, maxEnergy);
        UpdateUI();
    }

    public bool TrySpend(float amount)
    {
        if (currentEnergy < amount) return false;

        currentEnergy -= amount;
        UpdateUI();
        return true;
    }

    void UpdateUI()
    {
        if (energySlider) energySlider.value = currentEnergy / maxEnergy;
        if (energyText) energyText.text = Mathf.RoundToInt(currentEnergy).ToString();
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              