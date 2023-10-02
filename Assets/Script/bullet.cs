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
    public Rigidbody2D Rb
    {
        get { return rb; }
        set { rb = value; }
    }
    void Update()
    {
        transform.Translate(Vector3.right*Time.deltaTime*_speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.name == "Wall")
            //Destroy(gameObject);
            objectPool.Release(this);
    }

    private void OnDisable()
    {
        rb.velocity = new Vector2(0f, 0f);
        rb.angularVelocity = 0f;
    }
}
