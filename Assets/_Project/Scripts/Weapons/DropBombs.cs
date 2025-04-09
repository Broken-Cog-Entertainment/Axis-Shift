using AS.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AS
{
    public class DropBombs : MonoBehaviour
    {
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private Transform bombDropPos;
        [SerializeField] private float bombDropRate;
        [SerializeField] private int bombDropBurst;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(Bombing());
        }

        IEnumerator Bombing()
        {
            while (true)
            {
                for (int i = 0; i < bombDropBurst; i++)
                {
                    DropBomb();
                    yield return new WaitForSeconds(0.3f);
                }

                yield return new WaitForSeconds(bombDropRate);
            }

        }

        void DropBomb()
        {
            GameObject bomb = Instantiate(bombPrefab, bombDropPos.position, Quaternion.identity);
            bomb.GetComponent<ExplosiveProjectile>().shooter = this.gameObject;
        }
    }
}
