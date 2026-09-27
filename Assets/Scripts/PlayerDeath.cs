using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles what happens to the player GameObject itself when PlayerHealth
/// reports death: disables player control scripts, plays a death
/// animation, then signals that the sequence has finished so something
/// else (GameOverUI) can bring up the Game Over screen.
/// </summary>
[RequireComponent(typeof(PlayerHealth))]
public class PlayerDeath : MonoBehaviour
{
    [Header("Death Animation")]
    [Tooltip("How long to wait after triggering the death animation before showing the Game Over screen. Match this to the animation's length.")]
    [SerializeField] private float deathAnimationDuration = 1.5f;

    [Header("Scripts To Disable On Death")]
    [Tooltip("Drag PlayerMovement, PlayerCombat, PlayerMagicAbilities, etc. here so the player stops responding to input immediately on death.")]
    [SerializeField] private Behaviour[] scriptsToDisable;

    private Rigidbody2D rb;
    private Animator animator;

    /// <summary>
    /// Fired once the death animation has finished playing.
    /// GameOverUI listens for this to bring up the Game Over screen.
    /// </summary>
    public static event Action OnDeathSequenceFinished;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += HandleDeath;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        foreach (Behaviour script in scriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }

        // Stop horizontal motion but let gravity keep affecting the body.
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (animator != null) animator.SetTrigger("Dead");

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        OnDeathSequenceFinished?.Invoke();
    }
}
