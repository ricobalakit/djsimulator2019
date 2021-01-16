using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(0); 
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); 
        }
    }
}
