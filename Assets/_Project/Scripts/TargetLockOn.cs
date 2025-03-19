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

    private Vector3 lastPlayerPos;
    private Vector2 lastMousePos;


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

            if(target != null)
            {
                distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
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
        if(distance > viewRange + lockBreakDistance)
        {
            UnlockTarget();
            return;
        }

        float movement = Vector3.Distance(transform.position, lastPlayerPos);
        if(movement > lockBreakMovement)
        {
            UnlockTarget();
            return;
        }

        Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float mouseMovement = Vector2.Distance(mousePos, lastMousePos);

        if(mouseMovement >= lockBreakInput)
        {
            UnlockTarget();
            return;
        }

        lastPlayerPos = transform.position;
    }

    void UnlockTarget()
    {
        target = null;
    }
}
