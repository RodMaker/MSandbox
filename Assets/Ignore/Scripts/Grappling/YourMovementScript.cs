using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YourMovementScript : MonoBehaviour
{
    public Rigidbody rb;

    public bool freeze;

    void Update()
    {
        if (freeze)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
