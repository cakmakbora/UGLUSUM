using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManagerScene2 : MonoBehaviour
{
    
    private bool onCooldown = false;

    public GameObject Flashing1;
    public GameObject Flashing2;
    public GameObject Flashing3;    
    public GameObject enemyDot1;
    public GameObject enemyDot2;
    public GameObject enemyDot3;

    public Animator animator1;
    public Animator animator2;
    public Animator animator3;
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
            GameManager.gameRunning = !GameManager.gameRunning;
            animator1.enabled = true;
            animator2.enabled = true;
            animator3.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.F) && !onCooldown)
        {
            StartCoroutine(Flash(Flashing1, Flashing2, Flashing3));
        }
    }

    IEnumerator Flash(GameObject gameObject1, GameObject gameObject2, GameObject gameObject3)
    {
        onCooldown = true;
        gameObject1.SetActive(true);
        gameObject2.SetActive(true);
        gameObject3.SetActive(true);
        enemyDot1.SetActive(true);
        enemyDot2.SetActive(true);
        enemyDot3.SetActive(true);
        yield return new WaitForSeconds(1);
        gameObject1.SetActive(false);
        gameObject2.SetActive(false);
        gameObject3.SetActive(false);
        enemyDot1.SetActive(false);
        enemyDot2.SetActive(false);
        enemyDot3.SetActive(false);
        yield return new WaitForSeconds(5);
        onCooldown = false;
    }
}
