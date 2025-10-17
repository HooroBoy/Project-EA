using UnityEngine;

public class EnemyShootController : MonoBehaviour {
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Bullet _bulletPrefab;

    [SerializeField] private DirectionFinderByLayer _directionFinder;

    [SerializeField] private ShootingStrategy _shootingStrategy;

    [SerializeField] private float _shootInterval = 2f;
    private float _timeSinceLastShot = 0f;
    [SerializeField] private float _detectionInterval = 0.5f;
    private float _timeSinceLastDetection = 0f;

    private void Start()
    {
        if (_shootPoint == null)
        {
            _shootPoint = transform;
        }

        if (_directionFinder == null)
        {
            _directionFinder = GetComponent<DirectionFinderByLayer>();
        }
    }

    private void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
        _timeSinceLastDetection += Time.deltaTime;

        if (_timeSinceLastDetection >= _detectionInterval)
        {
            Debug.Log("Detecting targets...");
            Transform target = _directionFinder.FindNearestTarget();
            if (target != null && _timeSinceLastShot >= _shootInterval)
            {
                Debug.Log("Shooting at target: " + target.name);
                _shootingStrategy.Shoot(target, _shootPoint, _bulletPrefab);
                _timeSinceLastShot = 0f;
            }
            _timeSinceLastDetection = 0f;
        }
    }
    
}