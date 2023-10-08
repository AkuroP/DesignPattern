using System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField, Required("nop")] Health _health;
    public Health _Health { get => _health; }


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

    }

    public void DamageHitBox(Collider2D col, string colName)
    {
        if ((col.gameObject.layer == LayerMask.NameToLayer("PlayerSphere") 
             || col.gameObject.layer == LayerMask.NameToLayer("Power"))
            && CompareTag("Enemy") && colName is "HitBox" or "PowerSphere")
            if (!_health.IsDead)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Power"))
                {
                    _health.Damage(25);
                    return;
                }
                
                _health.Damage(10);
            }

        if (col.gameObject.layer == LayerMask.NameToLayer("EnemySphere") && CompareTag("Player") && colName == "HitBox")
            if (!_health.IsDead)_health.Damage(10);
    }
}
