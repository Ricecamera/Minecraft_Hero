using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Vector3 velocity;
    private Rigidbody myRigidbody;

    [SerializeField]
    private float xBound = 26.6f;

    [SerializeField]
    private float zBound = 15.0f;

    private Vector3 CheckOutofBound(Vector3 targetPosition)
    {
        if (targetPosition.x > xBound)
        {
            targetPosition.x = xBound;
        }
        else if (targetPosition.x < -xBound)
        {
            targetPosition.x = -xBound;
        }

        if (targetPosition.z > zBound)
        {
            targetPosition.z = zBound;
        }
        else if (targetPosition.z < -zBound)
        {
            targetPosition.z = -zBound;
        }

        return targetPosition;
    }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var tragetPosition = myRigidbody.position + this.velocity * Time.fixedDeltaTime;
        tragetPosition = CheckOutofBound(tragetPosition);
        myRigidbody.MovePosition(tragetPosition);
    }

    public void Move(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public void LookAt(Vector3 target)
    {
        target.y = transform.position.y; // Normalize y component
        transform.LookAt(target);
    }

    public void Reset()
    {
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
    }


}
