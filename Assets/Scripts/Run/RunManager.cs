using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }
    public const string MainMenuScene        = "mainmenu";
    public const string CharacterSelectScene = "CharacterSelect";
    public const string MapScene             = "Map";
    public const string BattleScene          = "Battle";
    public const string ShopScene            = "Shop";
    public const string EventScene           = "Event";
    public const string RewardScene          = "Reward";
    public const string RestScene            = "Rest";
    public const string GameOverScene        = "GameOver";
    public const string VictoryScene         = "Victory";
    public RunData CurrentRun { get; private set; }
    public MapNodeData PendingNode { get; private set; }
    public Vector3? SavedMapCameraPosition { get; set; }
    public float? SavedMapCameraSize { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartRun(CharacterData character)
    {
        CurrentRun = new RunData();
        CurrentRun.Initialize(character);
        GoToMap();
    }

    public void AbandonRun()
    {
        CurrentRun = null;
        LoadScene(MainMenuScene);
    }


    public void GoToMap()             => LoadScene(MapScene);
    public void GoToMainMenu()        => LoadScene(MainMenuScene);
    public void GoToCharacterSelect() => LoadScene(CharacterSelectScene);

    public void EnterNode(MapNodeData node)
    {
        if (CurrentRun == null)
        {
            Debug.LogWarning("[RunManager] EnterNode called but CurrentRun is null. " +
                             "Did you start the game from the Map scene directly? " +
                             "Start from mainmenu or CharacterSelect instead.");
            return;
        }

        PendingNode = node;
        CurrentRun.currentNodeIndex = node.nodeIndex;
        CurrentRun.currentFloor = node.floor;

        Debug.Log($"[RunManager] Entering node #{node.nodeIndex} | Type: {node.nodeType} | Floor: {node.floor}");

        switch (node.nodeType)
        {
            case MapNodeType.Normal:
            case MapNodeType.Elite:
            case MapNodeType.Boss:
                LoadScene(BattleScene);
                break;
            case MapNodeType.Shop:
                LoadScene(ShopScene);
                break;
            case MapNodeType.Event:
                LoadScene(EventScene);
                break;
            case MapNodeType.Rest:
                LoadScene(RestScene);
                break;
        }
    }

    public void OnCombatVictory()
    {
        if (CurrentRun != null)
            CurrentRun.enemiesDefeated++;

        if (PendingNode?.nodeType == MapNodeType.Boss)
        {
            LoadScene(VictoryScene);
            return;
        }

        LoadScene(RewardScene);
    }

    public void OnPlayerDeath()
    {
        LoadScene(GameOverScene);
    }

    public void OnRunEnded()
    {
        CurrentRun = null;
        LoadScene(MainMenuScene);
    }
    public void OnNodeCompleted() => GoToMap();

    public void OnRewardDismissed() => OnNodeCompleted();

    private void LoadScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
            SceneManager.LoadScene(sceneName);
        else
            Debug.LogError($"[RunManager] Scene '{sceneName}' not found in Build Settings.");
    }
}
