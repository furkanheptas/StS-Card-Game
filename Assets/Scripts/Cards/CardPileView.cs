using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPileView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image[] stackImages;

    private const float StackOffsetX = 2f;
    private const float StackOffsetY = 2f;

    private void Awake()
    {
        RefreshStackVisual(0);
    }

    public void SetCount(int count)
    {
        if (countText != null)
            countText.text = count.ToString();

        RefreshStackVisual(count);
    }

    private void RefreshStackVisual(int count)
    {
        if (stackImages == null) return;

        for (int i = 0; i < stackImages.Length; i++)
        {
            bool visible = count > i;
            stackImages[i].gameObject.SetActive(visible);

            if (visible)
            {
                RectTransform rt = stackImages[i].rectTransform;
                rt.anchoredPosition = new Vector2(-i * StackOffsetX, -i * StackOffsetY);
            }
        }
    }
}
