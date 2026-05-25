using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunHUD : MonoBehaviour
{
    public static RunHUD Instance { get; private set; }

    [Header("Vitals")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Slider hpSlider;

    [Header("Gold & Floor")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI floorText;

    [Header("Collectibles")]
    [SerializeField] private Transform collectibleContainer;
    [SerializeField] private CollectibleHUDIcon collectibleIconPrefab;

    private readonly List<CollectibleHUDIcon> spawnedIcons = new List<CollectibleHUDIcon>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        transform.SetParent(null, false);

        if (GetComponent<Canvas>() == null)
        {
            Canvas c = gameObject.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.sortingOrder = 100;

            if (GetComponent<UnityEngine.UI.CanvasScaler>() == null)
            {
                var scaler = gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
                gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Refresh();
    }
    public void Refresh()
    {
        RunData run = RunManager.Instance?.CurrentRun;
        if (run == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (hpText != null)   hpText.text   = $"HP: {run.currentHP} / {run.maxHP}";
        if (hpSlider != null)
        {
            hpSlider.maxValue = run.maxHP;
            hpSlider.value    = run.currentHP;
        }
        if (goldText != null)  goldText.text  = $"Gold: {run.gold}";
        if (floorText != null) floorText.text = $"Floor: {run.currentFloor + 1}";

        RefreshCollectibles(run);
    }

    private void RefreshCollectibles(RunData run)
    {
        foreach (CollectibleHUDIcon icon in spawnedIcons) Destroy(icon.gameObject);
        spawnedIcons.Clear();

        if (collectibleContainer == null || collectibleIconPrefab == null) return;

        foreach (CollectibleData col in run.collectibles)
        {
            CollectibleHUDIcon icon = Instantiate(collectibleIconPrefab, collectibleContainer);
            icon.Initialize(col);
            spawnedIcons.Add(icon);
        }
    }
}
