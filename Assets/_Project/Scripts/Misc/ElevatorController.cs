using System.Collections;
using UnityEngine;

namespace AS
{
    public class ElevatorController : MonoBehaviour
    {
        public Animator doorAnim;
        public Animator elevatorAnim;

        bool activated = false;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                activated = true;
            }
        }

        private void Update()
        {
            if (activated)
            {
                StartCoroutine(ActivateElevator());
            }
        }

        IEnumerator ActivateElevator()
        {
            activated = false;
            doorAnim.SetTrigger("Close");
            yield return new WaitForSeconds(2f);
            elevatorAnim.SetTrigger("Lift");
        }
    }
}
