using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    const float width  = 11.8f;
    const float height = 6.3f;

    public float snakeSpeed = 12f;
    public float enemySpeed = 1.5f;


    public GameObject eggPosition = null;


    //prevent swapwning:
    // GameObject -> Egg currentEgg;
    public Collider2D[] colliders;
    public float radius = 0.4f ;


    public BodyPart   bodyPrefab = null;
    public GameObject rockPrefab = null;
    public GameObject eggPrefab = null;
    public GameObject goldEggPrefab = null;



    // MAP OBSTICLES:
    public GameObject icebergPrefab = null;
    public GameObject iciclePrefab = null;  
    public GameObject mountainPrefab = null;
    public GameObject stonePrefab = null;
    public GameObject volcanoPrefab = null;
    public GameObject treePrefab = null;
    public GameObject snowmanPrefab = null;



    public Sprite tailSprite = null;
    public Sprite bodySprite = null;

    public SnakeHead snakeHead = null;
    public EnemyMovement enemy = null;
    public GameObject enemyObject = null;

    public bool alive = true;

    public bool waitingToPlay = true;

    public List<Egg> eggs = new List<Egg>();
    public List<Vector3> positions = new List<Vector3>();
    public Egg currentEgg = null;

    int level = 0;
    int noOfEggsForNextLevel = 0;

    public int score   = 0;
    public int hiScore = 0;



    public Text scoreText = null;
    public Text hiScoreText = null;

    //public Text tapToPlayText = null;
    public Text gameOverText = null;


    public Text levelUpLabel = null;


    public Button goBack = null;
    public Button playAgain = null;


    public Menu menu = null;
    //public MenuScript menuScript = null;




    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Debug.Log("Starting Snake Game");
        CreateWalls();
        CreateObsticles();

        if(positions.Count >= 23)
        {
            Debug.Log("obsticles created");
        }

        alive = false;
    }

    // Update is called once per frame
   public void Update()
    {
        if (waitingToPlay)
        {
            foreach(Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    StartGamePlay();
                }
            }

            if (Input.GetMouseButtonUp(0))
                StartGamePlay();
        }
    }




    private void Awake()
    {
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
#endif
    }




    public void GameOver()
    {
        alive = false;
        waitingToPlay = true;
        enemy.objectSpeed = 0;

        gameOverText.gameObject.SetActive(true);
        //tapToPlayText.gameObject.SetActive(true);
        goBack.gameObject.SetActive(true);
        playAgain.gameObject.SetActive(true);
        //menu.gameObject.SetActive(true);
        //menuScript.GamePlay();
    }




    IEnumerator Label()
    {
        levelUpLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        levelUpLabel.gameObject.SetActive(false);
    }






    public void ResetEnemy(GameObject gameObject)
    {

        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0); // up
        gameObject.transform.position = new Vector3(0 - 8.49f, -1.98f, -0.01f);
    }




    void StartGamePlay()
    {
        score = 0;
        level = 0;
        enemy.objectSpeed = 1.5f;

        scoreText.gameObject.SetActive(true);
        hiScoreText.gameObject.SetActive(true);

        scoreText.text = "Score = " + score;
        hiScoreText.text = "HiScore = " + hiScore;

       gameOverText.gameObject.SetActive(false);
       playAgain.gameObject.SetActive(false);
       goBack.gameObject.SetActive(false);
        //tapToPlayText.gameObject.SetActive(false);
       // menu.gameObject.SetActive(false);

        waitingToPlay = false;
        alive = true;

        KillOldEggs();

        snakeHead.ResetSnake();
        ResetEnemy(enemyObject);

        LevelUp();
    }

    void LevelUp()
    {
        level++;

        noOfEggsForNextLevel = 4 + (level * 2);
        //noOfEggsForNextLevel = 1;

        snakeSpeed = 1.5f + (level/3f);
        enemy.objectSpeed = 0.5f + (level / 4f);
        //enemy.objectSpeed = 0;
        if (snakeSpeed > 6)
        {
            snakeSpeed = 6;
            enemy.objectSpeed = 5;
        }

        //snakeHead.ResetSnake();
        CreateEgg();
        StartCoroutine(Label());
    }

    public void EggEaten(Egg egg)
    {
        score += 4;

        noOfEggsForNextLevel--;
        if (noOfEggsForNextLevel==0)
        {
            score += 10;
            LevelUp();
        }          
        else if (noOfEggsForNextLevel==1) // last egg
            CreateEgg(true);
        else
            CreateEgg(false);

        if (score>hiScore)
        {
            hiScore = score;
            hiScoreText.text = "HiScore = "+hiScore;
        } 

        scoreText.text = "Score = "+score;

        eggs.Remove(egg);
        Destroy(egg.gameObject);
    }

    public void EggEatenEnemy(Egg egg)
    {
        if(score > 0)
        {
            score--;
        }
        

        if (noOfEggsForNextLevel == 1) // last egg
            CreateEgg(true);
        else
            CreateEgg(false);

        if (score > hiScore)
        {
            hiScore = score;
            hiScoreText.text = "HiScore = " + hiScore;
        }

        scoreText.text = "Score = " + score;

        eggs.Remove(egg);
        Destroy(egg.gameObject);

    }







    // WALLS CREATION:
    void CreateWalls()
    {
        float z = -1f;
        
        Vector3 start  = new Vector3(-width, -height, z);
        Vector3 finish = new Vector3(-width, +height, z);
        CreateLeftWall(start, finish);

        start = new Vector3(+width, -height, z);
        finish = new Vector3(+width, +height, z);
        CreateRightWall(start, finish);

        start = new Vector3(-width, -height, z);
        finish = new Vector3(+width, -height, z);
        CreateDownWall(start, finish);

        start = new Vector3(-width, +height, z);
        finish = new Vector3(+width, +height, z);
        CreateUpWall(start, finish);
    }

    void CreateUpWall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start,finish);
        int noOfRocks = (int)(distance * 1.5f);
        Vector3 delta = (finish-start)/noOfRocks;

        Vector3 position = start;
        for(int rock = 0; rock<=noOfRocks; rock++)
        {
            //float rotation = Random.Range(0, 360f);
            //float scale    = Random.Range(1.5f, 2f);
            CreateRock(position, 0.8f, 180);
            position = position + delta;
        }
    }

    void CreateLeftWall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start, finish);
        int noOfRocks = (int)(distance * 1.7f);
        Vector3 delta = (finish - start) / noOfRocks;

        Vector3 position = start;
        for (int rock = 0; rock <= noOfRocks; rock++)
        {
            //float rotation = Random.Range(0, 360f);
            //float scale    = Random.Range(1.5f, 2f);
            CreateRock(position, 0.8f, -90);
            position = position + delta;
        }
    }

    void CreateRightWall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start, finish);
        int noOfRocks = (int)(distance * 1.7f);
        Vector3 delta = (finish - start) / noOfRocks;

        Vector3 position = start;
        for (int rock = 0; rock <= noOfRocks; rock++)
        {
            //float rotation = Random.Range(0, 360f);
            //float scale    = Random.Range(1.5f, 2f);
            CreateRock(position, 0.8f, 90);
            position = position + delta;
        }
    }

    void CreateDownWall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start, finish);
        int noOfRocks = (int)(distance * 1.7f);
        Vector3 delta = (finish - start) / noOfRocks;

        Vector3 position = start;
        for (int rock = 0; rock <= noOfRocks; rock++)
        {
            //float rotation = Random.Range(0, 360f);
            //float scale    = Random.Range(1.5f, 2f);
            CreateRock(position, 0.8f, 0);
            position = position + delta;
        }
    }

    void CreateRock(Vector3 position, float scale, float rotation)
    {
        GameObject rock = Instantiate(rockPrefab, position, Quaternion.Euler(0,0, rotation));
        rock.transform.localScale = new Vector3(scale,scale,1);
    }







    // METHOD FOR ALL OBSTICLEC
    void CreateObsticles()
    {
        //random generate position and one object in for loops:
        
        Vector3 position;
        //transform.position = position;


        // adding obsticles:
        position = new Vector3(3.48f, 1.63f, -0.01f);
        GameObject iceberg = Instantiate(icebergPrefab, position, Quaternion.Euler(0, 0, 0));
        iceberg.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);


        position = new Vector3(9.15f, -2.17f, -0.01f);
        GameObject iceberg1 = Instantiate(icebergPrefab, position, Quaternion.Euler(0, 0, 0));
        iceberg.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);




        position = new Vector3(-7.48f, 4.35f, -0.01f);
        GameObject icicle = Instantiate(iciclePrefab, position, Quaternion.Euler(0, 0, 0));
        icicle.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(6.71f, 3.73f, -0.01f);
        GameObject icicle1 = Instantiate(iciclePrefab, position, Quaternion.Euler(0, 0, 0));
        icicle.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(-8.86f, -4.62f, -0.01f);
        GameObject icicle2 = Instantiate(iciclePrefab, position, Quaternion.Euler(0, 0, 0));
        icicle.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);




        position = new Vector3(9.693334f, 0.3019454f, -0.01f);
        GameObject mountain = Instantiate(mountainPrefab, position, Quaternion.Euler(0, 0, 0));
        mountain.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(-9.72f, -0.68f, -0.01f);
        GameObject mountain1 = Instantiate(mountainPrefab, position, Quaternion.Euler(0, 0, 0));
        mountain.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(-0.18f, 5.38f, -0.01f);
        GameObject mountain2 = Instantiate(mountainPrefab, position, Quaternion.Euler(0, 0, 0));
        mountain.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);




        position = new Vector3(-10.16f, 5.26f, -0.01f);
        GameObject stone = Instantiate(stonePrefab, position, Quaternion.Euler(0, 0, 0));
        stone.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(-3.67f, 4.28f, -0.01f);
        GameObject stone1 = Instantiate(stonePrefab, position, Quaternion.Euler(0, 0, 0));
        stone.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(1.99f, 3.92f, -0.01f);
        GameObject ston2e = Instantiate(stonePrefab, position, Quaternion.Euler(0, 0, 0));
        stone.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(-2.51f, 0.33f, -0.01f);
        GameObject stone4 = Instantiate(stonePrefab, position, Quaternion.Euler(0, 0, 0));
        stone.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(-5.96f, -2.22f, -0.01f);
        GameObject stone5 = Instantiate(stonePrefab, position, Quaternion.Euler(0, 0, 0));
        stone.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);




        position = new Vector3(-9.72f, 5.44f, -0.10f);
        GameObject tree = Instantiate(treePrefab, position, Quaternion.Euler(0, 0, 0));
        tree.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(-4.24f, 4.61f, -0.10f);
        GameObject tree1 = Instantiate(treePrefab, position, Quaternion.Euler(0, 0, 0));
        tree1.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(7.99f, 2.98f, -0.01f);
        GameObject tree2 = Instantiate(treePrefab, position, Quaternion.Euler(0, 0, 0));
        tree2.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(4.47f, 4.9f, -0.01f);
        GameObject tree3 = Instantiate(treePrefab, position, Quaternion.Euler(0, 0, 0));
        tree3.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(5.67f, -4.68f, -0.01f);
        GameObject tree4 = Instantiate(treePrefab, position, Quaternion.Euler(0, 0, 0));
        tree4.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(-9.9f, -4.75f, -0.01f);
        GameObject tree5 = Instantiate(treePrefab, position, Quaternion.Euler(0, 0, 0));
        tree5.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(2.55f, -0.94f, -0.01f);
        GameObject tree6 = Instantiate(treePrefab, position, Quaternion.Euler(0, 0, 0));
        tree6.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        position = new Vector3(-1.96f, -5.28f, -0.10f);
        GameObject tree7 = Instantiate(treePrefab, position, Quaternion.Euler(0, 0, 0));
        tree7.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);




        position = new Vector3(0.26f, -4.74f, -0.01f);
        GameObject volcano = Instantiate(volcanoPrefab, position, Quaternion.Euler(0, 0, 0));
        volcano.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);



        position = new Vector3(10.85f, 5.3f, -0.01f);
        GameObject snowman = Instantiate(snowmanPrefab, position, Quaternion.Euler(0, 0, 0));
        snowman.transform.localScale = new Vector3(1, 1, 1);
        positions.Add(position);

        //Debug.Log(positions);



    }







    //Collider2D CollisionWithEnemy = Physics2D.OverlapCircle(spawnPoint, enemyRadius, LayerMask.GetMask("EnemyLayer"));




    //spawn bool:
    bool PreventSpawnOverLap(Vector3 spawnPos)
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 centerPoint = colliders[i].bounds.center;
            float widthLoc = colliders[i].bounds.extents.x;
            float heightLoc = colliders[i].bounds.extents.y;

            float leftExtend = centerPoint.x - widthLoc;
            float rightExtend = centerPoint.x + widthLoc;
            float lowerExtend = centerPoint.y - heightLoc;
            float upperExtend = centerPoint.y + heightLoc;

            if (spawnPos.x >= leftExtend  && spawnPos.x <= rightExtend)
            {
                if (spawnPos.y >= lowerExtend && spawnPos.y <= upperExtend)
                {
                    return false;
                }
            }
        }
        return true;
    }


    
    bool PreventOverlap(Vector3 spawnPos)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            if(spawnPos.x > (positions[i].x + 3) &&  spawnPos.x < (positions[i].x - 3))
            {
                if(spawnPos.y > (positions[i].y + 2) && spawnPos.y < (positions[i].y - 2))
                {
                    return false;
                }
            }
        }
        return true;
    }
    




    void CreateEgg(bool golden = false)
    {
        
        /*
        Vector3 position;
        position.x = -width + Random.Range(1f, (width*2)-2f);
        position.y = -height + Random.Range(1f, (height * 2) - 2f);
        position.z = -1f;
        */
        Egg egg = null;


        // spawn:
        Vector3 spawnPos = new Vector3(0, 0, 0);
        bool canSpawnHere = false;


        while (!canSpawnHere)
        {
            float spawnPosX = -width + Random.Range(1f, (width * 2) - 2f);
            float spawnPosY = -height + Random.Range(1f, (height * 2) - 2f);

            spawnPos = new Vector3(spawnPosX, spawnPosY, -1f);
            canSpawnHere = PreventOverlap(spawnPos);

            if (canSpawnHere)
            {
                break;
            }

        }



        if (golden)
        {
             egg = Instantiate(goldEggPrefab, spawnPos, Quaternion.identity).GetComponent<Egg>(); 
        }
        else
        {
             egg = Instantiate(eggPrefab, spawnPos, Quaternion.identity).GetComponent<Egg>();
        }
      


        eggs.Add(egg);
        //positions.Add(position);
        currentEgg = egg;

        eggPosition.transform.position = spawnPos;

    }

    void KillOldEggs()
    {
        foreach(Egg egg in eggs)
        {
            Destroy(egg.gameObject);
        }
        eggs.Clear();
    }
}
