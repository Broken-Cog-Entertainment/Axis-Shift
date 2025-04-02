using System;
using UnityEngine;

namespace AS.Weapons
{
    public class Bullet : MonoBehaviour
    {
        public GameObject shooter;
        public float damageAmount;

        public float speed = 10f;
        public float destroyWhenRange = 1f;
        
        public Vector3 target;

        private string playerBulletPool = "BulletPool";
        private string enemyBulletPool = "EnemyBulletPool";

        private void Update()
        {
            var direction = (target - transform.position).normalized;

            transform.position += direction * (speed * Time.deltaTime);
            transform.LookAt(target);

            if (Vector3.Distance(target, transform.position) < destroyWhenRange)
            {
                ReturnToPool();
            }
        }

        public virtual void OnCollisionEnter(Collision other)
        {
            if (other.gameObject == shooter) return;
            //if (other.gameObject.CompareTag("Water")) return;

            if (other.gameObject.TryGetComponent(out IDamageable hit))
            {
                hit.TakeDamage(damageAmount);
                Debug.Log("Hit " + other.gameObject.name);
                ReturnToPool();
            }

            else
            {
                if(shooter.gameObject.tag == "Player")
                {
                    if (other.gameObject.TryGetComponent(out ActivateSwitch switchHit))
                    {
                        switchHit.Activate();
                        ReturnToPool();
                    }
                }
                else
                {
                    Debug.Log("Switch was shot by enemy, ignore activation");
                }
            }
        }

        void ReturnToPool()
        {
            if(shooter == null)
            {
                ObjectPool.SharedInstance.ReturnToPool(enemyBulletPool, gameObject);
            }
            else
            {
                string poolName = shooter.CompareTag("Player") ? playerBulletPool : enemyBulletPool;
                ObjectPool.SharedInstance.ReturnToPool(poolName, gameObject);
            }
        }
    }
}