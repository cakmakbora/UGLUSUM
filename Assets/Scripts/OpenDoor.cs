using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{

    public Transform obj1;
    public Transform obj2;
    public Quaternion targetRotation1;
    public Quaternion targetRotation2;
    public float rotationSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        targetRotation1 = Quaternion.Euler(0, -55, 0);
        targetRotation2 = Quaternion.Euler(0, -125, 0);
    }

    // Update is called once per frame
    void Update()
    {
        obj1.rotation = Quaternion.Lerp(obj1.rotation, targetRotation1, Time.deltaTime * rotationSpeed);
        obj2.rotation = Quaternion.Lerp(obj2.rotation, targetRotation2, Time.deltaTime * rotationSpeed);
    }
}
