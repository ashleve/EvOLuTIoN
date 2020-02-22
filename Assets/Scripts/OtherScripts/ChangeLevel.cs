using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour {
	

	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown("1"))
            {
                SceneManager.LoadScene(0);
            }
            else if (Input.GetKeyDown("2"))
            {
                SceneManager.LoadScene(1);
            }
            else if (Input.GetKeyDown("3"))
            {
                SceneManager.LoadScene(2);
            }
        }
    }

}
