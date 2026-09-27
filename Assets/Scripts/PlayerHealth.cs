using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    private bool isDead = false;

    public static event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    public static event Action OnPlayerDeath;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            OnPlayerDeath?.Invoke();
            // Note: the GameObject is deliberately NOT destroyed here.
            // PlayerDeath handles disabling control and playing the death
            // animation, so the body stays visible behind the Game Over
            // screen instead of vanishing instantly.
        }
    }

    public void Heal(int healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
