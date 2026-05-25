using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Characters")]
    [Tooltip("All available playable characters.")]
    [SerializeField] private List<CharacterData> availableCharacters = new List<CharacterData>();

    [Header("Card Slot Prefab")]
    [SerializeField] private CharacterCardUI characterCardPrefab;
    [SerializeField] private Transform cardContainer;

    [Header("Detail Panel")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Confirm")]
    [SerializeField] private Button startRunButton;

    private CharacterData selectedCharacter;

    private void Start()
    {
        startRunButton.onClick.AddListener(OnStartRunClicked);
        startRunButton.interactable = false;

        foreach (CharacterData character in availableCharacters)
        {
            CharacterCardUI card = Instantiate(characterCardPrefab, cardContainer);
            card.Initialize(character, () => SelectCharacter(character));
        }
    }

    private void SelectCharacter(CharacterData character)
    {
        selectedCharacter = character;
        startRunButton.interactable = true;

        if (portraitImage != null) portraitImage.sprite = character.portrait;
        if (nameText != null) nameText.text = character.characterName;
        if (descriptionText != null) descriptionText.text = character.characterDescription;
        if (hpText != null) hpText.text = $"HP: {character.startingMaxHP}";
        if (goldText != null) goldText.text = $"Gold: {character.startingGold}";
    }

    private void OnStartRunClicked()
    {
        if (selectedCharacter == null) return;
        RunManager.Instance.StartRun(selectedCharacter);
    }
}
