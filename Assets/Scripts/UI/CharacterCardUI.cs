using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCardUI : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject selectedHighlight;

    private Action onSelected;

    public void Initialize(CharacterData data, Action onSelect)
    {
        onSelected = onSelect;

        if (portraitImage != null) portraitImage.sprite = data.portrait;
        if (nameText != null) nameText.text = data.characterName;
        if (selectedHighlight != null) selectedHighlight.SetActive(false);

        selectButton.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        if (selectedHighlight != null) selectedHighlight.SetActive(true);
        onSelected?.Invoke();
    }
}
