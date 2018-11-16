using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Population : MonoBehaviour{

    public GameObject player;
    public GameObject goal;
    private GameObject champion;    //player with best fitness score

    public static int playerNum;    //number of players
    public GameObject[] Players;

    //spawn positions for each level
    private Vector3 spawn1 = new Vector3(0.82f, 0.01f, -41.85f);
    private Vector3 spawn2 = new Vector3(68.5f, 1.00f, -15.3f);
    private Vector3 spawn3 = new Vector3(111f, 50.2f, -134.0f);


    private float fitnessSum;
    private float mutationRate = 0.02f; //0.02 means only 2% of movement vectors will be modified for each new player
    private int minStep = Player.brainSize;  //minimum of steps taken to reach the goal
    public int generation = 0;

    private bool jumpingEnabled;
    private bool noWinnerBefore = true;
    private long k = 0; //frame counter



    // Use this for initialization
    void Start(){
        playerNum = PauseMenu.playerNum; //number of players is taken from other script because it enables changing this value in game by user
        Players = new GameObject[playerNum];

        jumpingEnabled = PauseMenu.jumpingEnabled;  //jumping can be enabled through game menu

        SpawnPlayers();
    }

    // Update is called once per frame
    void FixedUpdate(){

        if(k % 5 == 0)  //update only once per 5 frames
        {
            k = 0;
            if (ReachedTheGoal())
            {
                if(noWinnerBefore)
                {
                    print("Success!!! The Winner was born in " + generation + " generation!");
                    noWinnerBefore = false;
                }
            }      

            if(AllDead())   //if everyone is dead their score is evaluated, the champion is colored green and game pauses for 1 second
            {
                NaturalSelection();
                champion.GetComponent<Player>().SetAsChampion();                   
                StartCoroutine(PauseAndRespawn()); // respawning needs to be embedded inside coroutine coz pause doesn't work otherwise :/                   
            }
            else
            {
                for(int i = 0; i < playerNum; i++)
                {
                    if (Players[i].GetComponent<Player>().i > minStep)
                    {
                        Players[i].GetComponent<Player>().Die(); //player is killed if it has already taken more steps then the best player that reached the goal (that way they stil learn to optimise their moves to reach the goal faster)
                    }
                    else if (Players[i].GetComponent<Rigidbody>().velocity.magnitude < Player.maxSpeed)  //movement vector is applied only if player hasn't crossed max speed limit 
                    {
                        Players[i].GetComponent<Player>().MovePlayer();
                    }

                }
            }

        }
        k++;

    }



    IEnumerator PauseAndRespawn()
    {
        enabled = false;    //turn off update function
        yield return new WaitForSeconds(1.0f);  //pause
        enabled = true;     //turn on update function

        RespawnAll();
        generation++;

        if (generation % 5 == 0) IncreaseLifespan();   //every 5 generations expand their lifetime
    }


    bool AllDead()
    {
        for (int i = 0; i < playerNum; i++)
        {
            if (Players[i].GetComponent<Player>().dead == false) return false;
        }
        return true;
    }


    bool ReachedTheGoal()   //returns true if any player reached the goal
    {
        bool rechedTheGoal = false;
        for (int i = 0; i < playerNum; i++)
        {
            if (Players[i].GetComponent<Player>().reachedTheGoal == true)
            {
                if (Players[i].GetComponent<Player>().i < minStep)
                {
                    minStep = Players[i].GetComponent<Player>().i;
                }            
                rechedTheGoal = true;
            }
        }

        if(rechedTheGoal) return true;
        else return false;
    }


    void RespawnAll()
    {
        for (int i = 0; i < playerNum; i++)
        {
            Players[i].GetComponent<Player>().Respawn();
        }

        Players[0].GetComponent<Player>().SetAsChampion();    //makes the best player from previous generation to be green
    }


    void SetChampion()
    {
        float best_score = Vector3.Distance(Players[0].transform.position, goal.transform.position);
        champion = Players[0];

        for (int i = 1; i < playerNum; i++)
        {
            float DistanceToGoal = Vector3.Distance(Players[i].transform.position, goal.transform.position);
            if (DistanceToGoal < best_score)
            {
                best_score = DistanceToGoal;
                champion = Players[i];
            }
        }
    }


    void IncreaseLifespan()
    {
        for (int i = 0; i < playerNum; i++)
        {
            Players[i].GetComponent<Player>().lifespan += 5;
        }
    }


    void SpawnPlayers()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < playerNum; i++)
        {
            //Spawn player
            GameObject player_x;
            switch (sceneName)
            {
                case "Level1":
                    player_x = Instantiate(player, spawn1, Quaternion.identity) as GameObject;
                    break;
                case "Level2":
                    player_x = Instantiate(player, spawn2, Quaternion.identity) as GameObject;
                    break;
                case "Level3":
                    player_x = Instantiate(player, spawn3, Quaternion.identity) as GameObject;
                    break;
                default :
                    player_x = Instantiate(player) as GameObject;
                    break;
            }
            //player_x.transform.localScale = new Vector3(2, 2, 2); // set size of player

            //Generate name for player
            string[] name_tmp = { "player", (i + 1).ToString() };
            name = string.Join("", name_tmp, 0, 2);
            player_x.name = name;

            //Assign player to array
            Players[i] = player_x;

            //Calculate distance from spawn to goal
            Players[i].GetComponent<Player>().distToGoalFromSpawn = Vector3.Distance(Players[i].transform.position, goal.transform.position);

        }
    }


    void NaturalSelection()
    {
        SetChampion();

        CalculateFitness();
        CalculateFitnessSum();

        CopyBrain(Players[0], champion);    //Champion is always reborn in next generation unchanged

        for (int i = 1; i < playerNum; i++)
        {
            GameObject parent = SelectParent();
            CopyBrain(Players[i], parent);
            Mutate(Players[i]);
        }
        
    }


    void CopyBrain(GameObject P1, GameObject P2)
    {
        for( int i = 0; i < Player.brainSize; i++)
        {
            P1.GetComponent<Player>().brain[i][0] = P2.GetComponent<Player>().brain[i][0];
            P1.GetComponent<Player>().brain[i][1] = P2.GetComponent<Player>().brain[i][1];
            P1.GetComponent<Player>().brain[i][2] = P2.GetComponent<Player>().brain[i][2];
        }
    }



    void CalculateFitness()
    {
        for (int i = 0; i < playerNum; i++)
        {
            float DistanceToGoal = Vector3.Distance(Players[i].transform.position, goal.transform.position);

            if(Players[i].GetComponent<Player>().reachedTheGoal)
            {
                int step = Players[i].GetComponent<Player>().i;
                float distToGoalFromSpawn = Players[i].GetComponent<Player>().distToGoalFromSpawn;
                Players[i].GetComponent<Player>().fitness = 1.0f / 24 + distToGoalFromSpawn * 100 / (step * step);
            }
            else
            {
                Players[i].GetComponent<Player>().fitness = 10.0f / (DistanceToGoal * DistanceToGoal * DistanceToGoal * DistanceToGoal);
            }
        }
    }


    void CalculateFitnessSum()
    {
        fitnessSum = 0;
        for (int i = 0; i < playerNum; i++)
            fitnessSum += Players[i].GetComponent<Player>().fitness;
    }


    GameObject SelectParent()
    {
        float rand = Random.Range(0.0f, fitnessSum);
        float runningSum = 0;

        for(int i = 0; i < playerNum; i++)
        {
            runningSum += Players[i].GetComponent<Player>().fitness;
            if (runningSum >= rand)
            {
                return Players[i];
            }
        }

        return null; //should never come to this
    }


    void Mutate(GameObject PlayerX)
    {    
        for (int i = 0; i < PlayerX.GetComponent<Player>().lifespan; i++)
        {
            float rand = Random.Range(0.0f, 1.0f);
            if (rand < mutationRate)
            {
                PlayerX.GetComponent<Player>().brain[i] = new Vector3(Random.Range(10, -11), 0, Random.Range(10, -11));

                //they also get y axis force if jumping is enabled
                if(jumpingEnabled)
                {
                    PlayerX.GetComponent<Player>().brain[i][1] = Random.Range(10, -11);
                }
            }
        }
    }

}
