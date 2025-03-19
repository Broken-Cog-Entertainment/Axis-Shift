using AS.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float viewDistance = 10f; //Maximum distance the enemy can see the player
    public float fieldOfViewAngle = 90f; //Angle that the player can be seen in front of the enemy
    public Transform sightTransform; //Transform that represents where the enemy's field of vision originates from
    public float searchTimer = 0; //Time until enemy begins to wander around to search for player
    public float chaseDistance;
    public float attackDistance;
    public float strafeDistance;
    public float strafeSpeed;
    public float strafeRate;
    public bool isStrafing;

    public float moveSpeed;
    public float rotateSpeed;
    public float accelerateSpeed;

    public GameObject projectilePrefab;
    public float attackRate;
    public float attackBurst;
    public float attackDamage;
    public float nextFire;
    public float fireVelocity;
    public float angleToShoot;
    public Transform[] firePos;

    public float dodgeDistance;
    public float dodgeSpeed;
    public float dodgeCooldown;
    public float dodgeReactionTime;
    public bool canDodge = true;

    public LayerMask whatIsPlayer;
    public LayerMask whatIsObstacle;

    public Transform player;
    public Transform home; //Enemy home position
    public Vector3 lastknownPosition; //Position of the player 

    public float timeUntilLosePlayer = 2f;
    public float playerSpottedTime;

    public bool alerted = false;
    public bool chasing = false;
    public bool attacking = false;
    public bool seek = false;
    public bool wandering = false;
    public bool returning = false;

    public float waypointDistanceThreshold;
    public Transform currentWaypoint;
    public Transform[] waypoints;

    private Coroutine dodgeCoroutine;

    //  [SerializeField] private Animator myAnim;

    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        returning = true; //Enemy returns to home by default
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(this.transform.position, player.position); //Checks distance to player if they are not null

        if (CanSeePlayer())
        {
            Debug.Log("Player spotted!");

            ChasePlayer(); //Begin chasing player if they are spotted
        }

        else if (chasing && distanceToPlayer >= chaseDistance) //If player is too far away while being chased, leave chase state and enter seek state
        {
            SeekPlayer();
        }

        if (chasing) ChasePlayer();

        if (attacking) Attacking();

        if (seek) SeekPlayer();

        if (wandering) Wander();

        if (returning) ReturnHome();

        DetectIncomingProjectile();
    }

    public virtual void ChasePlayer()
    {
        lastknownPosition = player.position;
    }
    public virtual bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - sightTransform.position).normalized;
        float distanceToPlayer = Vector3.Distance(sightTransform.position, player.position);

        if (distanceToPlayer > viewDistance) //Cannot see if player is farther than max view distance
        {
            return false;
        }
        float angleToPlayer = Vector3.SignedAngle(sightTransform.forward, transform.up, directionToPlayer);
        if (angleToPlayer > fieldOfViewAngle / 2f) return false; //Cannot see if player is outside field of view angle

        if (Physics.Raycast(sightTransform.position, directionToPlayer, distanceToPlayer, whatIsObstacle)) return false; //Cannot see if player is hiding behind obstacle

        return true;
    }

    public virtual Vector3 Wander(float radius)
    {
        return transform.position;
    }

    public virtual void Attacking()
    {
        Debug.Log("Attacking player.");

        if (Time.time >= nextFire)
        {
            Shoot();
            nextFire = Time.time + attackRate;
        }

        if (!isStrafing)
        {
            Strafing();
        }
    }

    public virtual void Shoot()
    {
        foreach(Transform pos in firePos)
        {
            for(int i = 0; i < attackBurst; i++)
            {
                Debug.Log("Fired projectile!");
                GameObject projectile = Instantiate(projectilePrefab, firePos[i].position, Quaternion.identity);
                projectile.GetComponent<Bullet>().shooter = this.gameObject;
                projectile.GetComponent<Bullet>().damageAmount = attackDamage;

                projectile.GetComponent<Bullet>().target = player.position;
                projectile.GetComponent<Bullet>().transform.LookAt(player);
            }
        }       
    }

    public virtual void Strafing()
    {
        Debug.Log("Starting strafing");
        if (!isStrafing)
        {
            StartCoroutine(StrafeAroundPlayer());
        }
    }

    public virtual IEnumerator StrafeAroundPlayer()
    {
        yield return new WaitForSeconds(strafeRate);
    }

    public virtual void DetectIncomingProjectile()
    {
        Collider[] projectiles = Physics.OverlapSphere(transform.position, viewDistance, LayerMask.GetMask("Projectile"));

        foreach(Collider projectileCollider in projectiles)
        {
            Bullet projectile = projectileCollider.GetComponent<Bullet>();

            if (projectile != null && projectile.shooter.CompareTag("Player"))
            {
                if (canDodge)
                {
                    Debug.Log("Triggered dodge.");
                    dodgeCoroutine = StartCoroutine(Dodging());
                    return;
                }
            }           
        }
    }

    public virtual IEnumerator Dodging()
    {
        yield return new WaitForSeconds(dodgeCooldown);
    }

    public virtual void SeekPlayer()
    {
        
    }

    public virtual void Wander()
    {
        
    }

    public virtual void ReturnHome()
    {
        
    }
}