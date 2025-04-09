using AS.Utils.MathUtils;
using AS.Weapons;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AS.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class TankController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float maxThrust = 10f;
        public float maxSpeed = 20f;
        public float acceleration;
        public float turnSpeed;
        public float currentSpeed;

        [Header("Weapon Controls")]
        public Image hudReticle;

        public bool shootOnlyWhileAiming = true;
        public float maxRange;
        public Vector3 gunOffset;
        public GameObject bombPrefab;
        public float fireRate;

        [Header("Cameras")]
        public CinemachineOrbitalFollow freeLookCamera;

        public Transform aimPos;
        public float aimSmoothSpeed;
        public LayerMask aimMask;

        float pitch;
        float yaw;

        Vector2 lookInput;
        public Transform camRoot;

        public Transform firePos;

        public float manualRoll;

        public float groundDistance = 10f;
        public LayerMask groundMask;

       // public ThreeAxisControl control;
        private AnimationCurve _activeThrustControl;

        private bool _isAiming;
        private bool _isBoosted;
        public float _lastFiredTimer;

        private bool _lastUsedGunLeft;
        private Camera _mainCamera;

        private Vector3 _movement;
        private Vector2 _pitchYawDelta;

        private Vector2 _rawPitchYawDelta;

        private Rigidbody _rb;

        private float FireDelay => 1f / fireRate;

        bool isLockingOn;
        Quaternion lockOnRotation;

        public Transform turretTransform;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _mainCamera = Camera.main;
            //_rb.freezeRotation = true;

            hudReticle.gameObject.SetActive(false);
            //_activeThrustControl = manualThrustControl;
        }

        private void Update()
        {
            UpdateTimers(Time.deltaTime);

            if (_isAiming)
            {
                lookInput.x = Input.GetAxis("Mouse X") * 2f;
                lookInput.y = Input.GetAxis("Mouse Y") * 2f;

                pitch -= lookInput.y;
                pitch = Mathf.Clamp(pitch, -40f, 20f);

                yaw += lookInput.x;

                camRoot.eulerAngles = new Vector3(pitch, yaw, 0f);

             //   aimPos.position = turretTransform.position + turretTransform.forward;

                Vector3 flatForward = camRoot.forward;
                flatForward.y = 0f;
                flatForward.Normalize();
                turretTransform.forward = flatForward;

                // Raycast from center of screen to place the aim target
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimMask))
                {
                    aimPos.position = hit.point;
                }
                else
                {
                    aimPos.position = ray.origin + ray.direction * 1000f;
                }

                if (turretTransform != null)
                {
                    Vector3 direction = aimPos.position - turretTransform.position;
                    turretTransform.forward = direction.normalized;
                }

            }
            
            if (Input.GetMouseButton(0) || (Gamepad.current?.rightTrigger.isPressed ?? false)) Attack();
        }

        private void FixedUpdate()
        {
            float moveInput = Input.GetAxis("Vertical");
            float turnInput = Input.GetAxis("Horizontal");

            // Accelerate/decelerate forward motion
            float targetSpeed = moveInput * maxSpeed;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

            // Move
            Vector3 move = transform.forward * currentSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + transform.forward * currentSpeed * Time.fixedDeltaTime);

            // Rotate tank body
            if (Mathf.Abs(turnInput) > 0.01f)
            {
                float turnAmount = turnInput * turnSpeed * Time.fixedDeltaTime;
                Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
                _rb.MoveRotation(_rb.rotation * turnRotation);
            }

            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 2f))
            {
                Vector3 surfaceNormal = hit.normal;
                Vector3 forwardProjected = Vector3.ProjectOnPlane(transform.forward, surfaceNormal).normalized;
                Quaternion slerpRotation = Quaternion.LookRotation(forwardProjected, surfaceNormal);
                Quaternion blendRotation = Quaternion.Slerp(_rb.rotation, slerpRotation, Time.fixedDeltaTime * 5f);
                _rb.MoveRotation(blendRotation);
            }

            if(_rb.linearVelocity.magnitude > maxSpeed)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
            }
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

            //_activeThrustControl = isAiming ? aimingThrustControl : manualThrustControl;

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
            if (shootOnlyWhileAiming && !_isAiming)
            {
                Debug.Log("Not aiming!");
                return;
            }

            if (_lastFiredTimer > 0)
            {
                Debug.Log("Timer greater than zero!");
                return;
            }

         // var spawnOffset = gunOffset.With(_lastUsedGunLeft ? gunOffset.x : -gunOffset.x);
            var spawnPoint = transform.TransformPoint(firePos.position);

            var ray = _isAiming
                ? _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0))
                : new Ray(firePos.position, firePos.forward);

            var target = firePos.position + ray.direction * maxRange;

            if (Physics.Raycast(ray, out var hitInfo, maxRange, -1, QueryTriggerInteraction.Ignore))
            {
                target = hitInfo.distance < 20f ? hitInfo.point : firePos.position + firePos.forward * maxRange;
            }

            GameObject bomb = Instantiate(bombPrefab, firePos.position, Quaternion.identity);
            if (bomb != null)
            {
                bomb.GetComponent<ExplosiveProjectile>().shooter = this.gameObject;
                bomb.GetComponent<ExplosiveProjectile>().target = target;
                bomb.GetComponent<Rigidbody>().AddForce(transform.forward * 25f);
            }

            _lastFiredTimer = FireDelay;

       //   _lastUsedGunLeft = !_lastUsedGunLeft;
        }

        private void UpdateTimers(float deltaTime)
        {
            _lastFiredTimer -= deltaTime;
        }

        void Stabilize()
        {
            Vector3 rightingTorque = Vector3.Cross(transform.up, Vector3.up) * 1000f;
            _rb.AddTorque(rightingTorque);
        }
    }
}