using Game;
using System.Collections;
using System.Collections.Generic;
using Game.Script.SoundManager;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Events;

public class EntityAttack : MonoBehaviour
{
    [SerializeField] AttackZone _attackZone;
    [SerializeField] ObjectPool bulletPool;
    
    [SerializeField] private AudioClip _fireSFX;

    public event UnityAction OnAttack;
    public event UnityAction OnFire;

    private float _fireCD;
    [SerializeField] private float _maxFireCD = .5f;

    private void OnEnable()
    {
        OnFire += FireSfxVfx;
    }

    private void OnDisable()
    {
        OnFire -= FireSfxVfx;
    }

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

    public void Fire(Vector3 target, float velocity, LayerMask shooterOrigin, Color bulletColor)
    {
        if (_fireCD > 0) return;
        
        OnFire?.Invoke();

        target.Normalize();
        bulletPool.FireBullet(this.transform, target, velocity, shooterOrigin, bulletColor);
    }

    private void FireSfxVfx()
    {
        _fireCD = _maxFireCD;
        //son tir
        ServiceLocator.Get().PlaySound(_fireSFX);

    }



}
