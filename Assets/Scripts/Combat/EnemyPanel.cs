using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPanel : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Shield")]
    [SerializeField] private GameObject shieldContainer;
    [SerializeField] private TextMeshProUGUI shieldText;

    [Header("Intent")]
    [SerializeField] private Image intentIcon;
    [SerializeField] private TextMeshProUGUI intentValueText;
    [SerializeField] private TextMeshProUGUI intentLabel;

    [Header("Tracking")]
    [Tooltip("Vertical offset above the enemy sprite in world units.")]
    [SerializeField] private float worldVerticalOffset = 1.2f;

    private Enemy trackedEnemy;
    private Camera mainCamera;
    private RectTransform rectTransform;
    private Canvas rootCanvas;

    public void Track(Enemy enemy)
    {
        trackedEnemy = enemy;
        mainCamera   = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        rootCanvas   = GetComponentInParent<Canvas>();

        enemy.OnHPChanged     += RefreshHP;
        enemy.OnShieldChanged += RefreshShield;
        enemy.OnIntentChanged += RefreshIntent;
        enemy.OnDied          += OnEnemyDied;

        RefreshHP(enemy.Data.maxHP, enemy.Data.maxHP);
        RefreshShield(0);

        if (enemy.CurrentIntent != null)
            RefreshIntent(enemy.CurrentIntent);
    }

    private void LateUpdate()
    {
        if (trackedEnemy == null) return;

        Vector3 worldPos = trackedEnemy.transform.position + Vector3.up * worldVerticalOffset;
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            screenPos,
            rootCanvas.worldCamera,
            out Vector2 localPos);

        rectTransform.anchoredPosition = localPos;
    }

    private void OnDestroy()
    {
        if (trackedEnemy == null) return;
        trackedEnemy.OnHPChanged     -= RefreshHP;
        trackedEnemy.OnShieldChanged -= RefreshShield;
        trackedEnemy.OnIntentChanged -= RefreshIntent;
        trackedEnemy.OnDied          -= OnEnemyDied;
    }

    private void RefreshHP(int current, int max)
    {
        if (hpBar != null) { hpBar.maxValue = max; hpBar.value = current; }
        if (hpText != null) hpText.text = $"{current}/{max}";
    }

    private void RefreshShield(int shield)
    {
        if (shieldContainer != null) shieldContainer.SetActive(shield > 0);
        if (shieldText != null) shieldText.text = shield.ToString();
    }

    private void RefreshIntent(EnemyActionData action)
    {
        if (intentIcon != null && action.intentIcon != null)
            intentIcon.sprite = action.intentIcon;

        if (intentValueText != null)
            intentValueText.text = action.value > 0 ? action.value.ToString() : string.Empty;

        if (intentLabel != null)
            intentLabel.text = action.label;
    }

    private void OnEnemyDied()
    {
        Destroy(gameObject);
    }
}
