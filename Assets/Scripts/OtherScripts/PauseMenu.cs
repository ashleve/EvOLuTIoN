using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;
    public GameObject GameMaster;
    public Text GameSpeedDisplay;
    public Text GenerationCounter;
    public Text PlayerNumDisplay;
    public Slider GameSpeedSlider;
    public Slider PlayerNumSlider;
    public Toggle EnableJumpingToggle;
    public Toggle EnableRotationToggle;
    public InputField ChangeMovementSpeed;
    public InputField ChangeMaximumSpeed;

    public static bool jumpingEnabled = false;
    static bool rotationEnabled = false;

    string sceneName;

    static float gameSpeed = 1.0f;  //static so it doesn't change when level restarts

    public static int playerNum = 100;

    private bool tmp = false;


    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "Level1")
        {
            Cursor.visible = false;
        }

        UpdatePopNumberDisplay(playerNum);
        UpdateGameSpeedDisplay(gameSpeed);

        GameSpeedSlider.value = gameSpeed;
        PlayerNumSlider.value = playerNum;

        EnableJumpingToggle.isOn = jumpingEnabled;
        EnableRotationToggle.isOn = rotationEnabled;

        ChangeMovementSpeed.text = (Player.moveSpeed).ToString();
        ChangeMaximumSpeed.text = (Player.maxSpeed).ToString();
        //tmp = true;
    }


    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        UpdateGenerationText();
    }


    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = gameSpeed;
        if (sceneName != "Level1")
        {
            Cursor.visible = false;
        }
        GameIsPaused = false;
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        Cursor.visible = true;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void ChangeGameSpeedValue(float newSpeed)
    {
        gameSpeed = newSpeed;
    }

    public void UpdateGameSpeedDisplay(float newSpeed)
    {
        GameSpeedDisplay.text = "x" + newSpeed.ToString();
    }

    public void UpdateGenerationText()
    {
        int generation = GameMaster.GetComponent<Population>().generation;
        GenerationCounter.text = "Generation: " + generation.ToString();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(sceneName);
        PauseMenuUI.SetActive(false);
        Time.timeScale = gameSpeed;
        GameIsPaused = false;
    }

    public void ChangePopulationNumber(float n)
    {
        playerNum = (int)n;
    }

    public void UpdatePopNumberDisplay(float n)
    {
        PlayerNumDisplay.text = n.ToString();
    }

    public void SetJumping(bool choice)
    {
        jumpingEnabled = choice;
    }

    public void SetRotation(bool choice)
    {
        rotationEnabled = choice;
        if(rotationEnabled)
        {
            for (int i = 0; i < playerNum; i++)
            {
                GameMaster.GetComponent<Population>().Players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }
        else
        {
            for (int i = 0; i < playerNum; i++)
            {
                GameMaster.GetComponent<Population>().Players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    public void ChangeMoveSpeed(string input)
    {
        //if (tmp)
            Player.moveSpeed = float.Parse(input);
    }

    public void ChangeMaxSpeed(string input)
    {
        //if(tmp)
            Player.maxSpeed = float.Parse(input);
    }

}
