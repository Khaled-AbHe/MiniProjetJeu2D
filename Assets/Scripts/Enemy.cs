using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    // Added 'Stunned' to our state machine
    private enum EnemyState { Patrolling, Chasing, Attacking, Stunned }

    [Header("Health System")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Movement & Waypoints")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Transform[] patrolPoints;
    private int currentWaypointIndex = 0;
    private bool movingRight = true;

    [Header("Detection Ranges")]
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1.2f;

    [Header("Attack Settings")]
    [SerializeField] private float windUpTime = 0.6f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 20;

    [Header("Knockback & Stun")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float stunDuration = 0.4f;

    private EnemyState currentState = EnemyState.Patrolling;
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
        // If the enemy is stunned or currently striking, don't change states
        if (playerTransform == null || isAttacking || currentState == EnemyState.Stunned) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange && canAttack)
        {
            StartCoroutine(PerformWindUpAttack());
        }
        else if (distanceToPlayer <= chaseRange)
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
            currentState = EnemyState.Patrolling;
        }
    }

    void FixedUpdate()
    {
        // Completely stop standard AI movement logic if attacked or stunned
        if (isAttacking || currentState == EnemyState.Stunned)
        {
            if (currentState != EnemyState.Stunned)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            anim.SetFloat("Speed", 0f);
            return;
        }

        if (currentState == EnemyState.Patrolling)
        {
            PatrolLogic();
        }
        else if (currentState == EnemyState.Chasing)
        {
            ChaseLogic();
        }

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    private void PatrolLogic()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentWaypointIndex];

        if (targetPoint.position.x > transform.position.x && !movingRight) Flip();
        else if (targetPoint.position.x < transform.position.x && movingRight) Flip();

        float xVelocity = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocity.y);

        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.3f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Length;
        }
    }

    private void ChaseLogic()
    {
        if (playerTransform.position.x > transform.position.x && !movingRight) Flip();
        else if (playerTransform.position.x < transform.position.x && movingRight) Flip();

        float xVelocity = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocity.y);
    }

    private void Flip()
    {
        movingRight = !movingRight;
        Facing2D.Flip(transform);
    }

    private IEnumerator PerformWindUpAttack()
    {
        isAttacking = true;
        canAttack = false;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetFloat("Speed", 0f);

        anim.SetTrigger("WindUp");
        yield return new WaitForSeconds(windUpTime);

        // Double check they didn't stun us during the wind-up phase
        if (currentState != EnemyState.Stunned)
        {
            anim.SetTrigger("Attack");

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange)
            {
                Debug.Log($"Wham! Dealt {attackDamage} damage to player.");
                playerTransform.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            }
        }

        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // --- NEW COMBAT LOGIC ---
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Enemy hurt! HP Remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Trigger stun and knockback if alive
        StopAllCoroutines(); // Cancels active wind-up attacks safely
        isAttacking = false;

        StartCoroutine(StunAndKnockbackRoutine());
    }

    private IEnumerator StunAndKnockbackRoutine()
    {
        currentState = EnemyState.Stunned;

        // Calculate direct horizontal direction away from the player
        float knockbackDirection = transform.position.x > playerTransform.position.x ? 1f : -1f;

        // Apply immediate physics force (adds snappy horizontal impact alongside slight vertical pop)
        rb.linearVelocity = new Vector2(knockbackDirection * knockbackForce, knockbackForce * 0.3f);

        // Wait out the recovery duration
        yield return new WaitForSeconds(stunDuration);

        // Return to normal functionality
        currentState = EnemyState.Patrolling;
        canAttack = true;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // Don't execute if there are no patrol points assigned
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = Color.cyan; // Choose a distinct color for your patrol path

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] != null)
            {
                // Draw a small solid sphere at the waypoint position
                Gizmos.DrawWireSphere(patrolPoints[i].position, 0.2f);

                // Draw a line connecting this waypoint to the next one
                int nextIndex = (i + 1) % patrolPoints.Length;
                if (patrolPoints[nextIndex] != null)
                {
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
                }
            }
        }
    }

}
