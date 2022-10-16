using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class DartRaycast : MonoBehaviour
{

    private bool hasHit;
    private Grabbable theGrab;
    private Rigidbody rb;

    void Start()
    {
        hasHit = true;
        var theDart = GameObject.FindWithTag("RayCastPoint").transform;
        theGrab = theDart.GetComponent<Grabbable>();
        rb = theDart.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!hasHit && !theGrab.BeingHeld) ShootRaycast();

        if (!theGrab.BeingHeld) {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (theGrab.BeingHeld) {
            hasHit = false;
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    private void ShootRaycast() {
        hasHit = Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 0.2f);
    }

}
