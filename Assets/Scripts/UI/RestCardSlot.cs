using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestCardSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private Image cardArtwork;
    [SerializeField] private Button selectButton;

    private Action onSelected;

    public void Initialize(CardData card, Action onSelect)
    {
        onSelected = onSelect;

        if (cardNameText != null)   cardNameText.text   = card.cardName;
        if (upgradeNameText != null) upgradeNameText.text = card.CanUpgrade
            ? $"→ {card.upgradedVersion.cardName}"
            : "(max level)";
        if (cardArtwork != null && card.artwork != null)
            cardArtwork.sprite = card.artwork;

        selectButton.onClick.AddListener(() => onSelected?.Invoke());
    }
}
