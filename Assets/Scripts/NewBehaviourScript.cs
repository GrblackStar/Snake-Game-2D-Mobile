using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void GamePlay()
    {
        SceneManager.LoadScene("S3");
    }

    public void Quit()
    {
        Debug.Log("Application Closed!");
        Application.Quit();
    }    // Update is called once per frame
    void Update()
    {
        
    }
}
