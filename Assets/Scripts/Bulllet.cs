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
            Debug.Log("Bullet hit non-damagable layer");
            return;
        }
        Debug.Log("Bullet hit " + collision.gameObject.name);
        var healthController = collision.gameObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            healthController.TakeDamage(100000); // Deal 10 damage on hit
        }
        Destroy(gameObject);
    }
}