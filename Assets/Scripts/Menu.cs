using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    float time,second;
    [SerializeField]
    public Image FillImage;
    // Start is called before the first frame update
    
    public void Start()
    {
        second = 3;
        Invoke("LoadGame", 3f);
    }
    
    public void GoBackMenu()
    {
        SceneManager.LoadScene("S2");
    }
    public void update()
    {
        if(time<5)
        {
            time += Time.deltaTime;
            FillImage.fillAmount = time/second;
        }
    }
    

    public void LoadGame()
    {
        SceneManager.LoadScene("S2");
    }
  
    
    public void Quit()
    {
        Debug.Log("App closed!");
        Application.Quit();
    }
   
}
