using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectibleHUDIcon : MonoBehaviour
{
    [SerializeField] private Image artworkImage;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private GameObject tooltipPanel;

    private CollectibleData data;

    public void Initialize(CollectibleData collectible)
    {
        data = collectible;
        if (artworkImage != null && collectible.artwork != null)
            artworkImage.sprite = collectible.artwork;

        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (tooltipText  != null) tooltipText.text = $"{collectible.collectibleName}\n{collectible.description}";
    }

    public void OnPointerEnter() => tooltipPanel?.SetActive(true);
    public void OnPointerExit()  => tooltipPanel?.SetActive(false);
}
