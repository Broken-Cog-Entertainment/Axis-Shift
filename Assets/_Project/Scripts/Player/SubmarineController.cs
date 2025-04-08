using AS.Utils;
using AS.Utils.MathUtils;
using AS.Weapons;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AS.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class SubmarineController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public AnimationCurve thrustByVelocityDot;
        public AnimationCurve manualThrustControl;
        public AnimationCurve aimingThrustControl;
        public float maxThrust = 10f;
        public float maxSpeed = 20f;
        public float minDistanceToEnemy = 20f;

        [Header("Ground Lift Force")]
        public float groundLiftCheckDistance;
        public float groundLiftMaxForce;
        public LayerMask applyGroundForceLayerMask;

        [Header("Rotation Controls")]
        [Range(20, 90)] public float maxBankAngle = 30f;
        [Range(10, 90)] public float maxFreeLookBankAngle = 15f;
        [Range(15, 90)] public float minPitchAngle;
        [Range(15, 90)] public float maxPitchAngle;

        [Header("Weapon Controls")]
        public Image hudReticle;

        public bool shootOnlyWhileAiming = true;
        public float maxRange;
        public Vector3 gunOffset;
        public Bullet bulletPrefab;
        public float fireRate;

        [Header("Cameras")]
        public CinemachineOrbitalFollow freeLookCamera;

        public float pitch;
        public float yaw;
        public float roll;

        public float manualRoll;

        public ThreeAxisControl control;
        private AnimationCurve _activeThrustControl;

        private bool _isAiming;
        private bool _isBoosted;
        private float _lastFiredTimer;

        private bool _lastUsedGunLeft;
        private Camera _mainCamera;

        private Vector3 _movement;
        private Vector2 _pitchYawDelta;

        private Vector2 _rawPitchYawDelta;

        private Rigidbody _rb;

        public EnterWater waterCheck;

        private float FireDelay => 1f / fireRate;

        bool isLockingOn;
        Quaternion lockOnRotation;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _mainCamera = Camera.main;
            _rb.freezeRotation = true;

            hudReticle.gameObject.SetActive(true);
            _activeThrustControl = manualThrustControl;
        }

        private void Update()
        {
            UpdateTimers(Time.deltaTime);

            #region Manual Roll (todo: refactor)
   
            var manualRollControl = 0;

            if (Input.GetKey(KeyCode.Q))
            {
                manualRollControl -= 1;
            }
            if (Input.GetKey(KeyCode.E))
            {
                manualRollControl += 1;
            }

            if (Gamepad.current?.leftShoulder.isPressed ?? false)
            {
                manualRollControl -= 1;
            }

            if (Gamepad.current?.rightShoulder.isPressed ?? false)
            {
                manualRollControl += 1;
            }

            manualRollControl = Mathf.Clamp(manualRollControl, -1, 1);

            if (manualRollControl == 0)
            {
                manualRoll = Mathf.MoveTowardsAngle(manualRoll, 0, 30f * Time.deltaTime);
            }
            manualRoll += manualRollControl * Time.deltaTime * 120f;
            #endregion

            // Smooth out Yaw and Pitch

            if (_isAiming)
            {
                _pitchYawDelta = Vector2.Lerp(_pitchYawDelta, _rawPitchYawDelta, Time.deltaTime * 10f);

                pitch -= _pitchYawDelta.y * Time.deltaTime * 60f;
                pitch = Mathf.Clamp(pitch, -minPitchAngle, maxPitchAngle);

                yaw += _pitchYawDelta.x * Time.deltaTime * 60f * 0.8f;

                roll *= Mathf.Lerp(0.95f, 0.99f, Mathf.Abs(_pitchYawDelta.x));
                if (manualRollControl == 0)
                {
                    roll -= _pitchYawDelta.x;
                    roll -= Input.GetAxis("Horizontal") * 0.2f;
                }
                roll = Mathf.Clamp(roll, -maxBankAngle, maxBankAngle);
            }
            else
            {
                pitch = Mathf.MoveTowards(pitch, 0, Time.deltaTime * 20f);
                roll *= 0.98f;
                if (manualRollControl == 0)
                {
                    roll -= Input.GetAxis("Horizontal") * 0.5f;
                }
                roll = Mathf.Clamp(roll, -maxFreeLookBankAngle, maxFreeLookBankAngle);
            }

            if (Input.GetMouseButton(0) || (Gamepad.current?.rightTrigger.isPressed ?? false)) Attack();
        }

        private void FixedUpdate()
        {
            if (!waterCheck.inWater)
            {
                Debug.Log("In water!");
                return;
            }

            var manualThrust = _activeThrustControl.Evaluate(Input.GetAxis("Vertical"));

            var directionalThrust =
                thrustByVelocityDot.Evaluate(Vector3.Dot(transform.forward, _rb.linearVelocity.normalized)) * maxThrust;
            var groundLiftForce = 0f;

            var ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out var hitInfo, groundLiftCheckDistance, applyGroundForceLayerMask.value,
                    QueryTriggerInteraction.Ignore))
            {
                // (1 - d) to apply a stronger force as we get close
                groundLiftForce = groundLiftMaxForce * (1f - hitInfo.distance / groundLiftCheckDistance);
            }

            var speedBoost = _isBoosted ? 10f : 0;

            TargetLockOn lockOn = this.GetComponent<TargetLockOn>();

            bool isLockedOn = lockOn.target != null;
            if (isLockedOn)
            {
                lockOnRotation = _rb.rotation;

                if (lockOn.distanceToTarget <= minDistanceToEnemy)
                {
                    _rb.linearVelocity = Vector3.zero;
                }
                else
                {
                    _rb.linearVelocity =
                transform.forward * (Mathf.Min(directionalThrust + manualThrust, maxSpeed) + speedBoost);
                }
            }
            else
            {
                if (isLockedOn)
                {
                    _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, lockOnRotation, 0.5f));
                    isLockedOn = false;
                }
                else
                {
                    _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, Quaternion.Euler(pitch, yaw, manualRoll + roll), 0.5f));
                }
                _rb.linearVelocity =
                transform.forward * (Mathf.Min(directionalThrust + manualThrust, maxSpeed) + speedBoost);
            }


            _rb.AddForce(transform.right.With(y: 0).normalized * (Input.GetAxis("Horizontal") * 2f),
                ForceMode.VelocityChange);
            _rb.AddForce(Vector3.up * groundLiftForce, ForceMode.VelocityChange);
            _rb.AddForce(-Physics.gravity, ForceMode.Acceleration);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnSprint(InputValue value)
        {
            var isBoosted = value.isPressed;
            if (isBoosted == _isBoosted) return;

            freeLookCamera.HorizontalAxis.TriggerRecentering();
            freeLookCamera.VerticalAxis.TriggerRecentering();
            _isBoosted = isBoosted;
        }

        // ReSharper disable once UnusedMember.Local
        public void OnToggleAim(InputValue value)
        {
            // var isAiming = value.isPressed;
            // if (isAiming == _isAiming) return;
            var isAiming = !_isAiming;

            _activeThrustControl = isAiming ? aimingThrustControl : manualThrustControl;

            hudReticle.gameObject.SetActive(isAiming);
            freeLookCamera.gameObject.SetActive(!isAiming);
            _isAiming = isAiming;
        }

        private void SpecialMovement(InputValue value)
        {
            var axis = value.Get<float>();

            manualRoll += axis;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnLook(InputValue value)
        {
            var input = value.Get<Vector2>();

            _rawPitchYawDelta = input.ClampIndividualElements(-2, 2);
        }

        private void Attack()
        {
            if (shootOnlyWhileAiming && !_isAiming) return;

            if (_lastFiredTimer > 0) return;

            var spawnOffset = gunOffset.With(_lastUsedGunLeft ? gunOffset.x : -gunOffset.x);
            var spawnPoint = transform.TransformPoint(spawnOffset);

            var ray = _isAiming
                ? _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0))
                : new Ray(transform.position, transform.forward);

            var target = transform.position + ray.direction * maxRange;

            if (Physics.Raycast(ray, out var hitInfo, maxRange, -1, QueryTriggerInteraction.Ignore))
            {
                target = hitInfo.distance < 20f ? hitInfo.point : transform.position + transform.forward * maxRange;
            }

            GameObject bullet = ObjectPool.SharedInstance.GetPooledObject("BulletPool");
            if (bullet != null)
            {
                bullet.GetComponent<Bullet>().shooter = this.gameObject;
                bullet.transform.position = spawnPoint;
                bullet.SetActive(true);
                bullet.GetComponent<Bullet>().target = target;
                //bullet.transform.LookAt(target);
            }

            _lastFiredTimer = FireDelay;

            _lastUsedGunLeft = !_lastUsedGunLeft;
        }

        private void UpdateTimers(float deltaTime)
        {
            _lastFiredTimer -= deltaTime;
        }
    }
}
