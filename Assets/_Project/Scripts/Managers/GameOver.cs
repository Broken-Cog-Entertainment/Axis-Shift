using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public bool gameOver;
    public GameObject gameOverCanvas;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        gameOverCanvas.SetActive(false);
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if(health.currentHealth[0] <= 0)
        {
            gameOver = true;
        }

        if (gameOver)
        {
            gameOverCanvas.SetActive(true);
        }
    }
}
