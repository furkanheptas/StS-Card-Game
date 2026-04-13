using UnityEngine;
using TMPro;
using System.Collections;

public class BattleUIBroadcast : MonoBehaviour
{

    public static BattleUIBroadcast Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject bannerObject; 
    [SerializeField] private TextMeshProUGUI statusText; 

    private void Awake()
    {
        Instance = this;
        if (bannerObject != null) bannerObject.SetActive(false);
    }

    public IEnumerator ShowMessageRoutine(string message, float delay)
    {
        if (bannerObject == null) yield break;

        statusText.text = message;
        bannerObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        bannerObject.SetActive(false);
    }

}