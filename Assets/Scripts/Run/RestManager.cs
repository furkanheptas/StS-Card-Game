using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestManager : MonoBehaviour
{
    private enum RestState { ChooseAction, UpgradePicking, Done }

    [Header("Choice Buttons")]
    [SerializeField] private Button healButton;
    [SerializeField] private Button upgradeButton;

    [Header("Heal")]
    [Tooltip("Fraction of max HP restored when resting (0-1).")]
    [SerializeField] private float healFraction = 0.30f;
    [SerializeField] private TextMeshProUGUI healAmountText;

    [Header("Upgrade Panel")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform upgradeCardContainer;
    [SerializeField] private RestCardSlot upgradeCardSlotPrefab;
    [SerializeField] private Button upgradeBackButton;

    [Header("Result")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button continueButton;

    private RestState state = RestState.ChooseAction;

    private void Start()
    {
        upgradePanel?.SetActive(false);
        continueButton?.gameObject.SetActive(false);

        RunData run = RunManager.Instance?.CurrentRun;
        if (run != null)
        {
            int healAmount = Mathf.Max(1, Mathf.FloorToInt(run.maxHP * healFraction));
            if (healAmountText != null)
                healAmountText.text = $"Heal {healAmount} HP";

            bool hasUpgradable = run.deck.Exists(c => c.CanUpgrade);
            if (upgradeButton != null)
                upgradeButton.interactable = hasUpgradable;
        }

        healButton?.onClick.AddListener(OnHealClicked);
        upgradeButton?.onClick.AddListener(OnUpgradeClicked);
        upgradeBackButton?.onClick.AddListener(OnUpgradeBackClicked);
        continueButton?.onClick.AddListener(OnContinueClicked);
    }

    private void OnHealClicked()
    {
        if (state != RestState.ChooseAction) return;

        RunData run = RunManager.Instance?.CurrentRun;
        if (run == null) return;

        int healAmount = Mathf.Max(1, Mathf.FloorToInt(run.maxHP * healFraction));
        run.HealHP(healAmount);

        ShowResult($"Rested. Restored {healAmount} HP.\nCurrent HP: {run.currentHP}/{run.maxHP}");
    }

    private void OnUpgradeClicked()
    {
        if (state != RestState.ChooseAction) return;

        upgradePanel?.SetActive(true);
        PopulateUpgradeList();
    }

    private void OnUpgradeBackClicked()
    {
        upgradePanel?.SetActive(false);
        ClearUpgradeList();
    }

    private void PopulateUpgradeList()
    {
        ClearUpgradeList();

        RunData run = RunManager.Instance?.CurrentRun;
        if (run == null) return;

        foreach (CardData card in run.deck)
        {
            if (!card.CanUpgrade) continue;

            RestCardSlot slot = Instantiate(upgradeCardSlotPrefab, upgradeCardContainer);
            CardData captured = card;
            slot.Initialize(card, () => UpgradeCard(captured));
        }
    }

    private void ClearUpgradeList()
    {
        foreach (Transform child in upgradeCardContainer)
            Destroy(child.gameObject);
    }

    private void UpgradeCard(CardData card)
    {
        RunData run = RunManager.Instance?.CurrentRun;
        if (run == null) return;

        bool success = run.UpgradeCard(card);
        upgradePanel?.SetActive(false);
        ClearUpgradeList();

        string msg = success
            ? $"Upgraded {card.cardName} → {card.upgradedVersion.cardName}!"
            : "Could not upgrade that card.";
        ShowResult(msg);
    }

    private void ShowResult(string message)
    {
        state = RestState.Done;

        healButton?.gameObject.SetActive(false);
        upgradeButton?.gameObject.SetActive(false);

        if (resultText != null) resultText.text = message;
        continueButton?.gameObject.SetActive(true);
    }

    private void OnContinueClicked()
    {
        RunManager.Instance?.OnNodeCompleted();
    }
}
