using System;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    public event Action Die;
    private int _currentHealth;
    void Start()
    {
        _currentHealth = _maxHealth;
    }

    void OnEnable()
    {
        Die += Perish;
    }
    void OnDisable()
    {
        Die -= Perish;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die?.Invoke();
        }
    }
    private void Perish()
    {
        // Handle enemy death (e.g., play animation, drop loot, etc.)
        Destroy(gameObject);
    }
}