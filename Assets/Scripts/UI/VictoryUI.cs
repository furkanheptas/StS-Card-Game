using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI floorsText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Buttons")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button tryAgainButton;

    private void Start()
    {
        RunData run = RunManager.Instance?.CurrentRun;
        if (run != null)
        {
            if (floorsText   != null) floorsText.text   = $"Floors Cleared: {run.currentFloor + 1}";
            if (enemiesText  != null) enemiesText.text  = $"Enemies Defeated: {run.enemiesDefeated}";
            if (goldText     != null) goldText.text     = $"Gold Earned: {run.totalGoldEarned}";
        }

        mainMenuButton?.onClick.AddListener(() => RunManager.Instance?.OnRunEnded());
        tryAgainButton?.onClick.AddListener(() => RunManager.Instance?.GoToCharacterSelect());
    }
}
