using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ShopData shopData;

    [Header("Gold Display")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Card Slots")]
    [SerializeField] private Transform cardContainer;
    [SerializeField] private ShopItemSlot itemSlotPrefab;

    [Header("Collectible Slots")]
    [SerializeField] private Transform collectibleContainer;

    [Header("Services")]
    [SerializeField] private Button removeCardButton;
    [SerializeField] private Button upgradeCardButton;
    [SerializeField] private TextMeshProUGUI removeCostText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;

    [Header("Card Picker Panel (shared)")]
    [SerializeField] private GameObject cardPickerPanel;
    [SerializeField] private Transform  cardPickerContainer;
    [SerializeField] private RestCardSlot cardPickerSlotPrefab;
    [SerializeField] private Button     cardPickerCancelButton;

    [Header("Leave")]
    [SerializeField] private Button leaveButton;

    private enum PickMode { None, Remove, Upgrade }
    private PickMode pickMode = PickMode.None;

    private readonly List<ShopItemSlot> spawnedSlots = new List<ShopItemSlot>();

    private void Start()
    {
        RefreshGold();
        PopulateCards();
        PopulateCollectibles();
        SetupServices();

        leaveButton?.onClick.AddListener(OnLeaveClicked);
        cardPickerCancelButton?.onClick.AddListener(CloseCardPicker);
        cardPickerPanel?.SetActive(false);
    }

    private void PopulateCards()
    {
        if (shopData == null) return;

        List<CardData> pool = new List<CardData>(shopData.cardPool);
        Shuffle(pool);

        int count = Mathf.Min(shopData.cardSlotCount, pool.Count);
        for (int i = 0; i < count; i++)
        {
            CardData card = pool[i];
            int price = Random.Range(shopData.cardPriceMin, shopData.cardPriceMax + 1);
            ShopItemSlot slot = Instantiate(itemSlotPrefab, cardContainer);
            slot.InitializeCard(card, price, () => BuyCard(card, price, slot));
            spawnedSlots.Add(slot);
        }
    }

    private void PopulateCollectibles()
    {
        if (shopData == null) return;

        List<CollectibleData> pool = new List<CollectibleData>(shopData.collectiblePool);
        Shuffle(pool);

        int count = Mathf.Min(shopData.collectibleSlotCount, pool.Count);
        for (int i = 0; i < count; i++)
        {
            CollectibleData col = pool[i];
            int price = Random.Range(shopData.collectiblePriceMin, shopData.collectiblePriceMax + 1);
            ShopItemSlot slot = Instantiate(itemSlotPrefab, collectibleContainer);
            slot.InitializeCollectible(col, price, () => BuyCollectible(col, price, slot));
            spawnedSlots.Add(slot);
        }
    }

    private void SetupServices()
    {
        if (shopData == null) return;

        if (removeCostText != null) removeCostText.text  = $"{shopData.removalCost}g";
        if (upgradeCostText != null) upgradeCostText.text = $"{shopData.upgradeCost}g";

        removeCardButton?.onClick.AddListener(OnRemoveCardClicked);
        upgradeCardButton?.onClick.AddListener(OnUpgradeCardClicked);
    }

    private void BuyCard(CardData card, int price, ShopItemSlot slot)
    {
        RunData run = RunManager.Instance?.CurrentRun;
        if (run == null || run.gold < price) return;

        run.SpendGold(price);
        run.AddCard(card);
        slot.MarkSold();
        RefreshGold();
    }

    private void BuyCollectible(CollectibleData col, int price, ShopItemSlot slot)
    {
        RunData run = RunManager.Instance?.CurrentRun;
        if (run == null || run.gold < price) return;

        run.SpendGold(price);
        run.AddCollectible(col);
        slot.MarkSold();
        RefreshGold();
    }

    private void OnRemoveCardClicked()
    {
        RunData run = RunManager.Instance?.CurrentRun;
        if (run == null || run.gold < shopData.removalCost) return;
        if (run.deck.Count == 0) return;

        pickMode = PickMode.Remove;
        OpenCardPicker(run, showUpgradable: false);
    }

    private void OnUpgradeCardClicked()
    {
        RunData run = RunManager.Instance?.CurrentRun;
        if (run == null || run.gold < shopData.upgradeCost) return;
        if (!run.deck.Exists(c => c.CanUpgrade)) return;

        pickMode = PickMode.Upgrade;
        OpenCardPicker(run, showUpgradable: true);
    }

    private void OpenCardPicker(RunData run, bool showUpgradable)
    {
        foreach (Transform child in cardPickerContainer) Destroy(child.gameObject);

        foreach (CardData card in run.deck)
        {
            if (showUpgradable && !card.CanUpgrade) continue;

            RestCardSlot slot = Instantiate(cardPickerSlotPrefab, cardPickerContainer);
            CardData captured = card;
            slot.Initialize(card, () => OnCardPicked(captured));
        }

        cardPickerPanel?.SetActive(true);
    }

    private void OnCardPicked(CardData card)
    {
        RunData run = RunManager.Instance?.CurrentRun;
        if (run == null) return;

        if (pickMode == PickMode.Remove)
        {
            run.SpendGold(shopData.removalCost);
            run.RemoveCard(card);
        }
        else if (pickMode == PickMode.Upgrade)
        {
            run.SpendGold(shopData.upgradeCost);
            run.UpgradeCard(card);
        }

        CloseCardPicker();
        RefreshGold();
    }

    private void CloseCardPicker()
    {
        pickMode = PickMode.None;
        cardPickerPanel?.SetActive(false);
    }

    private void RefreshGold()
    {
        int gold = RunManager.Instance?.CurrentRun?.gold ?? 0;
        if (goldText != null) goldText.text = $"Gold: {gold}";
    }

    private void OnLeaveClicked() => RunManager.Instance?.OnNodeCompleted();

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
