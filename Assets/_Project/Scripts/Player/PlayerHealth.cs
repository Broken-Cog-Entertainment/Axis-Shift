using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float health { get; set; }
    public float currentHealth;

    [SerializeField] private Slider healthSlider;

    public bool dead;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0)
        {
            dead = true;
           // this.GetComponent<Transformation>().enabled = false;

            foreach (Behaviour behaviour in this.GetComponents<Behaviour>())
            {
                behaviour.enabled = false;
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthSlider.value = currentHealth / health;
    }

    public float GetHealth()
    {
        return health;
    }
}
