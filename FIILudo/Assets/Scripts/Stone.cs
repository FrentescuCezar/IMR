using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    [Header("Routes")]
    public Route commonRoute;
    public Route finalRoute;

    public List<Node> fullRoute = new List<Node>();
    [Header("Nodes")]
    public Node startNode;
    public Node baseNode;
    public Node currentNode;
    public Node goalNode;

    int routePosition;
    int startNodeIndex;

    int steps;//dice
    int doneSteps;
    [Header("BOOLS")]
    public bool isOut;
    bool isMoving;

    bool hasTurn;

    [Header("SELECTOR")]
    public GameObject selector;

    void Start()
    {
        startNodeIndex = commonRoute.RequestPosition(startNode.gameObject.transform);
        CreateFullRoute();
    }

    void CreateFullRoute()
    {
        for(int i = 0; i < commonRoute.childNodeList.Count; i++)
        {
            int tempPos = startNodeIndex + i;
            tempPos %= commonRoute.childNodeList.Count;

            fullRoute.Add(commonRoute.childNodeList[tempPos].GetComponent<Node>());
            
        }

        for( int i = 0; i < finalRoute.childNodeList.Count;i++)
        {
            fullRoute.Add(finalRoute.childNodeList[i].GetComponent<Node>());
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isMoving )
        {
            steps = Random.Range(1, 7); // 7 is never ALES by Random.Range
            Debug.Log("Dice number = " + steps);
            if(doneSteps + steps < fullRoute.Count)
            {
                StartCoroutine(Move());
            }
            else
            {
                Debug.Log("Number is to high");
            }

        }
    }

    IEnumerator Move()
    {
        if(isMoving)
        {
            yield break;
        }
        isMoving = true;

        while(steps>0)
        {
            routePosition++;

            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            while (MoveToNextNode(nextPos, 90f)) { yield return null; }
            yield return new WaitForSeconds(0.1f);
            steps--;
            doneSteps++;
        }

        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goalPos, float speed)
    {
        return goalPos != (transform.position = Vector3.MoveTowards(transform.position, goalPos, speed * Time.deltaTime));
    }

}
