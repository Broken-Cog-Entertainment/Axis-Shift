using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AS
{
    public class FlyingEnemyBehaviour : EnemyBehaviour
    {
        [SerializeField] private Rigidbody myRB;
        [SerializeField] private Vector3 wanderPos;

        [SerializeField] private float targetHeight = 10f;
        [SerializeField] private float heightCorrectionSpeed = 2f;

        private bool isRotating = false;

        // Start is called before the first frame update
        public override void Start()
        {
            myRB = GetComponent<Rigidbody>();
            player = GameObject.FindGameObjectWithTag("Player").transform;

            returning = true; //Enemy returns to home by default

            if (waypoints == null || waypoints.Length == 0) return;
        }

        public void FixedUpdate()
        {
            DetectIncomingProjectile();
            MaintainHeight();

            if (CanSeePlayer() || DistanceToPlayer() < chaseDistance)
            {
                AttackState();
            }
            else
            {
                CircleState();
            }
        }

        void CircleState()
        {
            if (currentWaypoint == null || ReachedWaypoint())
            {
                PickRandomWaypoint();
            }
            else
            {
                Move(currentWaypoint.position);
            }
        }

        void AttackState()
        {
            if (!isRotating)
            {
                StartCoroutine(RotateTowardsTarget(angleToShoot));
            }

            LookTowards(player.position);

            if (DistanceToPlayer() > attackDistance)
            {
                Move(player.position);
            }
            else if (!isStrafing)
            {
                StartCoroutine(StrafeAroundPlayer());
            }

            nextFire += Time.deltaTime;

            if (nextFire >= attackRate)
            {
                nextFire = 0f;
                Attacking();
                Debug.Log("Shooting at player...");
            }
        }

        public override bool CanSeePlayer()
        {
            return base.CanSeePlayer();
        }

        public override Vector3 Wander(float radius)
        {
            Vector3 randomDirection = new Vector3(
                Random.Range(-radius, radius),
                0,
                Random.Range(-radius, radius));

            return transform.position + randomDirection;
        }

        public override void Attacking()
        {
            Shoot();

            // if (!isStrafing)
            //  {
            //     StartCoroutine(StrafeAroundPlayer());
            // }
        }

        public override void Shoot()
        {
            base.Shoot();
        }

        public override void Strafing()
        {
            base.Strafing();
        }

        public override IEnumerator StrafeAroundPlayer()
        {
            isStrafing = true;
            float elapsedTime = 0f;

            elapsedTime += Time.deltaTime;
            Vector3 currentStrafeDirection = Vector3.Cross(Vector3.up, (player.position - transform.position).normalized);
            if (Random.value > 0.5f) currentStrafeDirection *= -1;

            myRB.MovePosition(transform.position + currentStrafeDirection * strafeSpeed * Time.deltaTime);
            LookTowards(player.position);

            Debug.Log("Strafing to " + currentStrafeDirection);

            if (elapsedTime >= strafeRate)
            {
                elapsedTime = 0;
                currentStrafeDirection *= -1;
            }

            Debug.Log("Strafing coroutine ended.");
            yield return new WaitForSeconds(1f);
            isStrafing = false;
        }

        public override void DetectIncomingProjectile()
        {
            base.DetectIncomingProjectile();

            if (canDodge)
            {
                StartCoroutine(Dodging());
                canDodge = false;
            }
        }

        public override IEnumerator Dodging()
        {
            if (!canDodge) yield break;

            Vector3 randomDodgeDirection = Vector3.Cross(Vector3.up, (player.position - transform.position).normalized);
            if (Random.value > 0.5f) randomDodgeDirection *= -1;

            myRB.AddForce(randomDodgeDirection * dodgeSpeed, ForceMode.VelocityChange);
            yield return new WaitForSeconds(0.2f);

            myRB.linearVelocity = Vector3.zero;

            yield return new WaitForSeconds(dodgeCooldown);
            canDodge = true;
        }

        public void Move(Vector3 targetPos)
        {
            Vector3 dir = targetPos - transform.position;
            if (dir.sqrMagnitude < 0.01f) return;

            dir.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }

        private float DistanceToPlayer()
        {
            if (!player) return float.MaxValue;
            return Vector3.Distance(transform.position, player.position);
        }

        void LookTowards(Vector3 targetPos)
        {
            Vector3 targetDirection = targetPos - transform.position;
            if (targetDirection.sqrMagnitude < 0.01f) return;

            targetDirection.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * rotateSpeed);
        }

        private bool FaceTarget(float angleThreshold)
        {
            if (!player) return true;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            return angle <= angleThreshold;
        }

        IEnumerator RotateTowardsTarget(float angleThreshold)
        {
            isRotating = true;

            while (!FaceTarget(angleThreshold))
            {
                LookTowards(player.position);
                yield return null;
            }

            isRotating = false;
        }

        void PickRandomWaypoint()
        {
            if (waypoints != null && waypoints.Length > 0)
            {
                currentWaypoint = waypoints[Random.Range(0, waypoints.Length)];
            }
        }

        bool ReachedWaypoint()
        {
            if (!currentWaypoint) return false;
            return Vector3.Distance(transform.position, currentWaypoint.position) < waypointDistanceThreshold;
        }
        void MaintainHeight()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                float currentHeight = hit.distance;
                float desiredHeight = hit.point.y + targetHeight;

                if (currentHeight < targetHeight)
                {
                    float heightDifference = targetHeight - currentHeight;
                    Vector3 correctedPos = transform.position + Vector3.up * heightDifference;

                    transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, desiredHeight, Time.deltaTime * heightCorrectionSpeed), transform.position.z);
                }

            }
        }
    }
}

