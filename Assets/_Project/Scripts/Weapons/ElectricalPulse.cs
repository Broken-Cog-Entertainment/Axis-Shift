using UnityEngine;
using System.Collections;
using AS.Player;

namespace AS
{
    public class ElectricalPulse : MonoBehaviour
    {
        public GameObject player;

        public float pulseDuration;
        public float pulseRadius;
        public float currentPulseRate;
        public float minPulseRate;
        public float maxPulseRate;
        public float nextPulse = 0f;
        public float disabledMovementTime;

        public GameObject pulseEffect;

        private void Start()
        {
            RandomizePulseRate();
            pulseEffect.SetActive(false);
        }

        public void Update()
        {
            //nextPulse = 0f;
            nextPulse += Time.deltaTime;

            if(nextPulse >= currentPulseRate)
            {
                StartCoroutine(Pulse());
                nextPulse = 0f;
            }
        }

        IEnumerator Pulse()
        {
            float timeElapsed = 0f;
            pulseEffect.SetActive(true);
            while(timeElapsed < pulseDuration)
            {
                timeElapsed += Time.deltaTime;
                float currentRadius = Mathf.Lerp(0, pulseRadius, timeElapsed / pulseDuration);
                pulseEffect.transform.localScale = new Vector3(currentRadius, currentRadius, currentRadius);

                Collider[] targetsHit = Physics.OverlapSphere(transform.position, pulseRadius);
                foreach (Collider hit in targetsHit)
                 {
                    if (hit.gameObject == player)
                    {
                        player.GetComponent<PlayerController>().enabled = false;
                    }

                }

                yield return null;
            }

            yield return new WaitForSeconds(disabledMovementTime);
            pulseEffect.SetActive(false);

            if (!player.GetComponent<PlayerController>().enabled)
            {
                player.GetComponent<PlayerController>().enabled = true;
            }

        }

        public void RandomizePulseRate()
        {
            currentPulseRate = Random.Range(minPulseRate, maxPulseRate);
        }
    }
}
