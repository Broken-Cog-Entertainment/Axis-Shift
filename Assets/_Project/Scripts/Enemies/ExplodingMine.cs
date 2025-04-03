using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingMine : MonoBehaviour
{
    public float explosionPower;
    public float damageAmount;

    public GameObject explosionParticle;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameObject particle = Instantiate(explosionParticle, this.transform.position, Quaternion.identity) as GameObject;
            particle.GetComponent<ParticleSystem>().Play();

            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            
            if(rb != null)
            {
                rb.AddExplosionForce(explosionPower, this.transform.position, 5.0f, 3.0f);
            }

            other.GetComponentInParent<PlayerHealth>().TakeDamage(damageAmount);

            Destroy(this.gameObject);
        }
    }
}
