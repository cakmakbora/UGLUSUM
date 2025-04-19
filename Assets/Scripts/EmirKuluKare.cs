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
        // Player'ı bul
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Animator kontrolü
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Patrol noktalarını kontrol et
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("No patrol points assigned! Character will not move.");
            return;
        }

        // Başlangıcı pozisyonunu ayarla
        transform.position = patrolPoints[0].position;

        // Başlangıta yürüme animasyonunu başlat
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
        // Oyun durumu değiştiğinde animasyonu güncelle
        if (wasGameRunning != GameManager.gameRunning)
        {
            if (animator != null)
            {
                animator.enabled = GameManager.gameRunning;
                if (GameManager.gameRunning)
                {
                    // Oyun devam ettiğinde önceki duruma göre animasyonu başlat
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
                // Koşma animasyonuna geç
                if (animator != null)
                {
                    animator.SetTrigger(runTrigger);
                }
            }
            ChasePlayer();
        }
        else if (isChasing)
        {
            // Bir kere gördükten sonra sürekli kovalama modunda kal
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        // Player ile mesafe kontrolü
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < 2f) // Yakın mesafe kontrolü
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

        // Mesafe kontrolü
        float distanceToPlayer = Vector3.Distance(eyeOrigin.position, player.position);
        if (distanceToPlayer > detectionRange) return false;

        // Görüş açısı kontrolü
        Vector3 directionToPlayer = (player.position - eyeOrigin.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > viewAngle) return false;

        // Debug çizgileri
        Debug.DrawRay(eyeOrigin.position, directionToPlayer * detectionRange, Color.red);
        Debug.DrawRay(eyeOrigin.position, transform.forward * detectionRange, Color.blue);

        // Engel kontrolü - daha detaylı debug
        RaycastHit hit;
        bool hasHit = Physics.Raycast(eyeOrigin.position, directionToPlayer, out hit, detectionRange, obstructionMask);
        
        if (hasHit)
        {
            // Raycast'in neye çarptığını göster
            Debug.Log($"Raycast hit: {hit.collider.name} at distance {hit.distance:F2}");
            
            // Eğer çarptığı şey oyuncu değilse
            if (!hit.collider.CompareTag("Player"))
            {
                // Oyuncuya olan mesafe ile raycast mesafesini karşılaştır
                if (hit.distance < distanceToPlayer)
                {
                    Debug.Log($"Obstacle blocking view: {hit.collider.name} at {hit.distance:F2} units");
                    return false;
                }
            }
        }

        // Oyuncuyu gördüğünde debug mesajı
        Debug.Log($"EmirKulu sees player! Distance: {distanceToPlayer:F2}, Angle: {angleToPlayer:F2}");

        return true;
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        Transform targetWaypoint = patrolPoints[currentPatrolIndex];
        MoveTowards(targetWaypoint.position);

        // Waypoint'e ulaşldı mı kontrol et
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
        // Yön hesapla
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Y ekseninde hareket etme

        // Hareket
        transform.position += direction * currentSpeed * Time.deltaTime;

        // Dönü
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void StartPatrol()
    {
        // En yakın patrol noktasını bul
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

    // Gizmos ile görüş alanını görselleştir
    void OnDrawGizmosSelected()
    {
        if (eyeOrigin != null)
        {
            // Tespit mesafesi
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(eyeOrigin.position, detectionRange);

            // Görüş açısı
            Gizmos.color = Color.blue;
            Vector3 rightDir = Quaternion.Euler(0, viewAngle, 0) * transform.forward;
            Vector3 leftDir = Quaternion.Euler(0, -viewAngle, 0) * transform.forward;
            Gizmos.DrawRay(eyeOrigin.position, rightDir * detectionRange);
            Gizmos.DrawRay(eyeOrigin.position, leftDir * detectionRange);
        }
    }
}
