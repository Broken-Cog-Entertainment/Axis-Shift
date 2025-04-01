using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float health { get; set; }
    public float[] currentHealth;

    [SerializeField] private Slider[] healthSliders;
    [SerializeField] bool[] depleted;

    public bool dead;

    public float regenDelay;
    public float regenAmount;
    private Coroutine regenCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = new float[healthSliders.Length];
        depleted = new bool[healthSliders.Length];

        for(int i = 0; i < healthSliders.Length; i++)
        {
            currentHealth[i] = health;
            depleted[i] = false;
            healthSliders[i].maxValue = health;
            healthSliders[i].value = health;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth[0] <= 0)
        {
            dead = true;
            //this.GetComponent<Transformation>().enabled = false;

            foreach (Behaviour behaviour in this.GetComponents<Behaviour>())
            {
                behaviour.enabled = false;
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if(regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }

        for(int i = 2; i >= 0; i--)
        {
            if (!depleted[i])
            {
                currentHealth[i] -= damageAmount;
                if (currentHealth[i] <= 0)
                {
                    currentHealth[i] = 0;
                    depleted[i] = true;
                }

                UpdateHealthBar();
                regenCoroutine = StartCoroutine(RegenHealth());
                return;
            }
        }     
    }

    IEnumerator RegenHealth()
    {
        yield return new WaitForSeconds(regenDelay);

        while (true)
        {
            bool anyRegen = false;
            for (int i = 2; i >= 0; i--)
            {
                if (!depleted[i] && currentHealth[i] < health)
                {
                    currentHealth[i] += regenAmount * Time.deltaTime;

                    if (currentHealth[i] >= health)
                    {
                        currentHealth[i] = health;
                    }

                    anyRegen = true;
                }
            }
            if (!anyRegen) break;
            UpdateHealthBar();
            yield return null;

        }
    }

    void UpdateHealthBar()
    {
        for(int i = 0; i < healthSliders.Length; i++)
        {
            healthSliders[i].value = currentHealth[i];
            healthSliders[i].interactable = !depleted[i];
        }
    }

    public float GetHealth()
    {
        return health;
    }
}
