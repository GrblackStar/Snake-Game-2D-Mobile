using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuScript : MonoBehaviour
{
    public Menu myMenu = null;
    //myMenu.gameObject.SetActive(true);

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void GamePlay()
    {
        SceneManager.LoadScene("S3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
