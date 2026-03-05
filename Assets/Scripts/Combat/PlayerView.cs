using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Shield")]
    [SerializeField] private GameObject shieldContainer;
    [SerializeField] private TextMeshProUGUI shieldText;

    private Player player;

    public void Initialize(Player source)
    {
        player = source;
        player.OnHPChanged     += RefreshHP;
        player.OnShieldChanged += RefreshShield;

        RefreshHP(source.CurrentHP, source.MaxHP);
        RefreshShield(0);
    }

    private void OnDestroy()
    {
        if (player == null) return;
        player.OnHPChanged     -= RefreshHP;
        player.OnShieldChanged -= RefreshShield;
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
}
