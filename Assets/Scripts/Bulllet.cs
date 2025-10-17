using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask damagableLayers;
    public float lifeTime = 3f; // How long the bullet exists before being destroyed

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((damagableLayers.value & (1 << collision.gameObject.layer)) == 0)
        {
            return;
        }
        var healthController = collision.gameObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            healthController.TakeDamage(100); // Deal 10 damage on hit
        }
        Destroy(gameObject);
    }
}