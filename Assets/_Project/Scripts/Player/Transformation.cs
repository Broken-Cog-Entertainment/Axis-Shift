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
        public SubmarineController submarineController;

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

        void Awake()
        {
            myRB = GetComponent<Rigidbody>();
            StartCoroutine(TransformVehicle(TransformationType.Tank));
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
            // if (newTransform == currentForm)
            //  {
            //      Debug.Log("Already in this vehicle form!");
            //       yield break;
            //  }

            currentPosition = transform.position;
            currentRotation = transform.rotation;

            yield return new WaitForEndOfFrame();

           // storedVelocity = myRB.linearVelocity;
          //  storedAngularVelocity = myRB.angularVelocity;

            tankController.enabled = false;
            spaceshipController.enabled = false;
            submarineController.enabled = false;

            tankCam.gameObject.SetActive(false);
            spaceshipCam.gameObject.SetActive(false);
            var brain = Camera.main.GetComponent<CinemachineBrain>();

            tankForm.SetActive(false);
            spaceshipForm.SetActive(false);
            submarineForm.SetActive(false);

            switch (newTransform)
            {
                case TransformationType.Tank:

                    tankForm.SetActive(true);

                    tankController.enabled = true;
                    tankCam.gameObject.SetActive(true);
                    brain.UpdateMethod = CinemachineBrain.UpdateMethods.FixedUpdate;


                    // this.GetComponent<RadarPulse>().enabled = false;
                    // this.GetComponent<HomingMissileLauncher>().enabled = false;
                    // this.GetComponent<PlayerShoot>().enabled = true;

                    myRB.linearDamping = 1;

                    break;

                case TransformationType.Spaceship:

                    if (!spaceshipUnlocked) yield break;

                    spaceshipController.enabled = true;
                    spaceshipCam.gameObject.SetActive(true);
                    brain.UpdateMethod = CinemachineBrain.UpdateMethods.LateUpdate;
                    spaceshipForm.SetActive(true);

                    Debug.Log("Enabling spaceship controller.");

                    //  this.GetComponent<RadarPulse>().enabled = false;
                    //  this.GetComponent<HomingMissileLauncher>().enabled = false;
                    //  this.GetComponent<PlayerShoot>().enabled = true;
               
                    break;

                case TransformationType.Submarine:

                    if (!submarineUnlocked) yield break;

                    submarineController.enabled = true;
                    spaceshipCam.gameObject.SetActive(true);
                    brain.UpdateMethod = CinemachineBrain.UpdateMethods.LateUpdate;
                    submarineForm.SetActive(true);
                    //  this.GetComponent<RadarPulse>().enabled = true;
                    //  this.GetComponent<HomingMissileLauncher>().enabled = true;
                    //  this.GetComponent<PlayerShoot>().enabled = false;
                    
                    break;
            }

            yield return null;

            transform.position = currentPosition;
            transform.rotation = currentRotation;

            currentForm = newTransform;
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

