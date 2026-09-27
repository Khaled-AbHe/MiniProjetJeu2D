using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Combat Settings")]
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public float attackRate = 2f; // Attacks per second
    private float nextAttackTime = 0f;

    private Animator animator;
    private PlayerMovement pm;
    private PlayerMagic playerMagic;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        playerMagic = GetComponent<PlayerMagic>();
    }

    void Update()
    {
        // Enforce an attack cooldown, and don't allow attacking mid-dash
        if (Time.time >= nextAttackTime && pm.isGrounded && !pm.isDashing)
        {
            // Uses standard Unity input. Change to your preferred input system if needed.
            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    public void StartAttackSlowdown()
    {
        if (pm != null)
        {
            pm.slowPlayer = true;
        }
    }
    // CALL THIS AT THE END OF THE ATTACK ANIMATION
    public void EndAttackSlowdown()
    {
        if (pm != null)
        {
            pm.slowPlayer = false; // Restores normal speed
        }
    }

    void Attack()
    {
        // 1. Play the attack animation
        animator.SetTrigger("Attack");
    }

    // CRITICAL: This method will be called directly by the Animation Event
    public void PerformHitDetection()
    {
        // Detects and damages every enemy in range; each hit also siphons
        // magic proportional to the damage dealt (see PlayerMagic).
        CombatUtils.DamageEnemiesInRadius(
            attackPoint.position,
            attackRange,
            enemyLayers,
            attackDamage,
            enemy => playerMagic?.SiphonFromDamage(attackDamage)
        );
    }

    // Visualizes the attack radius in the Unity Scene View for easy tuning
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
