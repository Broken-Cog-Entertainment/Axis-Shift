using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AS.Weapons
{
    public class HomingMissile : MonoBehaviour
    {
        public Transform target;
        public Transform launchPos;
        public float speed;
        public float rotateSpeed;
        public float homingRange;
        public float missileLifetime;
        public float damageAmount;

        public bool foundTarget = false;

        private Rigidbody myRB;

        public LayerMask enemyLayer;

        private void Start()
        {
            myRB = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!foundTarget && target == null)
            {
                FindNearestEnemy();
            }            
        }

        private void FixedUpdate()
        {
            if (target == null)
            {
                myRB.linearVelocity = launchPos.forward * speed;
            }
            else
            {
                Vector3 desiredDirection = (target.position - transform.position).normalized;

                Quaternion lookRotation = Quaternion.LookRotation(desiredDirection);
                myRB.rotation = Quaternion.RotateTowards(myRB.rotation, lookRotation, rotateSpeed * Time.fixedDeltaTime * 100f);

                myRB.linearVelocity = transform.forward * speed;
            }         
            
            if(target != null && Vector3.Distance(transform.position, target.position) < 1.5f)
            {
                Explode();
            }
        }

        public void OnCollisionEnter(Collision other)
        {
            if (target != null && other.gameObject == target.gameObject)
            {
                other.gameObject.TryGetComponent(out IDamageable hit);
                hit.TakeDamage(damageAmount);
                Debug.Log("Hit " + other.gameObject.name);
                Destroy(this.gameObject);
            }
        }

        void FindNearestEnemy()
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, homingRange, enemyLayer);

            if (enemies.Length > 0)
            {
                Collider closest = enemies.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).First();

                target = closest.transform;
                foundTarget = true;
            }
        }

        void Explode()
        {
            if(target.TryGetComponent(out IDamageable hit))
            {
                hit.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
    }
}

