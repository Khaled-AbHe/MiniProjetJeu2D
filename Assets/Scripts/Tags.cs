using UnityEngine;

/// <summary>
/// Central place for GameObject tag string literals used across scripts.
/// Avoids typo-prone repetition of raw strings like "Player" or "Batterie"
/// in Hazard, PorteSortie, ZoneInterdite, Collecteur, and Enemy.
/// </summary>
public static class Tags
{
    public const string Player = "Player";
    public const string Batterie = "Batterie";
}
