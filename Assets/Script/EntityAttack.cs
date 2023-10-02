using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityAttack : MonoBehaviour
{
    [SerializeField] AttackZone _attackZone;
    [SerializeField] ObjectPool bulletPool;

    public event UnityAction OnAttack;
    public event UnityAction OnFire;

    private float fireCD;
    [SerializeField] private float maxFireCD = .5f;

    private void FixedUpdate()
    {
        if (fireCD > 0) fireCD -= Time.deltaTime;
    }

    public void LaunchAttack()
    {
        OnAttack?.Invoke();
        foreach (var el in _attackZone.InZone)
        {
            el.Damage(10);
        }
    }

    public void Fire()
    {
        if (fireCD > 0 && this == null) return;
        OnFire?.Invoke();
        fireCD = maxFireCD;
        bulletPool.FireBullet(this.transform, 10f);
    }



}
