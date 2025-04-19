using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool gameRunning = true;
    private bool onCooldown = false;

    public GameObject Flashing;
    public GameObject enemyDot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle gameRunning when Q is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameRunning = !gameRunning;
        }
        
        if (Input.GetKeyDown(KeyCode.F) && !onCooldown)
        {
            StartCoroutine(Flash(Flashing));
        }
    }

    IEnumerator Flash(GameObject gameObject)
    {
        onCooldown = true;
        gameObject.SetActive(true);
        enemyDot.SetActive(true);
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
        enemyDot.SetActive(false);
        yield return new WaitForSeconds(5);
        onCooldown = false;
    }
}
