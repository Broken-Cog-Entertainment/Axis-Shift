using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AS.Weapons
{
    public class ExplosiveProjectile : Bullet
    {
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private Vector3 explosionPoint;
        [SerializeField] private float explosionRadius = 5;
        [SerializeField] private float explosionForce;
        [SerializeField] private LayerMask targetLayer;

        [SerializeField] private bool exploded = false;

        [SerializeField] private float explosionDuration;


        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == shooter) return;
            // if (other.gameObject.tag == "Projectile") return;

            StartCoroutine(ExplosionDuration());
            Debug.Log(other.gameObject.name);
        }
        IEnumerator ExplosionDuration()
        {
            ExplosionDamage();

            yield return new WaitForSeconds(explosionDuration);

            exploded = false;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            ObjectPool.SharedInstance.ReturnToPool("BombPool", gameObject);
        }

        public void ExplosionDamage()
        {
            if (exploded) return;

            exploded = true;
            explosionPoint = transform.position;

        //    GetComponent<MeshRenderer>().enabled = false;
        //    GetComponent<Collider>().enabled = false;
        //    GetComponent<Rigidbody>().isKinematic = true;

            Collider[] targetsHit = Physics.OverlapSphere(explosionPoint, explosionRadius);

            foreach (var target in targetsHit)
            {
                if (target.gameObject == shooter) continue;

                if (target.TryGetComponent(out IDamageable hit))
                {
                    hit.TakeDamage(damageAmount);
                }

                Rigidbody rb = target.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (target.gameObject.tag == "Projectile")
                    {
                        Debug.Log("Collided with other projectile, ignoring force.");
                    }
                    else
                    {
                        rb.AddExplosionForce(explosionForce, explosionPoint, explosionRadius);
                    }
                }
            }

            StartCoroutine(ExplosionEffect());
        }

        IEnumerator ExplosionEffect()
        {
            GameObject explosion = ObjectPool.SharedInstance.GetPooledObject("ExplosionPool");
            explosion.transform.position = this.transform.position;
            explosion.GetComponent<ParticleSystem>().Play();

            if (explosion != null)
            {
                float elapsedTime = 0f;

                while (elapsedTime < explosionDuration)
                {
                    elapsedTime += Time.deltaTime;

                    float currentRadius = Mathf.Lerp(0, explosionRadius, elapsedTime / explosionDuration);
                    yield return null;
                }

                ObjectPool.SharedInstance.ReturnToPool("ExplosionPool", explosion);
            }
        }
    }
}

