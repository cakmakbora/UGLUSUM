using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool gameRunning = true;
    private bool onCooldown = false;

    public GameObject Flashing;
    public GameObject enemyDot;

    public Animator animator;

    public GameObject Player;
    public GameObject DeathScreen;
    // Start is called before the first frame update
    void Start()
    {
        gameRunning = true;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle gameRunning when Q is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameRunning = !gameRunning;
            animator.enabled = true;
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
    public void Die()
    {
        Player.GetComponent<Rigidbody>().isKinematic = true;
        gameRunning = false;
        Time.timeScale = 0;
        DeathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void RestartGame()
    {
        
        SceneManager.LoadScene(0);
    }
}
