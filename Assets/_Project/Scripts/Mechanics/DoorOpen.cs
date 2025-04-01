using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public ActivateSwitch[] switches;
    public bool opened = false;

    public Animator myAnim;

    public GameObject activatedObject; //This object changes colour to show that the door has been unlocked
    public Material disabledMaterial;
    public Material enabledMaterial;

    private void Start()
    {
        myAnim = GetComponentInChildren<Animator>();
        switches = GetComponentsInChildren<ActivateSwitch>();
        activatedObject.GetComponent<MeshRenderer>().material = disabledMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        CheckActiveSwitches();
    }

    void CheckActiveSwitches()
    {
        if (opened) return;

        bool allSwitchesActivated = true;

        foreach(ActivateSwitch _switch in switches)
        {
            if (!_switch.activated)
            {
                allSwitchesActivated = false;
                break;
            }
        }

        if (allSwitchesActivated && !opened)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        opened = true;
        myAnim.SetBool("Open", true);
        activatedObject.GetComponent<MeshRenderer>().material = enabledMaterial;
    }
}
