using System;
using UnityEngine;

namespace AS.Weapons
{
    public class Bullet : MonoBehaviour
    {
        public GameObject shooter;
        public float damageAmount;

        public float speed = 10f;
        public float destroyWhenRange = 0.5f;
        
        public Vector3 target;

        private void Update()
        {
            var direction = (target - transform.position).normalized;

            transform.position += direction * (speed * Time.deltaTime);
            transform.LookAt(target);

            if (Vector3.Distance(target, transform.position) < destroyWhenRange)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject == shooter) return;
            //if (other.gameObject.CompareTag("Water")) return;

            if (other.gameObject.TryGetComponent(out IDamageable hit))
            {
                hit.TakeDamage(damageAmount);
                Debug.Log("Hit " + other.gameObject.name);
            }

            else
            {
                ActivateSwitch activateSwitch = other.gameObject.GetComponent<ActivateSwitch>();
                if (activateSwitch != null)
                {
                    activateSwitch.Activate();
                }
            }
        }
    }
}