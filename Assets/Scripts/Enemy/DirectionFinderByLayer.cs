using UnityEngine;

public class DirectionFinderByLayer : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayerMask;
    [SerializeField] private Transform _originPoint;
    [SerializeField] private float _detectionRadius = 5f;
    private void Start()
    {
        if (_originPoint == null)
        {
            _originPoint = transform;
        }
    }

    public Transform FindNearestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(_originPoint.position, _detectionRadius, _targetLayerMask);
        Transform nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float distance = Vector2.Distance(_originPoint.position, hit.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = hit.transform;
            }
        }

        return nearestTarget;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_originPoint.position, _detectionRadius);
    }
}