using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemArtwork;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject soldOverlay;

    private Action onBuy;

    public void InitializeCard(CardData card, int price, Action onBuyCallback)
    {
        onBuy = onBuyCallback;
        if (itemArtwork != null && card.artwork != null) itemArtwork.sprite = card.artwork;
        if (itemNameText != null) itemNameText.text = card.cardName;
        if (itemDescText != null) itemDescText.text = card.description;
        if (priceText != null)    priceText.text    = $"{price}g";
        soldOverlay?.SetActive(false);
        buyButton?.onClick.AddListener(OnBuyClicked);
    }

    public void InitializeCollectible(CollectibleData col, int price, Action onBuyCallback)
    {
        onBuy = onBuyCallback;
        if (itemArtwork != null && col.artwork != null) itemArtwork.sprite = col.artwork;
        if (itemNameText != null) itemNameText.text = col.collectibleName;
        if (itemDescText != null) itemDescText.text = col.description;
        if (priceText != null)    priceText.text    = $"{price}g";
        soldOverlay?.SetActive(false);
        buyButton?.onClick.AddListener(OnBuyClicked);
    }

    public void MarkSold()
    {
        soldOverlay?.SetActive(true);
        buyButton.interactable = false;
    }

    private void OnBuyClicked() => onBuy?.Invoke();
}
