using UnityEngine;

namespace AS
{
    public class TurretEnemyBehaviour : EnemyBehaviour
    {
        [SerializeField] private LineRenderer laserTarget;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Update is called once per frame
        public override void Update()
        {
            if (CanSeePlayer())
            {
                attacking = true;
            }
            else
            {
                RotateTurret();
            }

            if (attacking)
            {
                if (player == null || laserTarget == null) return;

                Vector3 startPos = sightTransform.position;
                Vector3 aimDirection = (player.position - startPos).normalized;
              //  RaycastHit hit;

             //   if(Physics.Raycast(startPos, aimDirection, out hit, viewDistance, whatIsPlayer))
            //    {
             //       laserTarget.SetPosition(0, startPos);
             //       laserTarget.SetPosition(1, hit.point);
             //   }
             //   else
              //  {
               //     laserTarget.SetPosition(0, startPos);
               //     laserTarget.SetPosition(1, player.position);
               // }
                sightTransform.LookAt(player);

                Attacking();
            }
        }

        public override bool CanSeePlayer()
        {
            return base.CanSeePlayer();
        }

        public override void Attacking()
        {
            if (Time.time >= nextFire)
            {
                Shoot();
                nextFire = Time.time + attackRate;
            }
        }

        public override void Shoot()
        {
            base.Shoot();
        }

        public void RotateTurret()
        {
            sightTransform.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
    }
}
