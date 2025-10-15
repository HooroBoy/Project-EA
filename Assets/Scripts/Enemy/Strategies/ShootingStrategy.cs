using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects/ShootingStrategy", menuName = "ShootingStrategy", order = 0)]
public abstract class ShootingStrategy : ScriptableObject
{
    [SerializeField] protected float _bulletSpeed = 10f;
    public abstract void Shoot(Transform target, Transform shootPoint, Bullet bulletPrefab);
}