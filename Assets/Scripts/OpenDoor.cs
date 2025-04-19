using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public Transform obj1;
    public Transform obj2;
    public float rotationSpeed = 2f;
    public float interactionDistance = 3f;
    public Transform playerCamera; // Assign the player's camera here

    private Quaternion targetRotation1;
    private Quaternion targetRotation2;
    private bool isOpening = false;

    void Start()
    {
        // Store the original local rotations
        Quaternion initialRotation1 = obj1.localRotation;
        Quaternion initialRotation2 = obj2.localRotation;

        // Rotate outward: left door goes negative, right door goes positive
        targetRotation1 = initialRotation1 * Quaternion.Euler(0, -55, 0); // left door opens left
        targetRotation2 = initialRotation2 * Quaternion.Euler(0, 55, 0);  // right door opens right
    }

    void Update()
    {
        if (!isOpening)
        {
            // Check if player presses E
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Check distance to door
                float distance = Vector3.Distance(playerCamera.position, transform.position);
                if (distance <= interactionDistance)
                {
                    // Check if player is looking at this door
                    Ray ray = new Ray(playerCamera.position, playerCamera.forward);
                    if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
                    {
                        if (hit.transform == transform)
                        {
                            isOpening = true;
                        }
                    }
                }
            }
        }

        if (isOpening)
        {
            obj1.localRotation = Quaternion.Lerp(obj1.localRotation, targetRotation1, Time.deltaTime * rotationSpeed);
            obj2.localRotation = Quaternion.Lerp(obj2.localRotation, targetRotation2, Time.deltaTime * rotationSpeed);
        }
    }
}
