using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AS.Weapons
{
    public class LaunchHomingMissile : MonoBehaviour
    {
        [SerializeField] private GameObject homingMissilePrefab;
        [SerializeField] private Transform[] launchPos;

        [SerializeField] private GameObject target;

        [SerializeField] private float launchRate;
        [SerializeField] private float timeUntilNextLaunch = 0f;

        private void Start()
        {
            target = GameObject.FindWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        { 
            timeUntilNextLaunch += Time.deltaTime;

            if(timeUntilNextLaunch >= launchRate)
            {
                StartCoroutine(Launching());
                timeUntilNextLaunch = 0f;
            }
        }

        IEnumerator Launching()
        {
            for (int i = 0; i < launchPos.Length; i++)
            {
                yield return new WaitForSeconds(0.3f);
                GameObject missile = ObjectPool.SharedInstance.GetPooledObject("HomingMissilePool");

                if (missile != null)
                {
                    Debug.Log("Launched missile!");
                    missile.transform.position = launchPos[i].position;
                    missile.SetActive(true);
                    missile.GetComponent<HomingMissile>().target = target.transform.position;
                    missile.GetComponent<HomingMissile>().shooter = this.gameObject;
                }
            }

        }
    }
}
