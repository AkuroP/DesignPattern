using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Pool;

public class bullet : MonoBehaviour
{
    [SerializeField, BoxGroup("Bullet Parameter")] float _speed = 100f;
    public float Speed { set => _speed = value; }
    [SerializeField, BoxGroup("Bullet Parameter")] private Rigidbody2D _rb;
    
    //object pool parameter
    private IObjectPool<bullet> _objectPool;
    public IObjectPool<bullet> ObjectPool { get => _objectPool; set => _objectPool = value; }

    //bullet direction
    private Vector2 _bulletDir;
    public Vector2 BulletDir { get { return _bulletDir; } set { _bulletDir = value; } }

    [SerializeField, BoxGroup("Bullet Parameter")] private SpriteRenderer _bulletSR;
    public Color BulletSRColor { set => _bulletSR.color = value; }


    void FixedUpdate()
    {
        //transform.Translate(target.normalized * Time.deltaTime * _speed);
        _rb.velocity = _bulletDir.normalized * _speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && this.gameObject.layer == LayerMask.NameToLayer("EnemySphere"))
            if(collision.gameObject.name == "HitBox")DeactivateBullet();
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && this.gameObject.layer == LayerMask.NameToLayer("PlayerSphere"))
            if(collision.gameObject.name == "HitBox") DeactivateBullet();
        
        if(collision.gameObject.name == "Wall")
            DeactivateBullet();
        //Destroy(gameObject);
    }

    private void DeactivateBullet() => StartCoroutine(BulletRelease(.01f));

    IEnumerator BulletRelease(float delay)
    {
        yield return new WaitForSeconds(delay);
        _rb.velocity = new Vector2(0f, 0f);
        _rb.angularVelocity = 0f;
        _objectPool.Release(this);
    }
}
