using System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField, Required("nop")] Health _health;

    [SerializeField] int _baseSpeed;
    public Alterable<int> CurrentSpeed { get; private set; }
    
    private void Awake()
    {
        CurrentSpeed = new Alterable<int>(_baseSpeed);
    }

    private void Update()
    {
        CurrentSpeed.AddTransformator(i => i + 20, 10);

        var token = CurrentSpeed.AddTransformator(i => (int)(i * 1.5f), 5);

        var s = CurrentSpeed.CalculateValue();

        CurrentSpeed.RemoveTransformator(token);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Sphere") && CompareTag("Enemy"))
        {
            _health.Damage(50);
            Debug.Log(_health.CurrentHealth);
        }
    }
}
