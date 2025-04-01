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
            CreatePool("BulletPool", bulletPrefab, 25);
            CreatePool("EnemyBulletPool", enemyBulletPrefab, 25);
            CreatePool("BombPool", bombPrefab, 25);
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
            if(pools.ContainsKey(poolName) && pools[poolName].Count > 0)
            {
                GameObject obj = pools[poolName].Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                return null;
            }
        }

        public void ReturnToPool(string poolName, GameObject obj)
        {
            if (pools.ContainsKey(poolName))
            {
                obj.SetActive(false);
                pools[poolName].Enqueue(obj);
            }
        }
    }
}
