using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public SpawnManager spawnManager;

    public Animator mAnimator;


    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

            mAnimator.SetTrigger("trigger_attack");
            

            for (int i = 0; i < SpawnManager.ObjectsList.Capacity - 1; i++) {
                for (int j = i+1; j < SpawnManager.ObjectsList.Capacity; j++)
                {
                    mAnimator.SetTrigger("trigger_attack");
            }
        }
        
    }
}
