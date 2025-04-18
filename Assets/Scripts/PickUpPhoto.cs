using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpPhoto : MonoBehaviour
{
    
    public float interactionDistance = 4f;
    public Transform playerCamera; // Assign the player's camera here

    
    private bool grabbed = false;
    public GameObject Photo;

    public GameObject Player;
   



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !grabbed)
        {

            float distance = Vector3.Distance(playerCamera.position, transform.position);
            if (distance <= interactionDistance)
            {

                Ray ray = new Ray(playerCamera.position, playerCamera.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
                {
                    if (hit.transform == transform)
                    {
                        grabbed = true;
                        Photo.SetActive(true);
                        Player.GetComponent<Rigidbody>().isKinematic = true;
                        GameManager.gameRunning = false;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.E) && grabbed)
        {
            grabbed = false;
            Photo.SetActive(false);
            Player.GetComponent<Rigidbody>().isKinematic = false;
            GameManager.gameRunning = true;
        }
    }
}