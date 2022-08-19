using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]public  float objectSpeed;
    [SerializeField] GameObject eggPosition = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Egg egg = collision.GetComponent<Egg>();
        if (egg)
        {
            Debug.Log("Hit Egg");
            EatEgg(egg);
        }
        else
        {
            Debug.Log("Hit Obsticle");
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveGameObject();
        //if (!GameController.instance.alive) return;
    }


    private void EatEgg(Egg egg)
    {
        GameController.instance.EggEatenEnemy(egg);
    }



    void MoveGameObject()
    {
        transform.position = Vector3.MoveTowards(transform.position, eggPosition.transform.position, objectSpeed * Time.deltaTime);
    }


   /* public void ResetEnemy()
    {
        
        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0); // up
        gameObject.transform.position = new Vector3(0 - 8.49f, -1.98f, -0.01f);
    }*/



}