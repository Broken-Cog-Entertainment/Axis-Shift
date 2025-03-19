using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSwitch : MonoBehaviour
{
    public Material disabledMaterial;
    public Material enabledMaterial;

    public bool activated = false;

    private void Start()
    {
        this.GetComponent<MeshRenderer>().material = disabledMaterial;
    }

    public void Activate()
    {
        if (activated) return;

        activated = true;
        this.GetComponent<MeshRenderer>().material = enabledMaterial;

        Debug.Log("Activated switch!");
    }
}
