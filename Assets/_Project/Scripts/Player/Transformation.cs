using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

namespace AS.Player
{
    public enum TransformationType { Tank, Spaceship, Submarine }
    public class Transformation : MonoBehaviour
    {
        public GameObject tankForm;
        public GameObject spaceshipForm;
        public GameObject submarineForm;

        public Rigidbody myRB;

        public TankController tankController;
        public PlayerController spaceshipController;
        // public SubmarineMovement submarineController;

        public TransformationType currentForm;

        [SerializeField] private Vector3 storedVelocity;
        [SerializeField] private Vector3 storedAngularVelocity;

        [SerializeField] private Vector3 currentPosition;
        [SerializeField] private Quaternion currentRotation;

        public bool spaceshipUnlocked = false;
        public bool submarineUnlocked = false;

        [SerializeField] private bool inWater = false;

        [SerializeField] private CinemachineCamera tankCam;
        [SerializeField] private CinemachineCamera spaceshipCam;

        //  [SerializeField] private Animator myAnim;
        public float targetState = 0f;
        public float transitionTime = 0.5f;

        // [SerializeField] private WeaponHolder weaponHolder;

        void Start()
        {
            myRB = GetComponent<Rigidbody>();
            TransformVehicle(TransformationType.Tank);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(TransformVehicle(TransformationType.Tank));
                Debug.Log("Transformed into Tank.");

                if (currentForm == TransformationType.Spaceship)
                {
                  //  myAnim.SetTrigger("SpaceshipToTank");
                }
                if (currentForm == TransformationType.Submarine)
                {
                 //   myAnim.SetTrigger("SubmarineToTank");
                }

                //weaponHolder.SetActiveWeapon(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && spaceshipUnlocked)
            {
                StartCoroutine(TransformVehicle(TransformationType.Spaceship));
                Debug.Log("Transformed into Spaceship.");

                if (currentForm == TransformationType.Tank)
                {
                 //   myAnim.SetTrigger("TankToSpaceship");
                }
                if (currentForm == TransformationType.Submarine)
                {
                  //  myAnim.SetTrigger("SubmarineToSpaceship");
                }

                //weaponHolder.SetActiveWeapon(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && submarineUnlocked)
            {
                StartCoroutine(TransformVehicle(TransformationType.Submarine));
                Debug.Log("Transformed into Submarine.");

                if (currentForm == TransformationType.Tank)
                {
                  //  myAnim.SetTrigger("TankToSubmarine");
                }
                if (currentForm == TransformationType.Spaceship)
                {
                  //  myAnim.SetTrigger("SpaceshipToSubmarine");
                }

                //weaponHolder.SetActiveWeapon(3);
            }
        }

        IEnumerator TransformVehicle(TransformationType newTransform)
        {
            if (newTransform == currentForm)
            {
                Debug.Log("Already in this vehicle form!");
                yield return null;
            }

            yield return new WaitForSeconds(0.4f);

            storedVelocity = myRB.linearVelocity;
            storedAngularVelocity = myRB.angularVelocity;

            tankController.enabled = false;
            spaceshipController.enabled = false;

            tankCam.gameObject.SetActive(false);
            spaceshipCam.gameObject.SetActive(false);
            //   submarineController.enabled = false;

            tankForm.SetActive(false);
            spaceshipForm.SetActive(false);
         //   submarineForm.SetActive(false);

            switch (newTransform)
            {
                case TransformationType.Tank:

                    tankController.enabled = true;
                    tankCam.gameObject.SetActive(true);
                    tankForm.SetActive(true);

                    // this.GetComponent<RadarPulse>().enabled = false;
                    // this.GetComponent<HomingMissileLauncher>().enabled = false;
                    // this.GetComponent<PlayerShoot>().enabled = true;

                    myRB.linearDamping = 1;

                    break;

                case TransformationType.Spaceship:

                    spaceshipController.enabled = true;
                    spaceshipCam.gameObject.SetActive(true);
                    spaceshipForm.SetActive(true);

                    //  this.GetComponent<RadarPulse>().enabled = false;
                    //  this.GetComponent<HomingMissileLauncher>().enabled = false;
                    //  this.GetComponent<PlayerShoot>().enabled = true;
                    if (!spaceshipUnlocked) yield break;
                    break;

                case TransformationType.Submarine:

                    //    submarineController.enabled = true;
                    //  this.GetComponent<RadarPulse>().enabled = true;
                    //  this.GetComponent<HomingMissileLauncher>().enabled = true;
                    //  this.GetComponent<PlayerShoot>().enabled = false;
                    if (!submarineUnlocked) yield break;
                    break;
            }

            yield return null;

            myRB.linearVelocity = storedVelocity;
            myRB.angularVelocity = storedAngularVelocity;

            currentForm = newTransform;
        }

        IEnumerator RestoreVelocity()
        {
            float duration = 0.5f;
            float timeElapsed = 0f;

            Vector3 initialVelocity = myRB.linearVelocity;
            Vector3 initialAngularVelocity = myRB.angularVelocity;

            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                float time = timeElapsed / duration;
                myRB.linearVelocity = Vector3.Lerp(initialVelocity, storedVelocity, time);
                myRB.angularVelocity = Vector3.Lerp(initialAngularVelocity, storedAngularVelocity, time);
                yield return null;
            }

            myRB.linearVelocity = storedVelocity;
            myRB.angularVelocity = storedAngularVelocity;
        }

        public void UnlockSpaceship()
        {
            spaceshipUnlocked = true;
        }

        public void UnlockSubmarine()
        {
            submarineUnlocked = true;
        }
    }
}

