using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using System.Linq;
using static UnityEditor.PlayerSettings;
using AS.Player;
using UnityEngine.InputSystem;

public class AimAssist : MonoBehaviour
{
    public Transform target;

    public bool lockedOn = false;
    public bool canLockOn = true;
    //public Image crosshairIcon;

    public LayerMask enemyLayer;

    public float viewRange;
    public float lockOnRadius;
    public CinemachineCamera lockOnCam;

    public float distanceToTarget;

    public float lockBreakInput;
    public float lockBreakDistance;
    public float lockBreakMovement;
    public float lockBreakTimer;
    public float lockBreakThreshold;

    private Vector3 lastPlayerPos;
    private Vector2 lastMousePos;

    public Quaternion currentRotation;


    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        if (target == null && canLockOn)
        {
            Debug.Log("Attempting to find a target...");
            if (Physics.SphereCast(ray, lockOnRadius, out hit, viewRange, enemyLayer))
            {            
                target = hit.transform;
                lockedOn = true;
                Debug.Log($"Target Locked: {target.name}");
            }
            else
            {
                Debug.Log("No valid target found.");
            }
        }

        if (target != null && lockedOn)
        {
            if (target.gameObject == null)
            {
                UnlockTarget();
            }
            else
            {
                LockOnToTarget();
                CheckLockBreak();

                if(target != null)
                {
                    distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
                }            
            }
        }
    }

    void LockOnToTarget()
    {
        Vector3 directionToFace = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(directionToFace);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5f * Time.deltaTime);

        lockOnCam.LookAt = target.transform;
    }

    void CheckLockBreak()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        float movement = Vector3.Distance(transform.position, lastPlayerPos);

        bool shouldUnlock = distance > (viewRange + lockBreakDistance) || movement > lockBreakMovement || Mouse.current.delta.ReadValue().magnitude >= lockBreakInput * 0.5f;

        Debug.Log($"Mouse Delta: {Mouse.current.delta.ReadValue().magnitude}");

        if (shouldUnlock)
        {
            lockBreakTimer += Time.deltaTime;

            if(lockBreakTimer >= lockBreakThreshold)
            {
                UnlockTarget();
            }
        }
        else
        {
            lockBreakTimer = 0;
        }

        lastPlayerPos = transform.position;
    }

    void UnlockTarget()
    {
        if (target != null && target.gameObject != null)
        {
            Debug.Log($"Previous target: {target.name}");
        }

        Debug.Log("Unlocking target!");
        target = null;
        lockedOn = false;
        canLockOn = false;
        StartCoroutine(ResetLockOn());

        currentRotation = transform.rotation;
    }

    IEnumerator ResetLockOn()
    {
        yield return new WaitForSeconds(0.5f);
        canLockOn = true;
    }
}
