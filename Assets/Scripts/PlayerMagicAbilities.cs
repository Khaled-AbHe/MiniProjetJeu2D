using System.Collections;
using UnityEngine;

/// <summary>
/// The two abilities fueled by the siphoned magic bar (see PlayerMagic):
/// a damaging dash, and a self heal.
/// </summary>
[RequireComponent(typeof(PlayerMagic))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMagicAbilities : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask enemyLayers;

    [Header("Siphon Dash")]
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private int dashMagicCost = 30;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashDamageRadius = 0.6f;
    [SerializeField] private int dashDamage = 15;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Siphon Heal")]
    [SerializeField] private KeyCode healKey = KeyCode.Q;
    [SerializeField] private int healMagicCost = 40;
    [SerializeField] private int healAmount = 2;
    [SerializeField] private float healCastTime = 0.5f;

    private PlayerMagic playerMagic;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isDashOnCooldown = false;
    public bool IsCastingHeal { get; private set; } = false;

    void Start()
    {
        playerMagic = GetComponent<PlayerMagic>();
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!playerMovement.isGrounded) return;

        if (Input.GetKeyDown(dashKey) && !isDashOnCooldown && !playerMovement.isDashing)
        {
            TryStartDash();
        }

        if (Input.GetKeyDown(healKey) && !IsCastingHeal)
        {
            TryStartHeal();
        }
    }

    private void TryStartDash()
    {
        if (!playerMagic.TrySpendMagic(dashMagicCost))
        {
            Debug.Log("Not enough magic to Siphon Dash.");
            return;
        }

        StartCoroutine(SiphonDashRoutine());
    }

    private IEnumerator SiphonDashRoutine()
    {
        isDashOnCooldown = true;
        playerMovement.isDashing = true;

        float direction = playerMovement.IsFacingRight ? 1f : -1f;
        if (animator != null) animator.SetTrigger("IsStriking");

        float elapsed = 0f;
        bool hasDealtDamage = false;

        while (elapsed < dashDuration)
        {
            rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

            // Only need to land the damage once per dash, but keep checking
            // each frame in case the enemy wasn't in range at frame one.
            if (!hasDealtDamage)
            {
                hasDealtDamage = DealDashDamage();
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerMovement.isDashing = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashCooldown);
        isDashOnCooldown = false;
    }

    private bool DealDashDamage()
    {
        // Note: dash damage does NOT call playerMagic.SiphonFromDamage —
        // only sword hits (PlayerCombat) refill the bar, so dash can't fuel itself.
        int hitCount = CombatUtils.DamageEnemiesInRadius(transform.position, dashDamageRadius, enemyLayers, dashDamage);
        return hitCount > 0;
    }

    private void TryStartHeal()
    {
        if (!playerMagic.TrySpendMagic(healMagicCost))
        {
            Debug.Log("Not enough magic to Siphon Heal.");
            return;
        }

        StartCoroutine(SiphonHealRoutine());
    }

    private IEnumerator SiphonHealRoutine()
    {
        IsCastingHeal = true;
        if (animator != null) animator.SetTrigger("HealCast");

        yield return new WaitForSeconds(healCastTime);

        playerHealth.Heal(healAmount);
        IsCastingHeal = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, dashDamageRadius);
    }
}
