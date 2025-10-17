using UnityEngine;

public class BossShootController : MonoBehaviour
{
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private DirectionFinderByLayer _directionFinder;
    [SerializeField] private float _shootInterval = 2f;
    [SerializeField][Tooltip("Number of shots per interval")] private float _numberOfShots = 5;
    private float _timeSinceLastShot = 0f;

    void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
        if (_timeSinceLastShot >= _shootInterval)
        {
            ShootBurst();
            _timeSinceLastShot = 0f;
        }
    }

    void ShootBurst()
    {
        if ( _directionFinder == null || _shootPoint == null || _bulletPrefab == null || _numberOfShots <= 0) return;

        float angleStep = 360f / _numberOfShots;
        float angle = 0f;

        Vector2 targetDirection = Vector2.right; // Default direction
        Transform target = _directionFinder.FindNearestTarget();
        if (target != null)
        {
            targetDirection = (target.position - _shootPoint.position).normalized;
        }

        for (int i = 0; i < _numberOfShots; i++)
        {
            float bulletDirXPosition = _shootPoint.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float bulletDirYPosition = _shootPoint.position.y + Mathf.Cos((angle * Mathf.PI) / 180);

            Vector3 bulletVector = new Vector3(bulletDirXPosition, bulletDirYPosition, 0);
            Vector2 bulletMoveDirection = (bulletVector - _shootPoint.position + (Vector3)targetDirection).normalized;

            var bullet = Instantiate(_bulletPrefab, _shootPoint.position, Quaternion.identity);
            var bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.linearVelocity = bulletMoveDirection * 5f; // Adjust speed as needed

            angle += angleStep;
        }
    }

}