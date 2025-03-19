using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float health { get; set; }
    private float currentHealth;

    public GameObject sliderPrefab;
    public GameObject enemyCanvas;

    public Slider healthSlider;

    public float maxHealthBarVisibleTime;
    private float currentHealthBarVisibleTime;

    private void Start()
    {
        GameObject slider = Instantiate(sliderPrefab) as GameObject;
        enemyCanvas = GameObject.FindWithTag("EnemyCanvas");
        slider.transform.SetParent(enemyCanvas.transform);
        slider.GetComponent<EnemyHealthBar>().enemyPos = this.transform;
        healthSlider = slider.GetComponent<Slider>();

        currentHealth = health;
        UpdateHealthBar();
        healthSlider.gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();
        currentHealthBarVisibleTime = maxHealthBarVisibleTime;

        this.GetComponent<EnemyBehaviour>().alerted = true;
    }

    private void Update()
    {
        if(currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }

        currentHealthBarVisibleTime -= Time.deltaTime;

        if(currentHealthBarVisibleTime > 0)
        {
            healthSlider.gameObject.SetActive(true);
        }
        else
        {
            healthSlider.gameObject.SetActive(false);
        }
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
