using System.Collections;
using UnityEngine;

public static class CombatVisualEffect
{
    private const float AttackDistance = 0.35f;
    private const float AttackMoveOutTime = 0.12f;
    private const float AttackMoveBackTime = 0.12f;

    private const float ShakeDuration = 0.16f;
    private const float ShakeAmount = 0.06f;

    public static IEnumerator PlayAttackMotion(Transform attacker, Transform target)
    {
        if (attacker == null || target == null)
            yield break;

        Vector3 startPosition = attacker.position;
        Vector3 direction = target.position - attacker.position;
        direction.z = 0f;

        if (direction.sqrMagnitude < 0.001f)
            direction = Vector3.right;

        direction.Normalize();
        Vector3 attackPosition = startPosition + direction * AttackDistance;

        yield return MoveWorld(attacker, startPosition, attackPosition, AttackMoveOutTime);
        yield return MoveWorld(attacker, attackPosition, startPosition, AttackMoveBackTime);

        if (attacker != null)
            attacker.position = startPosition;
    }

    public static IEnumerator PlayShake(Transform target)
    {
        if (target == null)
            yield break;

        Vector3 startLocalPosition = target.localPosition;
        float elapsed = 0f;

        while (elapsed < ShakeDuration)
        {
            if (target == null)
                yield break;

            Vector2 randomOffset = Random.insideUnitCircle * ShakeAmount;
            target.localPosition = startLocalPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (target != null)
            target.localPosition = startLocalPosition;
    }

    private static IEnumerator MoveWorld(Transform target, Vector3 from, Vector3 to, float duration)
    {
        if (target == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (target == null)
                yield break;

            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);
            target.position = Vector3.Lerp(from, to, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (target != null)
            target.position = to;
    }
}
