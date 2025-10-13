using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f; // How long the bullet exists before being destroyed

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}