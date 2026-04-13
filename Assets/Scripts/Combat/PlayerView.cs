using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerView : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Shield")]
    [SerializeField] private GameObject shieldContainer;
    [SerializeField] private TextMeshProUGUI shieldText;

    [Header("Visual Effects")]
    [Tooltip("Can azaldığında barın ne kadar süre titreyeceği.")]
    [SerializeField] private float shakeDuration = 0.2f;
    [Tooltip("Titreme şiddeti.")]
    [SerializeField] private float shakeAmount = 5f;

    private Player player;
    private Vector3 originalPos;

    public void Initialize(Player source)
    {
        player = source;
        player.OnHPChanged += RefreshHP;
        player.OnShieldChanged += RefreshShield;
        player.OnDied += HandleGameOver; 

        originalPos = transform.localPosition;

        RefreshHP(source.CurrentHP, source.MaxHP);
        RefreshShield(0);
    }

    private void OnDestroy()
    {
        if (player == null) return;
        player.OnHPChanged -= RefreshHP;
        player.OnShieldChanged -= RefreshShield;
        player.OnDied -= HandleGameOver;
    }

    private void RefreshHP(int current, int max)
    {
        if (hpBar != null)
        {
            hpBar.maxValue = max;
            hpBar.value = current;
        }
        if (hpText != null) hpText.text = $"{current}/{max}";

        StopAllCoroutines();
        StartCoroutine(ShakeEffect());
    }

    private void RefreshShield(int shield)
    {
        if (shieldContainer != null) shieldContainer.SetActive(shield > 0);
        if (shieldText != null) shieldText.text = shield.ToString();
    }

    private IEnumerator ShakeEffect()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

    private void HandleGameOver()
    {
        Debug.Log("<color=red>OYUN BİTTİ - KAYBETTİN!</color>");

        if (BattleUIBroadcast.Instance != null)
        {
            BattleUIBroadcast.Instance.StartCoroutine(
                BattleUIBroadcast.Instance.ShowMessageRoutine("OYUN BİTTİ - KAYBETTİN!", 5f)
            );
        }
    }
}