using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public int stoneId;
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


    [Header("Selector")]
    public GameObject selector;

    //ARC MOVEMENT
    float amplitude = 60f;
    float cTime = 0f;

    void Start()
    {
        startNodeIndex = commonRoute.RequestPosition(startNode.gameObject.transform);
        CreateFullRoute();

        SetSelector(false);
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

    IEnumerator Move(int diceNumber, bool isHuman)
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
            Vector3 startPos = fullRoute[routePosition - 1].gameObject.transform.position;
            //while (MoveToNextNode(nextPos, 140f)) { yield return null; }
            //while (MoveInArcToNextNode(startPos, nextPos, 8f)){ yield return null; }

            Debug.Log(isHuman);

            if(!isHuman) //CPU
            {
                while (MoveInArcToNextNode(startPos, nextPos, 8f)) { yield return null; }
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                while (MoveInArcToNextNode(startPos, nextPos, 1000f)) { yield return null; }
            }

            cTime = 0;
            steps--;
            doneSteps++;
        }
        goalNode = fullRoute[routePosition];
        //check possible kick
        if(goalNode.isTaken)
        {
            //kick the other stone
            goalNode.stone.ReturnToBase();
        }

        currentNode.stone = null;
        currentNode.isTaken = false;

        goalNode.stone = this;
        goalNode.isTaken = true;

        currentNode = goalNode;
        goalNode = null;

        //wincondition check
        if(WinCondition())
        {
            GameManager.instance.ReportWinning();
        }


        //report to gamemanager
        //switch the player
        if(diceNumber <6)
        {
            GameManager.instance.state = GameManager.States.SWITCH_PLAYER;
        }
        else
        {
            GameManager.instance.state = GameManager.States.ROLL_DICE;
        }

        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goalPos, float speed)
    {
        return goalPos != (transform.position = Vector3.MoveTowards(transform.position, goalPos, speed * Time.deltaTime));
    }

    bool MoveInArcToNextNode(Vector3 startPos,Vector3 goalPos,float speed)
    {
        cTime += speed * Time.deltaTime;
        Vector3 myPosition = Vector3.Lerp(startPos, goalPos, cTime);

        myPosition.y += amplitude * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);

        return goalPos != (transform.position = Vector3.Lerp(transform.position, myPosition, cTime));
    }

    public bool ReturnIsOut()
    {
        return isOut;
    }

    public void LeaveBase()
    {
        steps = 1;
        isOut = true;
        routePosition = 0;
        //Start coroutine

        StartCoroutine(MoveOut());
    }

    IEnumerator MoveOut()
    {
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;

        while (steps > 0)
        {
            //routePosition++;

            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            Vector3 startPos = baseNode.gameObject.transform.position;
            //while (MoveToNextNode(nextPos, 140f)) { yield return null; }
            while (MoveInArcToNextNode(startPos, nextPos, 8f)) { yield return null; }
            yield return new WaitForSeconds(0.1f);
            cTime = 0;
            steps--;
            doneSteps++;
        }
        //update node
        goalNode = fullRoute[routePosition];
        //check for kicking stone
        if(goalNode.isTaken)
        {
            goalNode.stone.ReturnToBase();
        }

        goalNode.stone = this;
        goalNode.isTaken = true;

        currentNode = goalNode;
        goalNode = null;

        GameManager.instance.state = GameManager.States.ROLL_DICE;
        isMoving = false;
    }

    public bool CheckPossibleMove(int diceNumber)
    {
        int tempPos = routePosition + diceNumber;
        if(tempPos >= fullRoute.Count)
        {
            return false;
        }
        return !fullRoute[tempPos].isTaken;
    }

    public bool CheckPossibleKick(int stoneId, int diceNumber)
    {
        int tempPos = routePosition + diceNumber;
        if (tempPos >= fullRoute.Count)
        {
            return false;
        }
        if(fullRoute[tempPos].isTaken)
        {
            if(stoneId == fullRoute[tempPos].stone.stoneId)
            {
                return false;
            }
            return true;
        }
        return false;
    }


    public void StartTheMove(int diceNumber, bool isHuman)
    {
        steps = diceNumber;
        StartCoroutine(Move(diceNumber, isHuman));
    }


    public void ReturnToBase()
    {
        StartCoroutine(Return());
    }

    IEnumerator Return()
    {
        GameManager.instance.ReportTurnPossible(false);
        routePosition = 0;
        currentNode = null;
        goalNode = null;
        isOut = false;
        doneSteps = 0;

        Vector3 baseNodePosition = baseNode.gameObject.transform.position;
        while (MoveToNextNode(baseNodePosition, 1000f)) { yield return null; }
        GameManager.instance.ReportTurnPossible(true);
    }

    bool WinCondition()
    {
        for (int i = 0; i < finalRoute.childNodeList.Count; i++)
        {
            if (!finalRoute.childNodeList[i].GetComponent<Node>().isTaken)
            {
                return false;
            }
        }
        return true;
    }

    //---------------------------------------HUMAN INPUT----------------------------------------

    public void SetSelector(bool on)
    {
        selector.SetActive(on);
        hasTurn = on;
    }

   

    private void OnMouseDown()
    {
        if(hasTurn)
        {
            if(!isOut)
            {
                LeaveBase();
            }
            else
            {
                StartTheMove(GameManager.instance.rolledHumanDice, true);
            }
            GameManager.instance.DeactivateAllSelector();
        }
        
    }
}
