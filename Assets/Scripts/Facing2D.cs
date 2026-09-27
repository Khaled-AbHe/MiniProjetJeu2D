using UnityEngine;

/// <summary>
/// Shared helper for the "mirror localScale.x to face the other direction"
/// logic that was duplicated in both Enemy.cs and PlayerMovement.cs.
/// </summary>
public static class Facing2D
{
    /// <summary>
    /// Mirrors the transform's local X scale so the object (and any
    /// children - weapons, effects, etc.) faces the opposite direction.
    /// </summary>
    public static void Flip(Transform transform)
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }
}
