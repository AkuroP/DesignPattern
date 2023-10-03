using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class bullet : MonoBehaviour
{
    [SerializeField] float _speed = 100f;
    
    private IObjectPool<bullet> objectPool;

    public IObjectPool<bullet> ObjectPool { set => objectPool = value;  }

    [SerializeField]private Rigidbody2D rb;
    private Vector2 target;
    public Vector2 Target { get { return target; } set { target = value; } }

    public float Speed { set => _speed = value; }

    public Rigidbody2D Rb
    {
        get { return rb; }
        set { rb = value; }
    }
    void FixedUpdate()
    {
        //transform.Translate(target.normalized * Time.deltaTime * _speed);
        rb.velocity = target.normalized * _speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.name == "Wall")
            objectPool.Release(this);
            //Destroy(gameObject);
    }

    private void OnDisable()
    {
        rb.velocity = new Vector2(0f, 0f);
        rb.angularVelocity = 0f;
    }
}
