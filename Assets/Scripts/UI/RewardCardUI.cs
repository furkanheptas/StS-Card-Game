using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardCardUI : MonoBehaviour
{
    [SerializeField] private Image artwork;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI manaCostText;
    [SerializeField] private Button pickButton;

    private Action onPicked;

    public void Initialize(CardData data, Action onPick)
    {
        onPicked = onPick;

        if (artwork != null) artwork.sprite = data.artwork;
        if (cardNameText != null) cardNameText.text = data.cardName;
        if (descriptionText != null) descriptionText.text = data.description;
        if (manaCostText != null) manaCostText.text = data.manaCost.ToString();

        if (pickButton != null) pickButton.onClick.AddListener(() => onPicked?.Invoke());
    }
}
