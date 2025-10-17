using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects/ShootingStrategy1", menuName = "ShootingStrategy1", order = 0)]
public class ShootingStrategy1 : ShootingStrategy
{
    public override void Shoot(Transform target, Transform shootPoint, Bullet bulletPrefab)
    {
        var bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        var bulletRb = bullet.GetComponent<Rigidbody2D>();
        Vector2 direction = (target.position - shootPoint.position).normalized;
        bulletRb.linearVelocity = direction * _bulletSpeed; // Adjust speed as needed
    }
}