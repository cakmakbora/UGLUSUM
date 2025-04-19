using UnityEngine;
using System.Collections.Generic;

public class PatrolBehaviourSquare : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rayDist = 1f;
    public Transform groundDetect;
    public string groundTag = "Ground";

    [Header("Patrol Settings")]
    public List<Transform> patrolPoints; // List of points to patrol between
    public float waypointReachedDistance = 0.5f; // How close to get to a waypoint before moving to next
    private int currentPatrolIndex = 0;
    

    [Header("Detection Settings")]
    public float viewAngle = 44f; // 4 derecelik görüþ açýsý
    public float viewDistance = 10f; // Görüþ mesafesi
    public LayerMask playerMask; // Inspector'da Player layer'ýný seçin
    public Transform eyeOrigin; // Göz pozisyonu (karakterin gözleri)
    public bool drawVisionGizmos = true;
    public LayerMask obstructionMask; // Assign this in Inspector to include walls, terrain, etc.

    private bool hadiseayak = true;
    private bool closed = true;


    public GameObject Player;
    public Rigidbody PlayerRb;

    public Rigidbody Rb;

    public Animator animator;
    void Start()
    {
        Rb = GetComponent<Rigidbody>();
        PlayerRb = Player.GetComponent<Rigidbody>();
        // Validate patrol points
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("No patrol points assigned! Character will not move.");
            return;
        }

        // Set initial position to first patrol point
        transform.position = patrolPoints[0].position;

        Invoke(nameof(enableLook), 0.5f);
    }

    private void enableLook()
    {
        closed = false;
    }

    void Update()
    {
        // Check if game is running
        if (!GameManager.gameRunning)
            return;

        if (patrolPoints == null || patrolPoints.Count == 0)
            return;

        // Patrol hareketi
        PatrolMovement();

        // Player tespiti
        DetectPlayer();
    }

    private void PatrolMovement()
    {
        // Get current target waypoint
        Transform targetWaypoint = patrolPoints[currentPatrolIndex];

        // Calculate direction to waypoint
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        direction.y = 0; // Keep movement on the XZ plane

        // Move towards the waypoint
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Rotate to face movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        // Check if reached the current waypoint
        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distanceToWaypoint <= waypointReachedDistance)
        {
            // Move to next waypoint
            
                currentPatrolIndex++;
                if (currentPatrolIndex >= patrolPoints.Count)
                {
                    currentPatrolIndex = 0;
                }
            
            
        }
    }

    private void DetectPlayer()
    {
        if (hadiseayak)
        {
            if (closed) return;

            Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewDistance, playerMask);

            foreach (Collider target in targetsInView)
            {
                Vector3 directionToTarget = (target.transform.position - eyeOrigin.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget <= viewAngle)
                {
                    Debug.DrawRay(eyeOrigin.position, directionToTarget * viewDistance, Color.red);

                    if (Physics.Raycast(eyeOrigin.position, directionToTarget, out RaycastHit hit, viewDistance))
                    {
                        if (((1 << hit.collider.gameObject.layer) & obstructionMask) != 0)
                        {
                            Debug.Log("View blocked by: " + hit.collider.name);
                        }
                        else if (((1 << hit.collider.gameObject.layer) & playerMask) != 0)
                        {
                            Debug.Log("Player detected (clear line of sight)!");
                            GameManager.gameRunning = false;
                            PlayerRb.velocity = Vector3.zero;
                            if (animator != null)
                            {
                                animator.enabled = false;
                            }
                            hadiseayak = false;
                        }
                    }
                }
            }

        }

    }

    // Draw gizmos to visualize patrol path and view angle
    void OnDrawGizmos()
    {
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            Gizmos.color = Color.blue;

            // Draw lines between patrol points
            for (int i = 0; i < patrolPoints.Count - 1; i++)
            {
                if (patrolPoints[i] != null && patrolPoints[i + 1] != null)
                {
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                }
            }

            // Draw spheres at patrol points
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.3f);
                }
            }
        }

        // Draw ground detection ray
        if (groundDetect != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(groundDetect.position, Vector3.down * rayDist);
        }

        // Draw view angle
        if (eyeOrigin != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 rightDir = Quaternion.Euler(0, viewAngle, 0) * transform.forward;
            Vector3 leftDir = Quaternion.Euler(0, -viewAngle, 0) * transform.forward;
            Gizmos.DrawRay(eyeOrigin.position, rightDir * viewDistance);
            Gizmos.DrawRay(eyeOrigin.position, leftDir * viewDistance);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.gameRunning = false;


            if (animator != null)
            {
                animator.enabled = false;
            }

            Rb.isKinematic = true;
            PlayerRb.isKinematic = true;
        }
    }
}

