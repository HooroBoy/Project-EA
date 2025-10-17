using UnityEngine;
[CreateAssetMenu(fileName = "ScriptableObjects/AccuracyMeteredShootingStrategy", menuName = "AccuracyMeteredShootingStrategy", order = 0)]
public class AccuracyMeteredShootingStrategy : ShootingStrategy
{
    [SerializeField] private float _inaccuracyAngle = 15f; // Maximum angle of inaccuracy in degrees
    [SerializeField] private float _accuracyPercentage = 0.8f; // Percentage of shots that should be accurate
    public override void Shoot(Transform target, Transform shootPoint, Bullet bulletPrefab)
    {
        var bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        var bulletRb = bullet.GetComponent<Rigidbody2D>();
        Vector2 direction = (target.position - shootPoint.position).normalized;

        if (Random.Range(0, 1f) > _accuracyPercentage)
        {
            // innacurate shot
            direction = GetInaccurateDirection(direction);
            Debug.Log("[AccuracyMeteredShootingStrategy] Inaccurate shot fired with angle offset.");
        }
        else
        {
            Debug.Log("[AccuracyMeteredShootingStrategy] Accurate shot fired.");
            // accurate shot, no change to direction
        }

        bulletRb.linearVelocity = direction * _bulletSpeed; // Adjust speed as needed
    }

    Vector2 GetInaccurateDirection(Vector2 originalDirection)
    {
        float angleOffset = Random.Range(-_inaccuracyAngle, _inaccuracyAngle);
        Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);
        return rotation * originalDirection;
    }
}