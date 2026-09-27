using System;
using UnityEngine;

public class PlayerMagic : MonoBehaviour
{
    [Header("Magic Pool")]
    [SerializeField] private int maxMagic = 100;

    [Header("Siphon Settings")]
    [Tooltip("Magic gained per point of damage dealt to enemies with the sword.")]
    [SerializeField] private float magicPerDamagePoint = 1.5f;

    private float currentMagic;

    // (currentMagic, maxMagic) - subscribe to this to drive a UI magic bar,
    // same pattern as PlayerHealth.OnHealthChanged.
    public static event Action<float, int> OnMagicChanged;

    public int MaxMagic => maxMagic;
    public float CurrentMagic => currentMagic;

    void Start()
    {
        currentMagic = 0f;
        OnMagicChanged?.Invoke(currentMagic, maxMagic);
    }

    /// <summary>
    /// Called by PlayerCombat whenever a sword hit lands on an enemy.
    /// This is the "siphon" — damage dealt converts into magic charge.
    /// </summary>
    public void SiphonFromDamage(int damageDealt)
    {
        AddMagic(damageDealt * magicPerDamagePoint);
    }

    public void AddMagic(float amount)
    {
        if (amount <= 0f) return;

        currentMagic = Mathf.Clamp(currentMagic + amount, 0, maxMagic);
        OnMagicChanged?.Invoke(currentMagic, maxMagic);
    }

    public bool HasEnoughMagic(int cost)
    {
        return currentMagic >= cost;
    }

    /// <summary>
    /// Attempts to spend magic. Returns false (and deducts nothing) if there
    /// isn't enough charge for the ability being cast.
    /// </summary>
    public bool TrySpendMagic(int cost)
    {
        if (!HasEnoughMagic(cost)) return false;

        currentMagic -= cost;
        OnMagicChanged?.Invoke(currentMagic, maxMagic);
        return true;
    }
}