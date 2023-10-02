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
        //essai de rendre l'object pool general : WIP
        //private object objectConcerned;

        [SerializeField]private bullet bulletPrefab;

        private IObjectPool<bullet> bulletPool;

        [SerializeField] private bool collectionCheck = true;

        [Tooltip("Cooldown avant spawn de l'objet")]
        [SerializeField] private float bulletMaxCD = .1f;
        private float bulletCD;

        //TO DO :
        //Mettre plusieurs spawn points
        //private List<Transform> bulletsSpawnPoints = new List<Transform>();
        //[SerializeField] private Transform bulletSpawnPoint;

        [SerializeField] private int objectDefaultCapacity = 20;
        [SerializeField] private int objectMaxSize = 50;


        // Start is called before the first frame update
        void Awake()
        {
            bulletPool = new ObjectPool<bullet>(CreateObject, OnGetFromPool, OnReleaseToPool, OnDestroyToPooledObject, collectionCheck, objectDefaultCapacity, objectMaxSize) ;
        }

        private void Start()
        {
            bulletCD = bulletMaxCD;
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
            bullet bulletInstance = Instantiate(bulletPrefab);
            bulletInstance.ObjectPool = bulletPool;
            return bulletInstance;
        }

        private void FixedUpdate()
        {
            /*
            //tire quand cd finit
            if (bulletCD < 0f && bulletPool != null)
            {
                bullet bullet = bulletPool.Get();
                if (bullet == null) return;
                bullet.transform.SetPositionAndRotation(bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            }
            //réduit le cooldown
            else if(bulletCD > 0f) bulletCD -= Time.deltaTime;*/
        }

        public void FireBullet(Transform spawnPoint, float velocity)
        {
            if (bulletPool != null)
            {
                bullet bullet = bulletPool.Get();
                if (bullet == null) return;
                bullet.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
                bullet.Rb.AddForce(bullet.transform.forward * velocity, ForceMode2D.Impulse);
            }
        }
    }
}
