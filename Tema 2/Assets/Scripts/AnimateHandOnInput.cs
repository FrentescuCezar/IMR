using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AnimateHandOnInput : MonoBehaviour
{

    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;

    float time;
    float timeDelay;

    void Start()
    {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float gripValue = gripAnimationAction.action.ReadValue<float>();

        if (gripValue == 1)
        {
            time = time + (float)11.0 * Time.deltaTime;
            handAnimator.SetFloat("Grip", time);
            if (time >= 1)
                time = 1;
        }
        else
        {
            if(time>0)
                time = time - (float)11.0 * Time.deltaTime; ;
            handAnimator.SetFloat("Grip", time);

        }




    }

}
