using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI floorReachedText;
    [SerializeField] private TextMeshProUGUI enemiesDefeatedText;
    [SerializeField] private TextMeshProUGUI goldEarnedText;
    [SerializeField] private TextMeshProUGUI cardsCollectedText;

    [Header("Buttons")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button tryAgainButton;

    private void Start()
    {
        RunData run = RunManager.Instance?.CurrentRun;
        if (run != null)
        {
            if (floorReachedText    != null) floorReachedText.text    = $"Floor Reached: {run.currentFloor + 1}";
            if (enemiesDefeatedText != null) enemiesDefeatedText.text = $"Enemies Defeated: {run.enemiesDefeated}";
            if (goldEarnedText      != null) goldEarnedText.text      = $"Total Gold Earned: {run.totalGoldEarned}";
            if (cardsCollectedText  != null) cardsCollectedText.text  = $"Cards Collected: {run.deck.Count}";
        }

        mainMenuButton?.onClick.AddListener(() => RunManager.Instance?.OnRunEnded());
        tryAgainButton?.onClick.AddListener(() => RunManager.Instance?.GoToCharacterSelect());
    }
}
