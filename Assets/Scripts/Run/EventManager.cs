using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EventManager : MonoBehaviour
{
    [Header("Fallback (for testing without a run)")]
    [SerializeField] private EventData fallbackEvent;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI eventNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image artworkImage;
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private EventChoiceButton choiceButtonPrefab;

    [Header("Result Panel")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button continueButton;

    private EventData currentEvent;

    private void Start()
    {
        resultPanel?.SetActive(false);
        continueButton?.onClick.AddListener(OnContinueClicked);
        currentEvent = RunManager.Instance?.PendingNode?.eventData ?? fallbackEvent;

        if (currentEvent == null)
        {
            Debug.LogWarning("[EventManager] No EventData found. Returning to map.");
            Invoke(nameof(ReturnToMap), 1.5f);
            return;
        }

        PopulateUI();
    }

    private void PopulateUI()
    {
        if (eventNameText  != null) eventNameText.text  = currentEvent.eventName;
        if (descriptionText != null) descriptionText.text = currentEvent.description;
        if (artworkImage   != null && currentEvent.artwork != null)
            artworkImage.sprite = currentEvent.artwork;

        foreach (EventChoice choice in currentEvent.choices)
        {
            EventChoiceButton btn = Instantiate(choiceButtonPrefab, choiceContainer);
            EventChoice captured = choice;
            btn.Initialize(choice, () => ResolveChoice(captured));
        }
    }

    private void ResolveChoice(EventChoice choice)
    {
        RunData run = RunManager.Instance?.CurrentRun;
        if (run == null) return;

        switch (choice.effectType)
        {
            case EventEffectType.GainGold:        run.AddGold(choice.value);                        break;
            case EventEffectType.LoseGold:        run.SpendGold(choice.value);                      break;
            case EventEffectType.GainHP:          run.HealHP(choice.value);                         break;
            case EventEffectType.LoseHP:          run.TakeDamage(choice.value);                     break;
            case EventEffectType.GainCard:        if (choice.cardReward != null)
                                                      run.AddCard(choice.cardReward);               break;
            case EventEffectType.GainCollectible: if (choice.collectibleReward != null)
                                                      run.AddCollectible(choice.collectibleReward); break;
            case EventEffectType.RemoveCard:      break;
        }

        foreach (Transform child in choiceContainer) Destroy(child.gameObject);
        resultPanel?.SetActive(true);
        if (resultText != null) resultText.text = choice.resultDescription;
    }

    private void OnContinueClicked() => ReturnToMap();

    private void ReturnToMap() => RunManager.Instance?.OnNodeCompleted();
}
