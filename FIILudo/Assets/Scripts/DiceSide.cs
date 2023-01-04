using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSide : MonoBehaviour
{
    bool onGround;
    public int sideValue;

    private void OnTriggerStay(Collider col)
    {
        if(col.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    public bool OnGround()
    {
        return onGround;
    }
}
