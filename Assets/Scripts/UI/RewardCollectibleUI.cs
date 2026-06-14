using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardCollectibleUI : MonoBehaviour
{
    [SerializeField] private Image artwork;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button pickButton;

    private Action onPicked;

    public void Initialize(CollectibleData data, Action onPick)
    {
        onPicked = onPick;

        if (artwork != null) artwork.sprite = data.artwork;
        if (nameText != null) nameText.text = data.collectibleName;
        if (descriptionText != null) descriptionText.text = data.description;

        if (pickButton != null) pickButton.onClick.AddListener(() => onPicked?.Invoke());
    }
}
