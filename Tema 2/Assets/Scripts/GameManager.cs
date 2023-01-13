using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.Serializable]
    public class Entity
    {
        public string playerName;
        public Stone[] myStones;
        public bool hasTurn;
        public enum PlayerTypes
        {
            HUMAN,
            CPU,
            NO_PLAYER
        }
        public PlayerTypes playerType;
        public bool hasWon;
    }

    public List<Entity> playerList = new List<Entity>();


    //STATEMACHINE
    public enum States
    {
        WAITING,
        ROLL_DICE,
        SWITCH_PLAYER
    }

    public States state;

    public int activePlayer;

    bool switchingPlayer;
    bool turnPossible = true;
    //HUMANI INPUTS

    [HideInInspector]public int rolledHumanDice;


    public Dice dice;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ActivateButton(false);
    }

    void Update()
    {
        if (playerList[activePlayer].playerType == Entity.PlayerTypes.CPU)
        {
            switch (state)
            {
                case States.ROLL_DICE:
                    {
                        if(turnPossible)
                        {
                            StartCoroutine(RollDiceDelay());
                            state = States.WAITING;
                        }
                    }
                    break;
                case States.WAITING:
                    {
                        //IDLE
                    }
                    break;
                case States.SWITCH_PLAYER:
                    {
                        if (turnPossible)
                        {
                            StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }
                    break;
            }
        }

        if (playerList[activePlayer].playerType == Entity.PlayerTypes.HUMAN)
        {
            switch (state)
            {
                case States.ROLL_DICE:
                    {
                        if (turnPossible)
                        {
                            //deactivate highlight
                            ActivateButton(true);
                            state = States.WAITING;
                        }
                    }
                    break;
                case States.WAITING:
                    {
                        //IDLE
                    }
                    break;
                case States.SWITCH_PLAYER:
                    {
                        if (turnPossible)
                        {
                            //deactivate the button
                            //deactivate the highlights

                            StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }
                    break;
            }
        }
    }

    void CPUDice()
    {
        dice.RollDice();
    }

    public void RollDice(int _diceNumber)//call from the dice direct
    {
        int diceNumber = _diceNumber;
        //int diceNumber = 6;
        if (playerList[activePlayer].playerType == Entity.PlayerTypes.CPU)
        {
            if (diceNumber == 6)
            {
                //check the start node
                CheckStartNode(diceNumber);
            }
            else
            {
                //check for kick
                MoveAStone(diceNumber);

            }
        }

        if (playerList[activePlayer].playerType == Entity.PlayerTypes.HUMAN)
        {
            rolledHumanDice = _diceNumber;
            HumanRollDice();
        }
            Debug.Log("DICE ROLLED WITH NUMBER" + diceNumber);
    }
    IEnumerator RollDiceDelay()
    {
        yield return new WaitForSeconds(2);
        CPUDice();
    }

    void CheckStartNode(int diceNumber)
    {
        //IS ANYONE ON THE START NODE
        bool startNodeFull = false;

        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].currentNode == playerList[activePlayer].myStones[i].startNode)
            {
                startNodeFull = true;
                break;
            }
        }
        if (startNodeFull)
        {
            //MOVE A STONE
            Debug.Log("tHE START NODE IS FULL");
            MoveAStone(diceNumber);

        }
        else
        {
            //if at least one is inside base
            for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
            {
                if (!playerList[activePlayer].myStones[i].ReturnIsOut())
                {
                    //leave the base
                    playerList[activePlayer].myStones[i].LeaveBase();
                    state = States.WAITING;
                    return;
                }
            }

            MoveAStone(diceNumber);
        }
    }

    void MoveAStone(int diceNumber)
    {
        List<Stone> movableStones = new List<Stone>();
        List<Stone> moveKickStones = new List<Stone>();

        //FILL THE LISTS
        for(int i = 0; i< playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].ReturnIsOut())
            {
                //check for possible kick
                if(playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneId,diceNumber))
                {
                    moveKickStones.Add(playerList[activePlayer].myStones[i]);
                    continue;
                }

                //check for possible move
                if (playerList[activePlayer].myStones[i].CheckPossibleMove(diceNumber))
                {
                    movableStones.Add(playerList[activePlayer].myStones[i]);
                    
                }
            }
        }


        //PERFORM KICK IF POSSIBLE
        if(moveKickStones.Count>0)
        {
            int num = Random.Range(0, moveKickStones.Count);
            moveKickStones[num].StartTheMove(diceNumber, false);
            state = States.WAITING;
            return;
        }


        //PERFORM MOVE IF POSSIBLE
        if (movableStones.Count > 0)
        {
            bool isHuman = playerList[activePlayer].playerType == Entity.PlayerTypes.HUMAN;
            int num = Random.Range(0, movableStones.Count);
            movableStones[num].StartTheMove(diceNumber, isHuman);
            state = States.WAITING;
            return;
        }

        //NONE IS POSSIBLE  
        //SWITCH PLAYER
        state = States.SWITCH_PLAYER;
    }

    IEnumerator SwitchPlayer()
    {
        if(switchingPlayer)
        {
            yield break;
        }
        switchingPlayer = true;

        yield return new WaitForSeconds(1);
        //SET NEXT PLAYER
        SetNextActivePlayer();

        switchingPlayer = false;
    }
    
    void SetNextActivePlayer()
    {
        activePlayer++;
        activePlayer %= playerList.Count;

        int available = 0;
        for(int i = 0; i<playerList.Count;i++)
        {
            if(!playerList[i].hasWon)
            {
                available++;
            }
        }

        if(playerList[activePlayer].hasWon && available > 1)
        {
            SetNextActivePlayer();
            return;
        }
        else if(available < 2)
        {
            //game over screen
            state = States.WAITING;
            return;
        }
        state = States.ROLL_DICE;
    }

    public void ReportTurnPossible(bool possible)
    {
        turnPossible = possible;
    }

    public void ReportWinning()
    {
        //SHOW SOME UI
        playerList[activePlayer].hasWon = true;
    }

    //--------------------------------------------------------Human input--------------------------------------

    public void ActivateButton(bool on)
    {
        
    }

    public void DeactivateAllSelector()
    {
        for(int i = 0; i < playerList.Count;i++)
        {
            for(int j = 0; j < playerList[i].myStones.Length; j++)
            {
                playerList[i].myStones[j].SetSelector(false);
            }
        }
    }

    public void HumanRoll()
    {
        dice.RollDice();
    }

    public void HumanRollDice()
    {
        ActivateButton(false);

        //movable list

        List<Stone> movableStones = new List<Stone>();

        //start node check
        bool startNodeFull = false;

        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].currentNode == playerList[activePlayer].myStones[i].startNode)
            {
                startNodeFull = true;
                break;
            }
        }
        // number < 6
        if(rolledHumanDice < 6)
        {
            movableStones.AddRange(PossibleStones());
        }

        //number == 6 & !startnode
        if (rolledHumanDice == 6 && !startNodeFull)
        {
            //inside base check
            for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
            {
                if (!playerList[activePlayer].myStones[i].ReturnIsOut())
                {
                    movableStones.Add(playerList[activePlayer].myStones[i]);
                }
            }
            //outside check
            movableStones.AddRange(PossibleStones());
        }
        else if(rolledHumanDice == 6 && startNodeFull)
        {
            movableStones.AddRange(PossibleStones());
        }

        if (movableStones.Count > 0)
        {
            for (int i = 0; i < movableStones.Count; i++)
            {
                movableStones[i].SetSelector(true);
            }
        }
        else
        {
            state = States.SWITCH_PLAYER;
        }
    }

    List <Stone> PossibleStones()
    {
        List<Stone> tempList = new List<Stone>();

        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
      
            if (playerList[activePlayer].myStones[i].ReturnIsOut())
            {
                if (playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneId, rolledHumanDice))
                {
                    tempList.Add(playerList[activePlayer].myStones[i]);
                    continue;
                }
                if (playerList[activePlayer].myStones[i].CheckPossibleMove(rolledHumanDice))
                {
                    tempList.Add(playerList[activePlayer].myStones[i]);
                }
            }
        }

        return tempList;
    }
}
