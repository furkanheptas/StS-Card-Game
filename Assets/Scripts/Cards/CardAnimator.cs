using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Card))]
public class CardAnimator : MonoBehaviour
{
    private Coroutine moveCoroutine;
    private Coroutine scaleCoroutine;
    private Coroutine rotateCoroutine;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void MoveTo(Vector2 targetAnchoredPosition, float duration, Action onComplete = null)
    {
        if (!gameObject.activeInHierarchy)
        {
            rectTransform.anchoredPosition = targetAnchoredPosition;
            onComplete?.Invoke();
            return;
        }
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveRoutine(targetAnchoredPosition, duration, onComplete));
    }

    public void ScaleTo(Vector3 targetScale, float duration, Action onComplete = null)
    {
        if (!gameObject.activeInHierarchy)
        {
            rectTransform.localScale = targetScale;
            onComplete?.Invoke();
            return;
        }
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleRoutine(targetScale, duration, onComplete));
    }

    public void RotateTo(Quaternion targetRotation, float duration, Action onComplete = null)
    {
        if (!gameObject.activeInHierarchy)
        {
            rectTransform.localRotation = targetRotation;
            onComplete?.Invoke();
            return;
        }
        if (rotateCoroutine != null) StopCoroutine(rotateCoroutine);
        rotateCoroutine = StartCoroutine(RotateRoutine(targetRotation, duration, onComplete));
    }

    public void StopAllAnimations()
    {
        if (moveCoroutine   != null) StopCoroutine(moveCoroutine);
        if (scaleCoroutine  != null) StopCoroutine(scaleCoroutine);
        if (rotateCoroutine != null) StopCoroutine(rotateCoroutine);
    }

    private IEnumerator MoveRoutine(Vector2 target, float duration, Action onComplete)
    {
        float elapsed = 0f;
        Vector2 start = rectTransform.anchoredPosition;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(start, target, Mathf.SmoothStep(0f, 1f, elapsed / duration));
            yield return null;
        }
        rectTransform.anchoredPosition = target;
        onComplete?.Invoke();
    }

    private IEnumerator ScaleRoutine(Vector3 target, float duration, Action onComplete)
    {
        float elapsed = 0f;
        Vector3 start = rectTransform.localScale;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, elapsed / duration));
            yield return null;
        }
        rectTransform.localScale = target;
        onComplete?.Invoke();
    }

    private IEnumerator RotateRoutine(Quaternion target, float duration, Action onComplete)
    {
        float elapsed = 0f;
        Quaternion start = rectTransform.localRotation;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.localRotation = Quaternion.Slerp(start, target, Mathf.SmoothStep(0f, 1f, elapsed / duration));
            yield return null;
        }
        rectTransform.localRotation = target;
        onComplete?.Invoke();
    }
}
