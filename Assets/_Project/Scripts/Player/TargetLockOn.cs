using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using System.Linq;
using static UnityEditor.PlayerSettings;
using AS.Player;
using UnityEngine.InputSystem;

public class TargetLockOn : MonoBehaviour
{
    public Transform target;

    public bool lockedOn = false;
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

    private Quaternion lastPlayerRotation;


    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        if(Physics.SphereCast(ray, lockOnRadius, out hit, viewRange, enemyLayer))
        {
            target = hit.transform;
        }
        else
        {
            target = null;
        }

        if(target != null)
        {
            LockOnToTarget();
            CheckLockBreak();

            if (target != null)
            {
                distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
            }
            else
            {
                distanceToTarget = Mathf.Infinity;
            }
        }


    }

    void LockOnToTarget()
    {
        Vector3 directionToFace = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(directionToFace);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5f * Time.deltaTime);

        lockOnCam.LookAt = target.transform;

        lastMousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    void CheckLockBreak()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
       
        float movement = Vector3.Distance(transform.position, lastPlayerPos);
      
        Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float mouseMovement = Vector2.Distance(mousePos, lastMousePos);

        bool shouldUnlock = distance > (viewRange + lockBreakDistance) || movement > lockBreakMovement || mouseMovement >= lockBreakInput;

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
        lastPlayerRotation = transform.rotation;
    }

    void UnlockTarget()
    {
        Debug.Log("Unlocking target!");
        target = null;

        this.transform.rotation = lastPlayerRotation;
    }
}
