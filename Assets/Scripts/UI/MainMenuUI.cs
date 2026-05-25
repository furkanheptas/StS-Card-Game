using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button newRunButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        if (RunManager.Instance == null)
        {
            GameObject go = new GameObject("RunManager");
            go.AddComponent<RunManager>();
        }

        newRunButton?.onClick.AddListener(OnNewRunClicked);
        quitButton?.onClick.AddListener(OnQuitClicked);
    }

    private void OnNewRunClicked()
    {
        RunManager.Instance.GoToCharacterSelect();
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
