using System;
using UnityEngine;

/// <summary>
/// Shared "hit everything in a radius on a layer mask" combat helper, used
/// by both the sword swing (PlayerCombat) and the Siphon Dash
/// (PlayerMagicAbilities). Damages anything implementing IDamageable
/// (Enemy, Boss, ...).
/// </summary>
public static class CombatUtils
{
    /// <summary>
    /// Deals damage to every IDamageable found inside the given circle.
    /// Returns the number of targets hit. An optional callback fires once
    /// per target hit, e.g. to trigger a magic siphon effect per hit.
    /// </summary>
    public static int DamageEnemiesInRadius(
        Vector2 origin,
        float radius,
        LayerMask enemyLayers,
        int damage,
        Action<IDamageable> onHit = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, enemyLayers);
        int hitCount = 0;

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.TakeDamage(damage);
                onHit?.Invoke(target);
                hitCount++;
            }
        }

        return hitCount;
    }
}
