using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public Vector3 spawn;
    public static float moveSpeed = 40;
    public static float maxSpeed = 8.0f;

    public static int brainSize = 300;
    public Vector3[] brain = new Vector3[brainSize];   //brain stores vectors of movement
    public int i = 0;   //brain iterator
    public int lifespan =  15;    //number of steps it can take before it dies (increased each 5 generations by population.cs script)

    public bool reachedTheGoal = false;
    public bool dead = false;

    public Material alive;
    public Material notAlive;
    public Material champion;
    public Shader alwaysOnTop;  //this shader makes champion visible over other players

    public float fitness = 0;

    private float distToGround;
    public float distToGoalFromSpawn;
    private string sceneName;



    // Use this for initialization
    void Start (){
        GenerateVectors();
        spawn = transform.position;
        distToGround = GetComponent<Collider>().bounds.extents.y;
        sceneName = SceneManager.GetActiveScene().name;
    }


    // Unity method for physics update
    void FixedUpdate(){

        if(!dead)
        {
            if(sceneName == "Level3")   //the bounds in which player stays alive are hardcoded for Level 3
            {
                if (transform.position.y < 40) Die();
                if (transform.position.z > -120 || transform.position.z < -150) Die();
            }
            else if (!IsGrounded()) Die();  //on other levels they die when there's no ground under them

            if (i >= brainSize || i >= lifespan) Die();
        }

    }


    public void MovePlayer()
    {
        if (!dead)
        {
            GetComponent<Rigidbody>().AddForce(brain[i] * moveSpeed);
            i++;
           
        }
    }



    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Goal")
        {
            reachedTheGoal = true;
            Die();
        }
    }


    public void Respawn()
    {
        transform.position = spawn;
        GetComponent<Rigidbody>().velocity = Vector3.zero; //stop movement in case it was falling while being dead or sth idk
        i = 0;
        dead = false;
        reachedTheGoal = false;
        GetComponent<Renderer>().material = alive;
    }


    public void Die()
    {
        dead = true;
        GetComponent<Renderer>().material = notAlive;
        GetComponent<Rigidbody>().velocity = Vector3.zero; //stop movement
    }


    public void SetAsChampion()
    {
        GetComponent<Renderer>().material = champion;
        GetComponent<Renderer>().material.shader = alwaysOnTop;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 15.0f);
    }


     private void GenerateVectors()
    {
       for (int j = 0; j < brainSize; j++)
            brain[j] = new Vector3(Random.Range(-10, 11), 0, Random.Range(10, -11));
    }

}
