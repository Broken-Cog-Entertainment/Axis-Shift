using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundEnemyBehaviour : EnemyBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform turret;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);

        Quaternion turretRotation = Quaternion.LookRotation(player.position - turret.transform.position, Vector3.up);
        turret.transform.rotation = turretRotation * Quaternion.Euler(180, 180, 90);
    }

    public override void ChasePlayer()
    {
        base.ChasePlayer();

        if (!attacking)
        {
            Debug.Log("Chasing player.");
            agent.SetDestination(player.position);
        }

        if (!agent.pathPending && Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            chasing = false;
            attacking = true;
            agent.ResetPath();
        }
    }

    public override bool CanSeePlayer()
    {
        return base.CanSeePlayer();
    }

    public override Vector3 Wander(float radius)
    {
        //Returns a random position to move towards within a radius of 5
        Vector3 randomDirection = Random.insideUnitSphere * radius + transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, radius, NavMesh.AllAreas))
        {
            return navHit.position;
        }

        return base.Wander(radius);
    }

    public override void Attacking()
    {
        agent.isStopped = false;

        base.Attacking();
    }

    public override void Shoot()
    {
        base.Shoot();
    }

    public override void Strafing()
    {
        Debug.Log("Starting strafing");
        if (!isStrafing)
        {
            StartCoroutine(StrafeAroundPlayer());
        }
    }

    public override IEnumerator StrafeAroundPlayer()
    {
        isStrafing = true;

        while (attacking)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Vector3 strafeDirection = Vector3.Cross(Vector3.up, directionToPlayer) * (Random.value > 0.5f ? 1 : -1);
            Vector3 strafePos = transform.position + strafeDirection * strafeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(strafePos, out hit, strafeDistance, NavMesh.AllAreas))
            {
                Debug.Log("Strafing to " + hit.position);
                agent.SetDestination(hit.position);
            }
            else
            {
                Debug.Log("Invalid strafe position. Recalculating...");
            }

            yield return new WaitForSeconds(strafeRate);
        }
        Debug.Log("Strafing coroutine ended.");
        isStrafing = false;
    }

    public override void DetectIncomingProjectile()
    {
        base.DetectIncomingProjectile();
    }

    public override IEnumerator Dodging()
    {
        if (!canDodge) yield break;
        canDodge = false;

        Vector3 dodgeDirection = (Random.value > 0.5f ? 1 : -1) * Vector3.Cross(Vector3.up, (player.position - transform.position)).normalized;
        Vector3 dodgePos = transform.position + dodgeDirection * dodgeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(dodgePos, out hit, dodgeDistance, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.speed = dodgeSpeed;
            agent.acceleration = dodgeSpeed * 5f;
            agent.SetDestination(hit.position);

            Debug.Log("Dodged to " + hit.position);

            float dodgeTime = 0.2f;
            float elapsedTime = 0f;

            while (elapsedTime < dodgeTime)
            {

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        agent.speed = moveSpeed;
        agent.acceleration = accelerateSpeed;

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    public override void SeekPlayer()
    {
        agent.SetDestination(lastknownPosition);

        if (agent.remainingDistance < 0.1f)
        {
            searchTimer += Time.deltaTime;

            if (searchTimer >= 2f)
            {
                searchTimer = 0f;
                seek = false;
                wandering = true;

                agent.SetDestination(Wander(5f)); //Finds a random nearby position to wander to before returning home
                Debug.Log("Beginning to wander");
            }
        }
    }

    public override void Wander()
    {
        if (!agent.pathPending && agent.remainingDistance <= 0.1f)
        {
            returning = true;
            wandering = false;
        }
    }

    public override void ReturnHome()
    {
        agent.SetDestination(home.position);

        if (agent.remainingDistance < 0.1f)
        {
            Quaternion targetRoation = home.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRoation, 5f * Time.deltaTime); //Smoothly reset rotation over time

            if (Quaternion.Angle(transform.rotation, targetRoation) <= 0.01f)
            {
                returning = false;
            }
        }
    }
}
