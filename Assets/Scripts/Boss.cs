using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    private enum BossState { Patrolling, Chasing }

    [Header("Health System")]
    [SerializeField] private int maxHealth = 300;
    private int currentHealth;

    [Tooltip("Gate destroyed when the boss dies, opening the way forward.")]
    [SerializeField] private GameObject gate;

    [Header("Movement & Waypoints")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] patrolPoints;
    private int currentWaypointIndex = 0;
    private bool movingRight = true;

    [Header("Detection Ranges")]
    [SerializeField] private float chaseRange = 8f;
    [Tooltip("Distance from the player at which the boss starts a smash.")]
    [SerializeField] private float attackRange = 3f;

    [Header("Smash Attack")]
    [SerializeField] private string smashTrigger = "Smash";
    [Tooltip("Seconds from the start of the Smash animation until the fists hit the ground (the raised-arms wind-up).")]
    [SerializeField] private float smashImpactDelay = 0.8f;
    [Tooltip("Seconds from the ground impact until the Smash animation finishes.")]
    [SerializeField] private float smashRecoveryTime = 0.4f;
    [SerializeField] private float attackCooldown = 2.5f;
    [SerializeField] private int smashDamage = 30;
    [Tooltip("Where the smash lands (place an empty child in front of the fists). Falls back to the boss position.")]
    [SerializeField] private Transform smashPoint;
    [SerializeField] private float smashRadius = 2f;

    private BossState currentState = BossState.Patrolling;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isAttacking = false;
    private bool canAttack = true;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag(Tags.Player);
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        // Mid-smash the boss commits to the attack and doesn't change state.
        if (playerTransform == null || isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange && canAttack)
        {
            StartCoroutine(PerformSmashAttack());
        }
        else if (distanceToPlayer <= chaseRange)
        {
            currentState = BossState.Chasing;
        }
        else
        {
            currentState = BossState.Patrolling;
        }
    }

    void FixedUpdate()
    {
        // Stationary while attacking.
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            anim.SetFloat("Speed", 0f);
            return;
        }

        if (currentState == BossState.Patrolling)
        {
            PatrolLogic();
        }
        else
        {
            ChaseLogic();
        }

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    private void PatrolLogic()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentWaypointIndex];

        FaceTowards(targetPoint.position.x);

        float xVelocity = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocity.y);

        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.3f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Length;
        }
    }

    private void ChaseLogic()
    {
        FaceTowards(playerTransform.position.x);

        float xVelocity = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocity.y);
    }

    private void FaceTowards(float targetX)
    {
        if (targetX > transform.position.x && !movingRight) Flip();
        else if (targetX < transform.position.x && movingRight) Flip();
    }

    private void Flip()
    {
        movingRight = !movingRight;
        Facing2D.Flip(transform);
    }

    private IEnumerator PerformSmashAttack()
    {
        isAttacking = true;
        canAttack = false;

        // Plant its feet and face the player before raising its arms.
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        anim.SetFloat("Speed", 0f);
        FaceTowards(playerTransform.position.x);

        anim.SetTrigger(smashTrigger);

        // Arms rise...
        yield return new WaitForSeconds(smashImpactDelay);

        // ...and the fists hit the ground.
        DealSmashDamage();

        yield return new WaitForSeconds(smashRecoveryTime);
        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void DealSmashDamage()
    {
        if (playerTransform == null) return;

        Vector2 impactPoint = smashPoint != null ? (Vector2)smashPoint.position : (Vector2)transform.position;

        // Player can dodge by being outside the radius (e.g. jumping clear).
        if (Vector2.Distance(impactPoint, playerTransform.position) <= smashRadius)
        {
            Debug.Log($"SMASH! Dealt {smashDamage} damage to player.");
            playerTransform.GetComponent<PlayerHealth>()?.TakeDamage(smashDamage);
        }
    }

    // The boss takes damage but has no knockback or stun, and taking a hit
    // never interrupts a smash in progress.
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Boss hurt! HP Remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (gate != null)
        {
            Destroy(gate);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        Vector3 impact = smashPoint != null ? smashPoint.position : transform.position;
        Gizmos.DrawWireSphere(impact, smashRadius);
    }

    private void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] != null)
            {
                Gizmos.DrawWireSphere(patrolPoints[i].position, 0.2f);

                int nextIndex = (i + 1) % patrolPoints.Length;
                if (patrolPoints[nextIndex] != null)
                {
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
                }
            }
        }
    }
}
