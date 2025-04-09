using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using System.Linq;

namespace AS.Player
{
    public class TargetLockOn : MonoBehaviour
    {
        public Transform player;
        public Transform target;

        public bool lockedOn = false;

        public LayerMask enemyLayer;

        public float detectionRadius;

        public float distanceToTarget;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                lockedOn = !lockedOn;
            }

            if (lockedOn)
            {
                player.LookAt(target);
                FindNearestEnemy();
                if (target != null)
                {
                    distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
                }
            }
            if (target == null)
            {
                lockedOn = false;
                //player.rotation = Quaternion.identity;
            }
        }

        void FindNearestEnemy()
        {
            Collider[] enemies = Physics.OverlapSphere(player.position, detectionRadius, enemyLayer);
            List<Transform> visibleEnemies = new List<Transform>();

            foreach (var enemy in enemies)
            {
                Transform _enemy = enemy.transform;
                if (IsInView(_enemy))
                {
                    visibleEnemies.Add(_enemy);
                }
            }

            if (visibleEnemies.Count > 0)
            {
                target = visibleEnemies.OrderBy(t => Vector3.Distance(player.position, t.position)).First();
            }
        }

        bool IsInView(Transform target)
        {
            Vector3 viewPos = Camera.main.WorldToViewportPoint(target.position);
            return viewPos.z > 0 && viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1;
        }
    }
}
