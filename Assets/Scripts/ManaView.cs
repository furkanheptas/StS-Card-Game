using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaView : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private Image manaFillImage;

    private void Start()
    {
        if (ManaSystem.Instance != null)
        {
            
            ManaSystem.Instance.OnManaChanged += UpdateManaUI;
            
            UpdateManaUI(ManaSystem.Instance.CurrentMana, ManaSystem.Instance.MaxMana);
        }
    }

    private void OnDestroy()
    {
        if (ManaSystem.Instance != null)
            ManaSystem.Instance.OnManaChanged -= UpdateManaUI;
    }

    private void UpdateManaUI(int current, int max)
    {
        if (manaText != null)
            manaText.text = $"{current}/{max}";

        if (manaFillImage != null)
            manaFillImage.fillAmount = (float)current / max;
    }
}