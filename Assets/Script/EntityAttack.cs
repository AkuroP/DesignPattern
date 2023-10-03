using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Events;

public class EntityAttack : MonoBehaviour
{
    [SerializeField] AttackZone _attackZone;
    [SerializeField] ObjectPool bulletPool;

    public event UnityAction OnAttack;
    public event UnityAction OnFire;

    private float _fireCD;
    [SerializeField] private float _maxFireCD = .5f;

    private void FixedUpdate()
    {
        if (_fireCD > 0) _fireCD -= Time.deltaTime;
    }

    public void LaunchAttack()
    {
        OnAttack?.Invoke();
        foreach (var el in _attackZone.InZone)
        {
            el.Damage(10);
        }
    }

    public void Fire(Vector3 target, float velocity)
    {
        if (_fireCD > 0) return;
        OnFire?.Invoke();
        _fireCD = _maxFireCD;
        target.Normalize();
        bulletPool.FireBullet(this.transform, target, velocity);
    }



}
