using UnityEngine;
using System.Collections.Generic;

public class PatrolBehaviour : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float rayDist = 1f;
    public Transform groundDetect;
    public string groundTag = "Ground";

    [Header("Patrol Settings")]
    public List<Transform> patrolPoints; // List of points to patrol between
    public float waypointReachedDistance = 0.5f; // How close to get to a waypoint before moving to next
    private int currentPatrolIndex = 0;
    private bool isMovingForward = true;

    void Start()
    {
        // Validate patrol points
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("No patrol points assigned! Character will not move.");
            return;
        }

        // Set initial position to first patrol point
        transform.position = patrolPoints[0].position;
    }

    void Update()
    {
        // Check if game is running
        if (!GameManager.gameRunning)
            return;

        if (patrolPoints == null || patrolPoints.Count == 0)
            return;

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
            if (isMovingForward)
            {
                currentPatrolIndex++;
                if (currentPatrolIndex >= patrolPoints.Count)
                {
                    currentPatrolIndex = patrolPoints.Count - 2;
                    isMovingForward = false;
                }
            }
            else
            {
                currentPatrolIndex--;
                if (currentPatrolIndex < 0)
                {
                    currentPatrolIndex = 1;
                    isMovingForward = true;
                }
            }
        }
    }

    // Draw gizmos to visualize patrol path
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
    }
}

