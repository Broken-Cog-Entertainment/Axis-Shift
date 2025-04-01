using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AS.Weapons
{
    public class HomingMissile : Bullet
    {
        public float rotateSpeed;
        public float missileLifetime;

        private Rigidbody myRB;

        private void Start()
        {
            myRB = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (target == null) return;

            Vector3 direction = (target - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            myRB.rotation = Quaternion.Slerp(myRB.rotation, lookRotation, rotateSpeed * Time.deltaTime);

            myRB.linearVelocity = transform.forward * speed;
        }

        public override void OnCollisionEnter(Collision other)
        {
            if (other.gameObject == shooter) return;
            //if (other.gameObject.CompareTag("Water")) return;

            if (other.gameObject.TryGetComponent(out IDamageable hit))
            {
                hit.TakeDamage(damageAmount);
                Debug.Log("Hit " + other.gameObject.name);
                ObjectPool.SharedInstance.ReturnToPool("HomingMissilePool", gameObject);
            }
        }
    }
}

