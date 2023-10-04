using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class Health : MonoBehaviour, IHealth
{
    [SerializeField] int _maxHealth;

    /// <summary>
    /// coucou
    /// </summary>
    public int CurrentHealth 
    {
        get;
        private set;
    }
    public bool IsDead => CurrentHealth <= 0;
    public int MaxHealth { get => _maxHealth; }

    public event Action<int> OnDamage;
    public event Action<int> OnRegen;
    public event Action OnDie;

    [SerializeField] private ParticleSystem _deathParticle;
    [SerializeField] private SpriteRenderer _entitySR;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        OnDamage += Damaged;
        OnDie += Death;
    }

    private void OnDisable()
    {
        OnDamage -= Damaged;
        OnDie -= Death;
    }

    public void Damage(int amount)
    {
        Assert.IsTrue(amount >= 0);
        if (IsDead) return;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnDamage?.Invoke(amount);
    }
    public void Regen(int amount)
    {
        Assert.IsTrue(amount >= 0);
        if (IsDead) return;
        InternalRegen(amount);
    }
    public void Kill()
    {
        if (IsDead) return;
        InternalDie();
    }

    public void Revive(int amount)
    {
        Assert.IsTrue(amount >= 0);
        if (!IsDead) return;
        InternalRegen(amount);
    }

    void InternalRegen(int amount)
    {
        Assert.IsTrue(amount >= 0);

        var old = CurrentHealth;
        CurrentHealth = Mathf.Min(_maxHealth, CurrentHealth + amount);
        OnRegen?.Invoke(CurrentHealth-old);
    }
    void InternalDie()
    {
        if (!IsDead) return;
        OnDie?.Invoke();
    }

    private void Damaged(int amount)
    {
        if(IsDead)InternalDie();
        else StartCoroutine(DamageVFX());
        //hit sound
    }

    private IEnumerator DamageVFX()
    {
        _entitySR.color = Color.red;
        yield return new WaitForSeconds(.1f);
        _entitySR.color = Color.white;
    }

    private void Death()
    {
        DeathParticle();
        DeathVFX();
        //death sound

    }

    private void DeathParticle()
    {
        if (_deathParticle.isPlaying) return;
        _deathParticle?.gameObject.SetActive(true);
        _deathParticle?.Play();
    }

    private void DeathVFX() => _entitySR.color = Color.black;

}
