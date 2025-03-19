using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    public float GetHealth();

    void TakeDamage(float damage);
}
