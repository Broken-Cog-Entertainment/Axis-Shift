using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AS
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool SharedInstance;
        public List<GameObject> pooledObjects;
        public GameObject objectToPool;
        public int totalAmountToPool;

        private void Awake()
        {
            SharedInstance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            pooledObjects = new List<GameObject>();
            GameObject obj;
            for(int i = 0; i < totalAmountToPool; i++)
            {
                obj = Instantiate(objectToPool);
                obj.SetActive(true);
                pooledObjects.Add(obj);
            }
        }

        public GameObject GetPooledObject()
        {
            for(int i = 0; i < totalAmountToPool; i++)
            {
                if (!pooledObjects[i].activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }
            return null;
        }
    }
}
