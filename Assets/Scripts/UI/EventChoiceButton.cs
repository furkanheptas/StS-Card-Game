using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI choiceText;
    [SerializeField] private Button button;

    private Action onChosen;

    public void Initialize(EventChoice choice, Action onChoose)
    {
        onChosen = onChoose;
        if (choiceText != null) choiceText.text = choice.choiceText;
        button.onClick.AddListener(() => onChosen?.Invoke());
    }
}
