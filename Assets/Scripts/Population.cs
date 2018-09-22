using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Population : MonoBehaviour{

    public GameObject player;
    public GameObject goal;
    private GameObject champion;    //player with best fitness score

    public static int playerNum = 100;
    public GameObject[] Players = new GameObject[playerNum];

    private Vector3 spawn1 = new Vector3(0.82f, 0.01f, -41.85f);
    private Vector3 spawn2 = new Vector3(68.5f, 1.00f, -15.3f);
    private Vector3 spawn3 = new Vector3(-6.4f, 0.85f, 1.64f);
    private Vector3 spawn4 = new Vector3(111f, 50.2f, -134.0f);

    public Text genCounter;

    private float fitnessSum;
    private float mutationRate = 0.02f;
    private int generation = 0;
    private int minStep = Player.size;  //minimum of steps taken to reach the goal


    private bool noWinnerBefore = true;
    private long k = 0; //frame counter
    private long n = 5;



    // Use this for initialization
    void Start(){

        Cursor.visible = false;
        SpawnPlayers();
        SetGenText();

    }

    // Update is called once per frame
    void FixedUpdate(){

        if(k % n == 0)  //update only once per n frames
        { 
        
            if (ReachedTheGoal())
            {
                if(noWinnerBefore)
                {
                    print("Success!!! The Winner was born in " + generation + " generation!");
                    noWinnerBefore = false;
                }
            }      

            if(AllDead())
            {
                NaturalSelection();
                champion.GetComponent<Player>().SetChampion();                   
                StartCoroutine(PauseAndRespawn()); // respawning needs to be embedded inside courotine coz delay doesn't work otherwise :/                   
            }
            else
            {
                for(int i = 0; i < playerNum; i++)
                {
                    if (Players[i].GetComponent<Player>().i > minStep) Players[i].GetComponent<Player>().Die();
                    else if (Players[i].GetComponent<Rigidbody>().velocity.magnitude > Players[i].GetComponent<Player>().maxSpeed) continue;

                    Players[i].GetComponent<Player>().MovePlayer();
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
        SetGenText();
        if (generation % 5 == 0) IncreaseLimit();   //every 5 generations expand their lifetime
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
        for (int i = 0; i < playerNum; i++)
        {
            if (Players[i].GetComponent<Player>().reachedTheGoal == true)
            {
                if (Players[i].GetComponent<Player>().i < minStep) minStep = Players[i].GetComponent<Player>().i;
                return true;
            }
        }
        return false;
    }


    void RespawnAll()
    {
        for (int i = 0; i < playerNum; i++)
        {
            Players[i].GetComponent<Player>().Respawn();
        }

        Players[0].GetComponent<Player>().SetChampion();
    }


    void IncreaseLimit()
    {
        for (int i = 0; i < playerNum; i++)
        {
            Players[i].GetComponent<Player>().limit += 5;
        }
    }


    void SetGenText()
    {
        genCounter.text = "Generation: " + generation.ToString();
    }


    void SpawnPlayers()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "level 3") n = 1;
        for (int i = 0; i < playerNum; i++)
        {

            //Spawn player
            GameObject player_x;
            switch (sceneName)
            {
                case "level 1":
                    player_x = Instantiate(player, spawn1, Quaternion.identity) as GameObject;
                    break;
                case "level 2":
                    player_x = Instantiate(player, spawn2, Quaternion.identity) as GameObject;
                    break;
                case "level 3":
                    player_x = Instantiate(player, spawn3, Quaternion.identity) as GameObject;
                    break;
                case "level 4":
                    player_x = Instantiate(player, spawn4, Quaternion.identity) as GameObject;
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
            Mutate(i);
        }
        
    }


    void CopyBrain(GameObject P1, GameObject P2)
    {
        for( int i = 0; i < Player.size; i++)
        {
            P1.GetComponent<Player>().brain[i][0] = P2.GetComponent<Player>().brain[i][0];
            P1.GetComponent<Player>().brain[i][1] = P2.GetComponent<Player>().brain[i][1];
            P1.GetComponent<Player>().brain[i][2] = P2.GetComponent<Player>().brain[i][2];
        }
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


    void Mutate(int x)
    {
        
        
        for (int i = 0; i < Players[x].GetComponent<Player>().limit; i++)
        {
            float rand = Random.Range(0.0f, 1.0f);
            if (rand < mutationRate)
            {
                Players[x].GetComponent<Player>().brain[i] = new Vector3(Random.Range(10, -11), 0, Random.Range(10, -11));
            }
        }
    }


    /*
       float SuccessRatio()    //returns number of players who reached the goal diveded by number of all players
       {
           int count = 0;
           for(int i = 0; i < playerNum; i++)
           {
               if (Players[i].GetComponent<Player>().reachedTheGoal) count++;
           }

           return (float)count / playerNum;
       }
   */

}
