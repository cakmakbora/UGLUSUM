using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    private float walkSpeed = 3f;
    private float runSpeed = 5f;
    private float jumpForce = 5f;
    private float currentSpeed = 0f;
    private float speedSmoothTime = 0.2f;
    private float speedSmoothVelocity;

    [Header("Mouse Look Settings")]
    private float mouseSensitivity = 2f;
    public Transform playerCamera;
    private float xRotation = 0f;
    private float yRotation = 0f;

    [Header("Others")]
    private bool closed = true;
    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 initialCamRotation = playerCamera.localEulerAngles;
        xRotation = initialCamRotation.x;

        Invoke(nameof(enableLook), 0.5f);
    }

  
    void Update()
    {
        // Check if game is running
        if (!GameManager.gameRunning)
            return;

        if (!closed)
        {
            LookAround();
        }

        Move();

        Jump();


    }

    private void enableLook()
    {
        closed = false;
    }

    private void LookAround()
    {


        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

    }

    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 newVelocity = new Vector3(move.x * currentSpeed, rb.velocity.y, move.z * currentSpeed);
        rb.velocity = newVelocity;
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
       
        return Physics.Raycast(transform.position, Vector3.down, 1.5f);
    }


}
