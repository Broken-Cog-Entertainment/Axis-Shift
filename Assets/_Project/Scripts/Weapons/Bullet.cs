using System;
using UnityEngine;

namespace AS.Weapons
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 10f;
        public float destroyWhenRange = 0.5f;
        
        public Vector3 target;

        private void Update()
        {
            var direction = (target - transform.position).normalized;

            transform.position += direction * (speed * Time.deltaTime);

            if (Vector3.Distance(target, transform.position) < destroyWhenRange)
            {
                Destroy(gameObject);
            }
        }
    }
}