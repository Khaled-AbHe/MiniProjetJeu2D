using System;
using UnityEngine;

/// <summary>
/// Shared "hit everything in a radius on a layer mask" combat helper, used
/// by both the sword swing (PlayerCombat) and the Siphon Dash
/// (PlayerMagicAbilities). Previously each script had its own copy of the
/// OverlapCircleAll -> loop -> TryGetComponent&lt;Enemy&gt; -> TakeDamage logic.
/// </summary>
public static class CombatUtils
{
    /// <summary>
    /// Deals damage to every Enemy found inside the given circle.
    /// Returns the number of enemies hit. An optional callback fires once
    /// per enemy hit, e.g. to trigger a magic siphon effect per hit.
    /// </summary>
    public static int DamageEnemiesInRadius(
        Vector2 origin,
        float radius,
        LayerMask enemyLayers,
        int damage,
        Action<Enemy> onHit = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, enemyLayers);
        int hitCount = 0;

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
                onHit?.Invoke(enemy);
                hitCount++;
            }
        }

        return hitCount;
    }
}
