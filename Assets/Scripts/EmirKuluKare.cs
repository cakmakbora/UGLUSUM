using UnityEngine;
using System.Collections.Generic;

public class EmirKuluKare : MonoBehaviour
{
    [Header("Patrol Settings")]
    public List<Transform> patrolPoints;
    public float patrolSpeed = 3f;
    public float waypointReachedDistance = 0.5f;
    private int currentPatrolIndex = 0;

    [Header("Chase Settings")]
    public float chaseSpeed = 6f;
    public float detectionRange = 50f;
    public float viewAngle = 45f;
    public Transform eyeOrigin;
    public LayerMask playerMask;
    public LayerMask obstructionMask;

    [Header("Animation Settings")]
    public Animator animator;
    public string walkTrigger = "Walk";
    public string runTrigger = "Run";

    private Transform player;
    private bool isChasing = false;
    
    private Vector3 lastKnownPlayerPosition;
    private float currentSpeed;
    private bool wasGameRunning = true;
    public Rigidbody PlayerRb;

    // Start is called before the first frame update
    void Start()
    {
        // Player'� bul
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Animator kontrol�
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Patrol noktalar�n� kontrol et
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("No patrol points assigned! Character will not move.");
            return;
        }

        // Ba�lang�� pozisyonunu ayarla
        transform.position = patrolPoints[0].position;

        // Ba�lang��ta y�r�me animasyonunu ba�lat
        if (animator != null)
        {
            animator.SetTrigger(walkTrigger);
        }

        currentSpeed = patrolSpeed;
        wasGameRunning = GameManager.gameRunning;
    }

    // Update is called once per frame
    void Update()
    {
        // Oyun durumu de�i�ti�inde animasyonu g�ncelle
        if (wasGameRunning != GameManager.gameRunning)
        {
            if (animator != null)
            {
                animator.enabled = GameManager.gameRunning;
                if (GameManager.gameRunning)
                {
                    // Oyun devam etti�inde �nceki duruma g�re animasyonu ba�lat
                    if (isChasing)
                        animator.SetTrigger(runTrigger);
                    else
                        animator.SetTrigger(walkTrigger);
                }
            }
            wasGameRunning = GameManager.gameRunning;
        }

        if (!GameManager.gameRunning) return;

        if (player == null) return;

        // Player tespiti
        if (CanSeePlayer())
        {
            if (!isChasing)
            {
                isChasing = true;
                currentSpeed = chaseSpeed;
                // Ko�ma animasyonuna ge�
                if (animator != null)
                {
                    animator.SetTrigger(runTrigger);
                }
            }
            ChasePlayer();
        }
        else if (isChasing)
        {
            // Bir kere g�rd�kten sonra s�rekli kovalama modunda kal
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        // Player ile mesafe kontrol�
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < 1.5f) // Yak�n mesafe kontrol�
            {
                GameManager.gameRunning = false;
                PlayerRb.velocity = Vector3.zero;
                
                if (animator != null)
                {
                    animator.enabled = false;
                }
            }
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        // Mesafe kontrol�
        float distanceToPlayer = Vector3.Distance(eyeOrigin.position, player.position);
        if (distanceToPlayer > detectionRange) return false;

        // G�r�� a��s� kontrol�
        Vector3 directionToPlayer = (player.position - eyeOrigin.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > viewAngle) return false;

        // Engel kontrol�
        RaycastHit hit;
        if (Physics.Raycast(eyeOrigin.position, directionToPlayer, out hit, detectionRange, obstructionMask))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                return false;
            }
        }

        return true;
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        Transform targetWaypoint = patrolPoints[currentPatrolIndex];
        MoveTowards(targetWaypoint.position);

        // Waypoint'e ula��ld� m� kontrol et
        if (Vector3.Distance(transform.position, targetWaypoint.position) <= waypointReachedDistance)
        {
            
            
                currentPatrolIndex++;
                if (currentPatrolIndex >= patrolPoints.Count)
                {
                    currentPatrolIndex = 0;
                    
                }
            
            
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;
        MoveTowards(player.position);
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        // Y�n hesapla
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Y ekseninde hareket etme

        // Hareket
        transform.position += direction * currentSpeed * Time.deltaTime;

        // D�n��
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void StartPatrol()
    {
        // En yak�n patrol noktas�n� bul
        float minDistance = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < patrolPoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        currentPatrolIndex = closestIndex;
        
    }

    // Gizmos ile g�r�� alan�n� g�rselle�tir
    void OnDrawGizmosSelected()
    {
        if (eyeOrigin != null)
        {
            // Tespit mesafesi
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(eyeOrigin.position, detectionRange);

            // G�r�� a��s�
            Gizmos.color = Color.blue;
            Vector3 rightDir = Quaternion.Euler(0, viewAngle, 0) * transform.forward;
            Vector3 leftDir = Quaternion.Euler(0, -viewAngle, 0) * transform.forward;
            Gizmos.DrawRay(eyeOrigin.position, rightDir * detectionRange);
            Gizmos.DrawRay(eyeOrigin.position, leftDir * detectionRange);
        }
    }
}
