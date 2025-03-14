using AS.Utils;
using AS.Utils.MathUtils;
using AS.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AS.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public AnimationCurve thrustByVelocityDot;
        public AnimationCurve manualThrustControl;
        public float maxThrust = 10f;
        public float maxSpeed = 20f;
        
        [Header("Rotation Controls")]
        [Range(20, 90)] public float maxBankAngle = 30f;
        [Range(15, 90)] public float minPitchAngle;
        [Range(15, 90)] public float maxPitchAngle;

        [Header("Weapon Controls")]
        public float maxRange;
        public Vector3 gunOffset;
        public bool useRealisticBulletPhysics;
        public Bullet bulletPrefab;
        public float fireRate;
        
        public float pitch;
        public float yaw;
        public float roll;

        public float FireDelay => 1f / fireRate;
        
        private Rigidbody _rb;
        private Camera _mainCamera;
        
        private float _yawDelta;
        private float _yawDeltaRaw;
        private float _pitchDelta;
        private float _pitchDeltaRaw;

        private Vector2 _pitchYawDeltaRaw;
        private Vector2 _pitchYawDelta;
        
        private Vector3 _movement;

        private bool _lastUsedGunLeft;
        private float _lastFiredTimer;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            _mainCamera = Camera.main;
            _rb.freezeRotation = true;
        }

        private void OnLook(InputValue value)
        {
            var input = value.Get<Vector2>();
            
            var deltaX = Mathf.Clamp(input.x, -2, 2);
            var deltaY = Mathf.Clamp(input.y, -2, 2);
            
            _pitchYawDeltaRaw = input.Clamp(-2, 2);
            _yawDeltaRaw = deltaX;
            _pitchDeltaRaw = deltaY;
        }
        
        private void Attack()
        {
            if (_lastFiredTimer > 0) return;

            var spawnOffset = gunOffset.With(x: _lastUsedGunLeft ? gunOffset.x : -gunOffset.x);
            var spawnPoint = transform.TransformPoint(spawnOffset);

            
            var ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            var target = transform.position + ray.direction * maxRange;
            
            if (Physics.Raycast(ray, out var hitInfo, maxRange, -1, QueryTriggerInteraction.Ignore) && hitInfo.distance > 20f)
            {
                target = hitInfo.point;
            }
            
            var bullet = Instantiate(bulletPrefab);
            bullet.transform.position = spawnPoint;
            bullet.target = target;
            bullet.transform.LookAt(target);

            _lastFiredTimer = FireDelay;
            
            _lastUsedGunLeft = !_lastUsedGunLeft;
        }

        private void Update()
        {
            _lastFiredTimer -= Time.deltaTime;
            
            // Smooth out Yaw and Pitch
            _yawDelta = Mathf.Lerp(_yawDelta, _yawDeltaRaw, Time.deltaTime * 10f);
            _pitchDelta = Mathf.Lerp(_pitchDelta, _pitchDeltaRaw, Time.deltaTime * 5f);
            
            pitch -= _pitchDelta * Time.deltaTime * 90f;
            pitch = Mathf.Clamp(pitch, -minPitchAngle, maxPitchAngle);
            
            yaw += _yawDelta * Time.deltaTime * 90f * 0.6f;
            
            roll *= Mathf.Lerp(0.95f, 0.99f, Mathf.Abs(_yawDelta));
            roll -= _yawDelta;
            roll = Mathf.Clamp(roll, -maxBankAngle, maxBankAngle);
            
            // _rb.rotation = Quaternion.Euler(pitch, yaw, bankAngle);

            if (Input.GetMouseButton(0) || (Gamepad.current?.rightTrigger.isPressed ?? false))
            {
                Attack();
            }
        }

        private void FixedUpdate()
        {
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, Quaternion.Euler(pitch, yaw, roll), 0.5f));
            
            var manualThrust = manualThrustControl.Evaluate(Input.GetAxis("Vertical"));
            var directionalThrust = thrustByVelocityDot.Evaluate(Vector3.Dot(transform.forward, _rb.linearVelocity.normalized)) * maxThrust;
            var groundLiftForce = 0f;
            
            var ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out var hitInfo, 3f, -1, QueryTriggerInteraction.Ignore))
            {
                Debug.DrawLine(ray.origin, hitInfo.point);
                groundLiftForce = 2f;
            }
            
            _rb.linearVelocity = transform.forward * Mathf.Min(directionalThrust + manualThrust, maxSpeed) + Vector3.up * groundLiftForce;
            
            _rb.AddForce(-Physics.gravity, ForceMode.Acceleration);
        }
    }
}
