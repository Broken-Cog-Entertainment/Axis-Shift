using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AS
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool SharedInstance;
        public List<GameObject> pooledObjects;

        [Header("Objects To Pool")]
        public GameObject bulletPrefab;
        public GameObject enemyBulletPrefab;
        public GameObject bombPrefab;
        public GameObject explosionPrefab;
        public GameObject homingMissilePrefab;

        public int totalAmountToPool;

        private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

        private void Awake()
        {
            SharedInstance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            CreatePool("BulletPool", bulletPrefab, 50);
            CreatePool("EnemyBulletPool", enemyBulletPrefab, 25);
            CreatePool("BombPool", bombPrefab, 25);
            CreatePool("ExplosionPool", explosionPrefab, 25);
            CreatePool("HomingMissilePool", homingMissilePrefab, 10);
        }

        public void CreatePool(string poolName, GameObject prefab, int initialSize)
        {
            if (!pools.ContainsKey(poolName))
            {
                Queue<GameObject> pool = new Queue<GameObject>();
                for (int i = 0; i < initialSize; i++)
                {
                    GameObject obj = Instantiate(prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(this.transform);
                    pool.Enqueue(obj);
                }
                pools.Add(poolName, pool);
            }
        }

        public GameObject GetPooledObject(string poolName)
        {
            if (!pools.ContainsKey(poolName))
            {
                Debug.LogError($"Pool {poolName} doesn't exist.");
                return null;
            }
            if (pools[poolName].Count == 0)
            {
                return null;
            }
            GameObject obj = pools[poolName].Dequeue();

            if(obj == null)
            {
                Debug.LogError($"Object is missing from {poolName}, it may have been destroyed.");
                return null;
            }
                obj.SetActive(true);
                return obj;
        }

        public void ReturnToPool(string poolName, GameObject obj)
        {
            if(obj == null)
            {
                Debug.LogWarning($"Attempting to return a null object to {poolName}.");
            return;
            }
            if (pools.ContainsKey(poolName))
            {
                obj.SetActive(false);

                if (!pools[poolName].Contains(obj))
                {
                    pools[poolName].Enqueue(obj);
                }
                else
                {
                    Debug.LogWarning($"Object {obj.name} is already in the pool {poolName}");
                }
            }
            else
            {
                Debug.LogError($"Pool {poolName} doesn't exist.");
            }
        }
    }
}
