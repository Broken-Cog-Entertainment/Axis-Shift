using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public GameObject shooter;
    public float damageAmount;

    public virtual void Update()
    {
        Destroy(this.gameObject, 3f);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == shooter) return;
        if (other.CompareTag("Water")) return;

        if(other.TryGetComponent(out IDamageable hit))
        {
            hit.TakeDamage(damageAmount);
            Debug.Log("Hit " + other.name);
        }

        else
        {
            ActivateSwitch activateSwitch = other.GetComponent<ActivateSwitch>();
            if (activateSwitch != null)
            {
                activateSwitch.Activate();
            }
        }
    }
}
