using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardManager : MonoBehaviour
{
    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Card Rewards")]
    [SerializeField] private Transform cardRewardContainer;
    [SerializeField] private RewardCardUI rewardCardPrefab;
    [Tooltip("All available cards that can appear as rewards.")]
    [SerializeField] private List<CardData> cardRewardPool = new List<CardData>();
    [Tooltip("How many card choices to show.")]
    [SerializeField] private int cardChoiceCount = 3;

    [Header("Collectible Reward")]
    [SerializeField] private RewardCollectibleUI collectibleSlot;
    [Tooltip("Collectibles that can appear after combat.")]
    [SerializeField] private List<CollectibleData> collectiblePool = new List<CollectibleData>();
    [Tooltip("Chance (0-1) that a collectible appears after a normal fight.")]
    [SerializeField] private float collectibleChance = 0.3f;

    [Header("Skip")]
    [SerializeField] private Button skipButton;

    private int goldEarned;

    private void Start()
    {
        CalculateGold();
        ShowCardChoices();
        MaybeShowCollectible();

        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipClicked);
    }

    private void CalculateGold()
    {
        goldEarned = Random.Range(10, 25);

        MapNodeData node = RunManager.Instance?.PendingNode;
        if (node != null && node.nodeType == MapNodeType.Elite)
            goldEarned = Random.Range(25, 35);
        if (node != null && node.nodeType == MapNodeType.Boss)
            goldEarned = Random.Range(80, 100);

        RunManager.Instance?.CurrentRun?.AddGold(goldEarned);

        if (goldText != null)
            goldText.text = $"+ {goldEarned} Gold";
    }

    private void ShowCardChoices()
    {
        if (cardRewardPool == null || cardRewardPool.Count == 0) return;

        List<CardData> shuffled = new List<CardData>(cardRewardPool);
        Shuffle(shuffled);

        int count = Mathf.Min(cardChoiceCount, shuffled.Count);
        for (int i = 0; i < count; i++)
        {
            RewardCardUI ui = Instantiate(rewardCardPrefab, cardRewardContainer);
            CardData card = shuffled[i];
            ui.Initialize(card, () => PickCard(card));
        }
    }

    private void MaybeShowCollectible()
    {
        if (collectibleSlot == null) return;
        if (collectiblePool == null || collectiblePool.Count == 0) return;

        bool show = Random.value <= collectibleChance;
        collectibleSlot.gameObject.SetActive(show);

        if (!show) return;

        CollectibleData collectible = collectiblePool[Random.Range(0, collectiblePool.Count)];
        collectibleSlot.Initialize(collectible, () => PickCollectible(collectible));
    }

    private void PickCard(CardData card)
    {
        RunManager.Instance?.CurrentRun?.AddCard(card);
        RunManager.Instance?.OnRewardDismissed();
    }

    private void PickCollectible(CollectibleData collectible)
    {
        RunManager.Instance?.CurrentRun?.AddCollectible(collectible);
        RunManager.Instance?.OnRewardDismissed();
    }

    private void OnSkipClicked()
    {
        RunManager.Instance?.OnRewardDismissed();
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
