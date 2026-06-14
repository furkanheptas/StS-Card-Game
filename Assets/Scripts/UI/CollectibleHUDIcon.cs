using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CollectibleHUDIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        if (tooltipText != null)
            tooltipText.text = $"{collectible.collectibleName}\n{collectible.description}";

        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData) => tooltipPanel?.SetActive(true);
    public void OnPointerExit(PointerEventData eventData)  => tooltipPanel?.SetActive(false);
}
