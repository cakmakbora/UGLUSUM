using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerScene2 : MonoBehaviour
{
    public static GameManagerScene2 instance;
    public static bool gameRunning = true;
    private bool onCooldown = false;

    [Header("UI References")]
    public GameObject deathScreenPanel;
    public TextMeshProUGUI deathText;
    public Button restartButton;
    public GameObject Player;

    [Header("Game Objects")]
    public GameObject Flashing1;
    public GameObject Flashing2;
    public GameObject Flashing3;    
    public GameObject enemyDot1;
    public GameObject enemyDot2;
    public GameObject enemyDot3;

    public Animator animator1;
    public Animator animator2;
    public Animator animator3;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // UI elementlerini başlangıçta gizle
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
        }

        // Butonlara listener'ları ekle
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame2);
        }
        
    }
    
    public void Die()
    {
        gameRunning = false;
        if (animator1 != null) animator1.enabled = false;
        if (animator2 != null) animator2.enabled = false;
        if (animator3 != null) animator3.enabled = false;
        
        Player.GetComponent<Rigidbody>().isKinematic = true;
        gameRunning = false;
        Time.timeScale = 0;
        deathScreenPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame2()
    {
        // Mevcut sahneyi yeniden yükle
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }



    // Update is called once per frame
    void Update()
    {
        // Toggle gameRunning when Q is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameRunning = !gameRunning;
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
