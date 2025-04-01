using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AS
{
    public class ExplosiveProjectile : ProjectileDamage
    {
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private Vector3 explosionPoint;
        [SerializeField] private float explosionRadius = 5;
        [SerializeField] private float explosionForce;
        [SerializeField] private LayerMask targetLayer;

        [SerializeField] private bool exploded = false;

        [SerializeField] private float explosionDuration;

        public override void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == shooter) return;
            // if (other.gameObject.tag == "Projectile") return;

            ExplosionDamage();
            Debug.Log(other.gameObject.name);
        }

        public void ExplosionDamage()
        {
            if (exploded) return;

            exploded = true;
            explosionPoint = transform.position;

            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            GetComponent<Rigidbody>().isKinematic = true;

            Collider[] targetsHit = Physics.OverlapSphere(explosionPoint, explosionRadius);

            foreach (var target in targetsHit)
            {
                if (target.gameObject == shooter) return;

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
            if (explosionEffect != null)
            {
                GameObject explosion = Instantiate(explosionEffect, explosionPoint, Quaternion.identity);
                //explosion.transform.localScale = Vector3.zero;

                float elapsedTime = 0f;

                while (elapsedTime < explosionDuration)
                {
                    elapsedTime += Time.deltaTime;

                    float currentRadius = Mathf.Lerp(0, explosionRadius, elapsedTime / explosionDuration);
                    //explosion.transform.localScale = new Vector3(currentRadius * 2, currentRadius * 2, currentRadius * 2);
                    yield return null;
                }

                //Destroy(explosion, 2f);

            }
            ObjectPool.SharedInstance.ReturnToPool("BombPool", gameObject);
        }
    }
}

