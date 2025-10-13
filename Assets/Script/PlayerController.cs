using UnityEngine;

// This script requires a Rigidbody2D component on the same GameObject.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // The projectile to be fired
    public Transform firePoint;     // The point from where the bullet is fired
    public float bulletForce = 20f; // The speed of the bullet
    public float fireRate = 0.5f;   // How many seconds to wait before the next shot

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 moveInput;
    private Vector2 mousePosition;
    private float nextFireTime = 0f;
    public Animator animator;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Get component references
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main; // Find the main camera in the scene
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // --- Input Handling ---
        // Get movement input (returns -1, 0, or 1)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        if ((Input.GetAxisRaw("Horizontal") != 0) || (Input.GetAxisRaw("Vertical") != 0))
        {
            animator.SetBool("IsRun",true);
        }else
        {
            animator.SetBool("IsRun",false);
            
        }
        // Get mouse position in the world
        // ScreenToWorldPoint is necessary to convert the mouse's pixel coordinates to game world coordinates
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // --- Shooting Input ---
        // Check for left mouse button click and if the cooldown has passed
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            // Update the next fire time based on the fire rate
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
        
    }

    // FixedUpdate is called on a fixed time interval and is best for physics calculations
    void FixedUpdate()
    {
        // --- Movement ---
        // Normalize the moveInput vector to prevent faster diagonal movement
        // and apply the movement to the rigidbody's velocity
        rb.velocity = moveInput.normalized * moveSpeed;

        // --- Aiming / Rotation ---
        // Calculate the direction from the player to the mouse
        Vector2 lookDirection = mousePosition - rb.position;

        // Calculate the angle in degrees. Atan2 returns the angle in radians,
        // so we convert it to degrees.
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        // Apply the rotation to the rigidbody.
        // We subtract 90 degrees because in Unity, a 0-degree rotation faces right (positive X-axis),
        // but most 2D character sprites are drawn facing up. If your sprite faces right, remove the "- 90f".
        rb.rotation = angle - 90f;
    }

    void Shoot()
    {
        // Instantiate the bullet at the firePoint's position and rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Get the Rigidbody2D component of the new bullet
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // Apply force to the bullet to make it move
        // We use firePoint.up because the firePoint is rotated with the player,
        // so its "up" direction is always facing where the player is aiming.
        bulletRb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }
}