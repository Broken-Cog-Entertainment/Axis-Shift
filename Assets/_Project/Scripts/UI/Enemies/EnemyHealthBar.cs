using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public Transform enemyPos;

    // Update is called once per frame
    void Update()
    {
        if(enemyPos != null)
        {
            this.transform.position = new Vector3(enemyPos.position.x, enemyPos.position.y + 3, enemyPos.position.z);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
