using UnityEngine;

public class EnemySpriteRotationController : MonoBehaviour {
    [SerializeField] private DirectionFinderByLayer _directionFinder;
    [SerializeField] private Transform _spriteTransform;

    private void Start()
    {
        if (_directionFinder == null)
        {
            _directionFinder = GetComponent<DirectionFinderByLayer>();
        }

        if (_spriteTransform == null)
        {
            _spriteTransform = transform;
        }
    }

    private void Update()
    {
        Transform target = _directionFinder.FindNearestTarget();
        if (target != null)
        {
            Vector3 direction = target.position - _spriteTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _spriteTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}