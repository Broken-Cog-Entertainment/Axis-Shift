using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private GameObject[] totalNumberOfEnemies;
    private List<GameObject> enemyList;
    private int enemiesRemainingCount;

    [SerializeField] private TMP_Text enemiesRemainingText;

    // Start is called before the first frame update
    void Start()
    {
        totalNumberOfEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyList = new List<GameObject>(totalNumberOfEnemies);
    }

    // Update is called once per frame
    void Update()
    {
        enemiesRemainingCount = totalNumberOfEnemies.Length;

        enemyList.RemoveAll(x => x == null);
        totalNumberOfEnemies = enemyList.ToArray();
        enemiesRemainingText.text = "Enemies Remaining: " + totalNumberOfEnemies.Length.ToString();

        if(enemiesRemainingCount == 0)
        {
            enemiesRemainingText.text = "MISSION COMPLETE!";
            enemiesRemainingText.color = new Color(0, 0, 1, 1);
        }
    }
}
