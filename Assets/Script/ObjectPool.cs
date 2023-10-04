using PlasticGui.WorkspaceWindow.Home;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Game
{
    public class ObjectPool : MonoBehaviour
    {

        [SerializeField]private bullet _bulletPrefab;

        private IObjectPool<bullet> _bulletPool;

        [SerializeField] private bool _collectionCheck = true;

        [Tooltip("Cooldown avant spawn de l'objet")]
        [SerializeField] private float _bulletMaxCD = .1f;
        private float _bulletCD;

        [SerializeField] private int _objectDefaultCapacity = 20;
        [SerializeField] private int _objectMaxSize = 50;

        // Start is called before the first frame update
        void Awake()
        {
            _bulletPool = new ObjectPool<bullet>(CreateObject, OnGetFromPool, OnReleaseToPool, OnDestroyToPooledObject, _collectionCheck, _objectDefaultCapacity, _objectMaxSize) ;
        }

        private void Start()
        {
            _bulletCD = _bulletMaxCD;

            for(int i = 0; i < _objectDefaultCapacity; i++)
            {
                bullet bullet = CreateObject();
                bullet.gameObject.SetActive(false);
            }
        }


        //objet retourne à la pool
        private void OnReleaseToPool(bullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        //affiche object de la pool
        private void OnGetFromPool(bullet bullet)
        {
            bullet.gameObject.SetActive(true);
        }

        //Détruit quand le max number est dépassé 
        private void OnDestroyToPooledObject(bullet bullet)
        {
            Destroy(bullet.gameObject);
        }

        //crée un objet et l'assigne à la pool
        private bullet CreateObject()
        {
            bullet bulletInstance = Instantiate(_bulletPrefab);
            bulletInstance.ObjectPool = _bulletPool;
            return bulletInstance;
        }


        public void FireBullet(Transform shooter, Vector3 targetPos, float velocity, LayerMask shooterOrigin, Color bulletColor)
        {
            if (_bulletPool != null)
            {
                bullet bullet = _bulletPool.Get();
                if (bullet == null) return;
                bullet.transform.SetPositionAndRotation(shooter.position, Quaternion.identity);
                bullet.BulletSRColor = bulletColor;
                bullet.gameObject.layer = shooterOrigin;
                bullet.BulletDir = targetPos;
                bullet.Speed = velocity;
                //targetPos.Normalize();
               //bullet.Rb.AddForce(targetPos * velocity, ForceMode2D.Impulse);
            }
        }
    }
}
