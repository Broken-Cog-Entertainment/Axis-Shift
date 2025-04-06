using System.Collections;
using UnityEngine;

namespace AS.Player
{
    public class ActivateTransformationAbility : MonoBehaviour
    {
        public GameObject spaceshipUnlock;
        public GameObject submarineUnlock;

      //  bool canActivate = false;
     //   public Transform platformPos;

     //   public int transformIndex = 0;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject == spaceshipUnlock)
            {
                if (other.gameObject == spaceshipUnlock)
                {
                    this.GetComponent<Transformation>().UnlockSpaceship();
                    other.GetComponent<Collider>().enabled = false;
                    Debug.Log("Unlocked Spaceship form!");
                }
                if(other.gameObject == submarineUnlock)
                {
                    this.GetComponent<Transformation>().UnlockSubmarine();
                    other.GetComponent<Collider>().enabled = false;
                    Debug.Log("Unlocked Submarine form!");
                }
            }
        }

      /*  IEnumerator EnableTransformation()
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

            if(transformIndex == 0)
            {
                player.GetComponent<Transformation>().UnlockSpaceship();
                transformIndex++;
                Debug.Log("Unlocked Spaceship!");
            }
            if(transformIndex == 1)
            {
                player.GetComponent<Transformation>().UnlockSubmarine();
                Debug.Log("Unlocked Submarine!");
            }        
        }*/
    }
}
