using System.Collections;
using UnityEngine;

namespace AS
{
    public class ActivateTransformationAbility : MonoBehaviour
    {
        public GameObject player;
        bool canActivate = false;
        public Transform platformPos;

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
        }
        private void OnTriggerStay(Collider other)
        {
            if(other.gameObject == player)
            {
                canActivate = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == player)
            {
                canActivate = false;
            }
        }

        private void Update()
        {
            if (canActivate)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(EnableTransformation());
                }
            }
        }

        IEnumerator EnableTransformation()
        {
            while (player.transform.position != platformPos.position)
            {
                Vector3 newPos = Vector3.MoveTowards(player.transform.position, platformPos.position, 2f * Time.deltaTime);
                player.transform.position = newPos;

                Vector3 direction = (platformPos.position - platformPos.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                player.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2f * Time.deltaTime);

                yield return null;
            }

            yield return new WaitForSeconds(2f);

            player.GetComponent<Transformation>().canTransformIntoSpaceship = true;
        }
    }
}
