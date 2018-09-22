using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasGUI : MonoBehaviour {

    public GameObject camera1;
    public GameObject camera2;
    public GameObject camera3;
    public GameObject camera4;
    //public Texture texture;

    void Start()
    {
        camera1.SetActive(true);
        camera2.SetActive(false);
        camera3.SetActive(false);
        camera4.SetActive(false);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 70, 50), "Camera 1")) 
        {
            camera1.SetActive(true);
            camera2.SetActive(false);
            camera3.SetActive(false);
            camera4.SetActive(false);
        }
        if (GUI.Button(new Rect(70, 0, 70, 50), "Camera 2"))
        {
            camera1.SetActive(false);
            camera2.SetActive(true);
            camera3.SetActive(false);
            camera4.SetActive(false);
        }
        if (GUI.Button(new Rect(140, 0, 70, 50), "Camera 3"))
        {
            camera1.SetActive(false);
            camera2.SetActive(false);
            camera3.SetActive(true);
            camera4.SetActive(false);
        }
        if (GUI.Button(new Rect(210, 0, 70, 50), "Camera 4"))
        {
            camera1.SetActive(false);
            camera2.SetActive(false);
            camera3.SetActive(false);
            camera4.SetActive(true);

        }
    }
}