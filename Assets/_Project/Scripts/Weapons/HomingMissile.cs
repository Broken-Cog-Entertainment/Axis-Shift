using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AS.Weapons
{
    public class HomingMissile : MonoBehaviour
    {
        public Transform target;
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
                Collider[] targetsHit = Physics.OverlapSphere(transform.position, homingRange, enemyLayer);

                if (targetsHit.Length > 0)
                {
                    foundTarget = true;
                    Collider firstHit = targetsHit[0];
                    target = firstHit.gameObject.transform;
                }
            }            
        }

        private void FixedUpdate()
        {
            if (target == null)
            {
                myRB.linearVelocity = transform.forward * speed;
            }
            else
            {
                Vector3 direction = (target.position - transform.position).normalized;

                Quaternion lookRotation = Quaternion.LookRotation(direction);
                myRB.rotation = Quaternion.Slerp(myRB.rotation, lookRotation, rotateSpeed * Time.deltaTime);

                myRB.linearVelocity = transform.forward * speed;
            }           
        }

        public void OnCollisionEnter(Collision other)
        {
            if (other.gameObject == target.gameObject)
            {
                other.gameObject.TryGetComponent(out IDamageable hit);
                hit.TakeDamage(damageAmount);
                Debug.Log("Hit " + other.gameObject.name);
                Destroy(this.gameObject);
            }
        }
    }
}

