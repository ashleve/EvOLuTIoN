using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public Vector3 spawn;
    public float moveSpeed = 8;
    public float maxSpeed = 8.0f;

    public static int size = 300;
    public Vector3[] brain = new Vector3[size];   //brain stores vectors of movement
    public int i = 0;
    public int limit =  15;    //number of steps it can take before it dies

    public bool reachedTheGoal = false;
    public bool dead = false;

    public Material alive;
    public Material notAlive;
    public Material champion;
    public Shader AlwaysOnTop;

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


    // Update is called once per frame
    void Update(){

        if(!dead)
        {
            if(sceneName == "level 4")
            {
                if (transform.position.y < 40) Die();
                if (transform.position.z > -120 || transform.position.z < -150) Die();
            }
            else if (!IsGrounded()) Die();
        }

    }


    public void MovePlayer()
    {
        if (!dead)
        {
            GetComponent<Rigidbody>().AddForce(brain[i] * moveSpeed);
            i++;
            if (i >= size || i >= limit) Die();
        }
    }



    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Die();
        }
        else if (other.gameObject.tag == "Goal")
        {
            reachedTheGoal = true;
            Die();
        }
    }


    public void Respawn()
    {
        transform.position = spawn;
        GetComponent<Rigidbody>().velocity = Vector3.zero; //stop movement
        i = 0;
        dead = false;
        reachedTheGoal = false;
        GetComponent<Renderer>().material = alive;
        //GetComponent<Renderer>().material.shader = transparent;
    }


    public void Die()
    {
        dead = true;
        GetComponent<Renderer>().material = notAlive;
        GetComponent<Rigidbody>().velocity = Vector3.zero; //stop movement
    }


    public void SetChampion()
    {
        GetComponent<Renderer>().material = champion;
        GetComponent<Renderer>().material.shader = AlwaysOnTop;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 17.7f);
    }


    void GenerateVectors()
    {
       for (int j = 0; j < size; j++)
            brain[j] = new Vector3(Random.Range(-10, 11), 0, Random.Range(10, -11));
    }


}
