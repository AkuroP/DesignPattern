using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class bullet : MonoBehaviour
{
    [SerializeField] float _speed = 100f;
    
    private IObjectPool<bullet> _objectPool;

    public IObjectPool<bullet> ObjectPool { set => _objectPool = value;  }

    [SerializeField]private Rigidbody2D _rb;
    private Vector2 _target;
    public Vector2 Target { get { return _target; } set { _target = value; } }

    public float Speed { set => _speed = value; }
    void FixedUpdate()
    {
        //transform.Translate(target.normalized * Time.deltaTime * _speed);
        _rb.velocity = _target.normalized * _speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.name == "Wall")
            _objectPool.Release(this);
            //Destroy(gameObject);
    }

    private void OnDisable()
    {
        _rb.velocity = new Vector2(0f, 0f);
        _rb.angularVelocity = 0f;
    }
}
