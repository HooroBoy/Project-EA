using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- Movement Settings ---
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movementInput;

    // --- Body Rotation Settings ---
    [Header("Body Rotation")]
    [Tooltip("Assign the GameObject that represents the player's lower body (legs, etc.).")]
    public Transform lowerBodyTransform; 
    [Tooltip("Assign the GameObject that represents the player's upper body (torso, arms, weapon).")]
    public Transform upperBodyTransform; 
    [Tooltip("Offset angle for lower body rotation if your sprite isn't facing 'up' by default.")]
    public float lowerBodyRotationOffset = -90f;
    [Tooltip("Offset angle for upper body rotation if your sprite isn't facing 'up' by default.")]
    public float upperBodyRotationOffset = -90f;

    // --- Shooting Settings ---
    [Header("Shooting")]
    [Tooltip("The prefab of the projectile to be spawned.")]
    public GameObject projectilePrefab; 
    [Tooltip("An empty GameObject positioned at the end of the weapon, indicating where projectiles spawn.")]
    public Transform firePoint;          
    [Tooltip("How many shots per second the player can fire.")]
    public float fireRate = 5f;          
    [Tooltip("The speed at which projectiles travel.")]
    public float projectileSpeed = 15f;
    private float nextFireTime = 0f; // Stores the time when the player can fire next

    // --- Health Settings (Basic) ---
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    // --- Animation Settings ---
    [Header("Animation")]
    [Tooltip("Animator for the player's lower body (legs).")]
    public Animator lowerBodyAnimator;
    [Tooltip("Animator for the player's upper body (torso, arms, weapon).")]
    public Animator upperBodyAnimator;

    // Animator Parameter Names (use these exact strings in your Unity Animator setup!)
    private const string LOWER_BODY_SPEED_PARAM = "Speed"; // Float: 0 for idle, >0 for walking
    private const string UPPER_BODY_SHOOT_TRIGGER = "Shoot"; // Trigger: To play a shooting animation
    // private const string UPPER_BODY_AIM_ANGLE_PARAM = "AimAngle"; // Optional: Float for complex aim blending

    // --- Initialization ---
    void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Basic error checking
        if (rb == null) Debug.LogError("Rigidbody2D not found on PlayerController GameObject!");
        if (lowerBodyTransform == null) Debug.LogError("LowerBodyTransform not assigned!");
        if (upperBodyTransform == null) Debug.LogError("UpperBodyTransform not assigned!");
        if (projectilePrefab == null) Debug.LogWarning("Projectile Prefab not assigned. Player won't be able to shoot.");
        if (firePoint == null) Debug.LogWarning("Fire Point not assigned. Projectiles might spawn at player origin.");
        if (lowerBodyAnimator == null) Debug.LogWarning("Lower Body Animator not assigned. Lower body animations won't play.");
        if (upperBodyAnimator == null) Debug.LogWarning("Upper Body Animator not assigned. Upper body animations won't play.");
    }

    // --- Called once per frame ---
    void Update()
    {
        HandleInput();
        RotateUpperBody();
        HandleShooting();
        UpdateAnimations(); // Call this to update animator parameters
    }

    // --- Called a fixed number of times per second (for physics) ---
    void FixedUpdate()
    {
        MovePlayer();
        RotateLowerBody();
    }

    // --- Input Handling ---
    void HandleInput()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput.Normalize(); 
    }

    // --- Player Movement ---
    void MovePlayer()
    {
        rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }

    // --- Lower Body Rotation (based on movement direction) ---
    void RotateLowerBody()
    {
        if (movementInput != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg + lowerBodyRotationOffset;
            lowerBodyTransform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        }
    }

    // --- Upper Body Rotation (based on mouse cursor position) ---
    void RotateUpperBody()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(upperBodyTransform.position).z; 
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector2 lookDir = mouseWorldPos - upperBodyTransform.position;
        float targetAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + upperBodyRotationOffset;
        upperBodyTransform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    // --- Shooting Logic ---
    void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    // --- Instantiate and launch projectile ---
    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;
        if (upperBodyAnimator != null)
        {
            upperBodyAnimator.SetTrigger(UPPER_BODY_SHOOT_TRIGGER); // Trigger shooting animation
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            projectileRb.linearVelocity = firePoint.up * projectileSpeed;
        }
        else
        {
            Debug.LogWarning("Projectile prefab is missing a Rigidbody2D component!");
        }
    }

    // --- Animation Updates ---
    void UpdateAnimations()
    {
        // Lower Body Animation: Control movement speed blend
        if (lowerBodyAnimator != null)
        {
            // Use the magnitude of movementInput to determine speed (0 for idle, >0 for walk)
            lowerBodyAnimator.SetFloat(LOWER_BODY_SPEED_PARAM, movementInput.magnitude);
        }

        // Upper Body Animation: (Shooting trigger already handled in Shoot() method)
        // You could add other upper body animations here if needed, e.g., for different weapon types or reload.
    }

    // --- Basic Health System (Example) ---
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage. Current Health: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        // TODO: Implement death animation, reload scene, game over screen, etc.
        Destroy(gameObject); 
    }
}