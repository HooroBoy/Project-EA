using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    // A simple state machine to define enemy behavior
    private enum State
    {
        Idle,       // Doing nothing, waiting for the player
        Chasing,    // Player is detected, moving towards/aiming
        Attacking   // Firing at the player
    }
    private State currentState;

    [Header("References")]
    public Transform playerTransform; // The player to target
    private Rigidbody2D playerRb;     // To check if the player is moving

    [Header("Senses")]
    public float hearingRadius = 10f; // The range at which the enemy can "hear" footsteps
    public float timeToLoseTarget = 3f; // Seconds of silence before the enemy gives up
    private float targetLostTimer;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 15f;
    public float fireRate = 1f; // How many seconds between shots
    private float nextFireTime = 0f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Find the player automatically by tag. Make sure your player GameObject is tagged "Player".
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerRb = player.GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.LogError("EnemyAI Error: Player not found! Make sure the player has the 'Player' tag.");
            enabled = false; // Disable script if no player is found
        }
    }

    void Start()
    {
        // Start in the Idle state
        currentState = State.Idle;
    }

    void Update()
    {
        // The core of our state machine. We call a different method based on the current state.
        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Chasing:
                ChasingState();
                break;
            case State.Attacking:
                AttackingState();
                break;
        }
    }

    // --- STATE LOGIC ---

    private void IdleState()
    {
        // In the Idle state, the enemy does nothing but listen.
        // The logic for detecting the player is in CheckForPlayer().
        CheckForPlayer();
    }

    private void ChasingState()
    {
        if (playerTransform == null) return;

        // Aim at the player
        AimAtPlayer();

        // Check if we can still hear the player. If not, start the "lose target" timer.
        if (!IsPlayerAudible())
        {
            targetLostTimer -= Time.deltaTime;
            if (targetLostTimer <= 0)
            {
                // We've lost the target, go back to idle
                currentState = State.Idle;
            }
        }
        else
        {
            // If we can hear the player, reset the timer
            targetLostTimer = timeToLoseTarget;
        }

        // If it's time to shoot, switch to the Attacking state
        if (Time.time >= nextFireTime)
        {
            currentState = State.Attacking;
        }
    }

    private void AttackingState()
    {
        if (playerTransform == null) return;

        // Keep aiming
        AimAtPlayer();

        // Fire a bullet
        Shoot();

        // Set the cooldown for the next shot
        nextFireTime = Time.time + fireRate;

        // Immediately switch back to Chasing state to re-evaluate the situation.
        // This makes the AI responsive.
        currentState = State.Chasing;
    }

    // --- HELPER METHODS ---

    private void CheckForPlayer()
    {
        if (IsPlayerAudible())
        {
            // Player is audible, switch to Chasing state
            currentState = State.Chasing;
            targetLostTimer = timeToLoseTarget; // Reset the lose timer
        }
    }

    private bool IsPlayerAudible()
    {
        if (playerTransform == null || playerRb == null) return false;

        // Calculate distance to the player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // The player is "audible" if they are within the hearing radius AND moving.
        // We check velocity.magnitude to see how fast the player is moving.
        bool isPlayerMoving = playerRb.velocity.magnitude > 0.1f;

        return distanceToPlayer <= hearingRadius && isPlayerMoving;
    }
    
    private void AimAtPlayer()
    {
        Vector2 lookDirection = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }
    
    // This allows you to see the hearing radius in the editor for easy debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}