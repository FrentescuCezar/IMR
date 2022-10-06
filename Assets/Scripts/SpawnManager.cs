using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    static public List<GameObject> ObjectsList = new List<GameObject>();


    public GameObject objectToSpawn;
    public PlacementManager pcm;
    // Start is called before the first frame update
    void Start()
    {
        pcm = FindObjectOfType<PlacementManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            GameObject obj = Instantiate(objectToSpawn, pcm.transform.position, pcm.transform.rotation * Quaternion.Euler(0, 180, 0));
            ObjectsList.Add(obj);
        }
    }
}
 