using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Card))]
public class CardAnimator : MonoBehaviour
{
    private Coroutine moveCoroutine;
    private Coroutine scaleCoroutine;

    public void MoveTo(Vector3 targetPosition, float duration, Action onComplete = null)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveRoutine(targetPosition, duration, onComplete));
    }

    public void ScaleTo(Vector3 targetScale, float duration, Action onComplete = null)
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleRoutine(targetScale, duration, onComplete));
    }

    public void StopAllAnimations()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
    }

    private IEnumerator MoveRoutine(Vector3 target, float duration, Action onComplete)
    {
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
        onComplete?.Invoke();
    }

    private IEnumerator ScaleRoutine(Vector3 target, float duration, Action onComplete)
    {
        float elapsed = 0f;
        Vector3 start = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localScale = target;
        onComplete?.Invoke();
    }
}
