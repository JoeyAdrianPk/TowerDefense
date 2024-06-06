using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWaveManager : MonoBehaviour
{

    public GameObject enemyObject;
    public List<GameObject> enemyTypes;
    public int numberOfWaves;
    public List<Vector2Int> pathCells;
    public Button startButton;

    private int enemyCountInWave = 5;
    private float spawnInterval = 0.5f;
    private float timeUntilNextWave = 5f;
    private List<GameObject> enemiesInScene = new List<GameObject>();
    private GameObject enemyInstance;
    private PathManager pathManager;
    private EnemyController enemyController;
    private List<Vector2Int> leftPathCells;
    private List<Vector2Int> topPathCells;
    private List<Vector2Int> rightPathCells;
    private List<Vector2Int> bottomPathCells;
    private int currentSpawnSide = 0;
    private bool victory = false;
    private int difficulty;
    int randomEnemyType;
    private float resetTime;
    private bool roundStarted = false;
    private bool alternateWaveSpawns;
    private bool isEnteringFromLeft;
    private bool isEnteringFromTop;
    private bool isEnteringFromRight;
    private bool isEnteringFromBottom;

    private LevelConfig currentLevelConfig;

    int nextPathCellIndex;

    void Awake()
    {
        GameManager.OnGameStateChanged += HandleStateChange;
    }

    private void HandleStateChange(GameState state)
    {

    }

    public void SetupLevel(LevelConfig levelConfig)
    {
       // Debug.Log("setting up level! ");
        victory = false;
        currentLevelConfig = levelConfig;
        // Use levelConfig to set up enemy waves
       // Debug.Log($"Setting up enemy waves for difficulty: {levelConfig.biome.difficulty}");
        // Example: Configure waves based on levelConfig

        numberOfWaves = levelConfig.waveCount;
        enemyCountInWave = levelConfig.enemyCountInWave;
        timeUntilNextWave = levelConfig.timeBetweenWaves;
        difficulty = levelConfig.levelDifficultyMultiplier;
        resetTime = timeUntilNextWave;
        alternateWaveSpawns = levelConfig.alternate;

        Debug.Log("difficulty set by levelConfig = " + difficulty);
    }

    // Start is called before the first frame update
    public void StartScene()
    {
        startButton = GameObject.Find("Start")?.GetComponent<Button>();
        roundStarted = false;
        pathManager = GetComponent<PathManager>();
        enemyController = GetComponent<EnemyController>();
        leftPathCells = pathManager.leftPathRoute;
        topPathCells = pathManager.topPathRoute;
        startButton.onClick.AddListener(OnStartClick);

       // Debug.Log("Difficulty = " + difficulty);
    }

    // Update is called once per frame
    void Update()
    {
        //  Debug.Log(enemiesInScene.Count);
        if (Time.timeScale == 0 || roundStarted == false)
        {
            return;
        }

        timeUntilNextWave -= 1 * Time.deltaTime;

        if (enemiesInScene.Count <= 0 && numberOfWaves <= 0 && victory == false)
        {
            AllEnemiesKilled();
        }

    }

    void AllEnemiesKilled()
    {
        Debug.Log("all enemies killed");
        GameManager.Instance.SetState(GameState.Victory);
        victory = true;
    }

    void OnStartClick()
    {
        Debug.Log("Difficulty when start is pressed = " + difficulty);
        roundStarted = true;
        Debug.Log("Enemy count in wave = " + enemyCountInWave);
        Debug.Log("Spawn interval = " + spawnInterval);

        isEnteringFromLeft = false;
        isEnteringFromTop = false;
        isEnteringFromRight = false;
        isEnteringFromBottom = false;

        foreach(string entrance in currentLevelConfig.entranceDirections)
        {
            switch (entrance)
            {
                case "left":
                    isEnteringFromLeft = true;
                    break;
                case "top":
                    isEnteringFromTop = true;
                    break;
                case "right":
                    isEnteringFromRight = true;
                    break;
                case "bottom":
                    isEnteringFromBottom = true;
                    break;
            }
        }
        // Below is wrong. Can't hard code the values. Get them from PathManager or levelConfig
        // levelConfig might have to be separate from the PathManager setup. Path/Grid setup differ from enemy/waves setup
        StartCoroutine(SpawnWaves(isEnteringFromLeft, isEnteringFromTop, isEnteringFromRight, isEnteringFromBottom, alternateWaveSpawns));

    }

    IEnumerator ManageWaves(bool leftStart = false, bool topStart = false, bool rightStart = false, bool bottomStart = false, bool alternate = false)
    {
        Debug.Log("We in here");
        while (numberOfWaves > 0)
        {

            yield return StartCoroutine(Spawn(leftStart, topStart, rightStart, bottomStart, alternate));
            // Wait until all enemies are destroyed before starting the next wave
            Debug.Log(timeUntilNextWave);
            yield return new WaitUntil(() => enemiesInScene.Count == 0 || timeUntilNextWave <= 0);

            numberOfWaves--;
            timeUntilNextWave = resetTime;
            Debug.Log("Waves remainng = " + numberOfWaves);
        }

    }

    IEnumerator SpawnWaves(bool left, bool top, bool right, bool bottom, bool alternate)
    {
        Debug.Log(alternate);
        while (numberOfWaves > 0)
        {
            if (!alternate)
            {
                StartCoroutine(ManageWaves(left, top, right, bottom, false));
                break;
            }
            else
            {
                // Determine the spawn position and path based on the current side
                Vector2 spawnPosition = Vector2.zero;
                List<Vector2Int> path = new List<Vector2Int>();

                switch (currentSpawnSide)
                {
                    case 0:
                        if (left)
                        {
                            spawnPosition = new Vector2(leftPathCells[0].x, leftPathCells[0].y);
                            path = leftPathCells;
                        }
                        break;
                    case 1:
                        if (top)
                        {
                            spawnPosition = new Vector3(topPathCells[0].x, topPathCells[0].y);
                            path = topPathCells;
                        }
                        break;
                    case 2:
                        if (right)
                        {
                            spawnPosition = new Vector3(rightPathCells[0].x, rightPathCells[0].y);
                            path = rightPathCells;
                        }
                        break;
                    case 3:
                        if (bottom)
                        {
                            spawnPosition = new Vector3(bottomPathCells[0].x, bottomPathCells[0].y);
                            path = bottomPathCells;
                        }
                        break;
                }

                // If no valid path is assigned, continue to the next iteration
                if (path.Count == 0)
                {
                    currentSpawnSide = (currentSpawnSide + 1) % 4;
                    continue;
                }

                // Spawn the enemies
                for (int count = 0; count < enemyCountInWave; count++)
                { 
                    switch (difficulty)
                    {
                        case 1:
                            randomEnemyType = Random.Range(0, 3);
                            break;
                        case 2:
                            randomEnemyType = Random.Range(3, 7);
                            break;
                        case 3:
                            randomEnemyType = Random.Range(7, 10);
                            break;
                    }
                

                    // int randomEnemyType = Random.Range(0, enemyTypes.Count);
                    GameObject enemyInstance = Instantiate(enemyTypes[randomEnemyType], spawnPosition, Quaternion.identity);
                    enemyInstance.GetComponent<EnemyController>().SetPathRoute(path);
                    enemiesInScene.Add(enemyInstance);
                    enemyInstance.GetComponent<EnemyController>().OnDestroyed += () => enemiesInScene.Remove(enemyInstance);
                    yield return new WaitForSeconds(spawnInterval);
                }
                numberOfWaves--;
                // Wait until all enemies are killed or the countdown timer expires
                //float waveCountdown = spawnInterval * enemyCountInWave; // Adjust as needed
                while (timeUntilNextWave > 0 && enemiesInScene.Count > 0)
                {
                    yield return null; // Wait for the next frame
                    timeUntilNextWave -= Time.deltaTime;
                }
                timeUntilNextWave = resetTime;
                // Increment the spawn side index for the next wave if alternate is true
                if (alternate)
                {
                    currentSpawnSide = (currentSpawnSide + 1) % 4;
                }
            }

        }
    }



    IEnumerator Spawn(bool left, bool top, bool right, bool bottom, bool alternate)
    {
        if (left)
        {
            int count = 0;

            while (count < enemyCountInWave)
            {
                 yield return new WaitForSeconds(spawnInterval);
                // int randomEnemyType = Random.Range(0, enemyTypes.Count);

                switch (difficulty)
                {
                    case 1:
                        randomEnemyType = Random.Range(0, 3);
                        break;
                    case 2:
                        randomEnemyType = Random.Range(3, 7);
                        break;
                    case 3:
                        randomEnemyType = Random.Range(7, 10);
                        break;
                }

              //  Debug.Log("random enemy type = " + randomEnemyType);
              //  Debug.Log("Difficulty in Spawn() = " + difficulty);
                GameObject enemyInstance = Instantiate(enemyTypes[randomEnemyType], new Vector3(leftPathCells[0].x, leftPathCells[0].y, 0f), Quaternion.identity);
                enemyInstance.GetComponent<EnemyController>().SetPathRoute(leftPathCells);
                enemiesInScene.Add(enemyInstance);
                enemyInstance.GetComponent<EnemyController>().OnDestroyed += () => enemiesInScene.Remove(enemyInstance);
                count++;

               // yield return new WaitForSeconds(spawnInterval);
            }
            
        }

        if (top)
        {
            int count = 0;
            while (count < enemyCountInWave)
            {
                yield return new WaitForSeconds(spawnInterval);
                // int randomEnemyType = Random.Range(0, enemyTypes.Count);

                switch (difficulty)
                {
                    case 1:
                        randomEnemyType = Random.Range(0, 3);
                        break;
                    case 2:
                        randomEnemyType = Random.Range(3, 7);
                        break;
                    case 3:
                        randomEnemyType = Random.Range(7, 10);
                        break;
                }

                GameObject enemyInstance = Instantiate(enemyTypes[randomEnemyType], new Vector3(topPathCells[0].x, topPathCells[0].y, 0f), Quaternion.identity);
                enemyInstance.GetComponent<EnemyController>().SetPathRoute(topPathCells);
                enemiesInScene.Add(enemyInstance);
                enemyInstance.GetComponent<EnemyController>().OnDestroyed += () => enemiesInScene.Remove(enemyInstance);
                count++;

               // yield return new WaitForSeconds(spawnInterval);
            }
        }
        if (right)
        {
            int count = 0;
            while (count < enemyCountInWave)
            {
                yield return new WaitForSeconds(spawnInterval);
                //  int randomEnemyType = Random.Range(0, enemyTypes.Count);

                switch (difficulty)
                {
                    case 1:
                        randomEnemyType = Random.Range(0, 3);
                        break;
                    case 2:
                        randomEnemyType = Random.Range(3, 7);
                        break;
                    case 3:
                        randomEnemyType = Random.Range(7, 10);
                        break;
                }

                GameObject enemyInstance = Instantiate(enemyTypes[randomEnemyType], new Vector3(rightPathCells[0].x, rightPathCells[0].y, 0f), Quaternion.identity);
                enemyInstance.GetComponent<EnemyController>().SetPathRoute(rightPathCells);
                enemiesInScene.Add(enemyInstance);
                enemyInstance.GetComponent<EnemyController>().OnDestroyed += () => enemiesInScene.Remove(enemyInstance);
                count++;

               // yield return new WaitForSeconds(spawnInterval);
            }
        }
        if (bottom)
        {
            int count = 0;
            while (count < enemyCountInWave)
            {
                yield return new WaitForSeconds(spawnInterval);
                //  int randomEnemyType = Random.Range(0, enemyTypes.Count);

                switch (difficulty)
                {
                    case 1:
                        randomEnemyType = Random.Range(0, 3);
                        break;
                    case 2:
                        randomEnemyType = Random.Range(3, 7);
                        break;
                    case 3:
                        randomEnemyType = Random.Range(7, 10);
                        break;
                }

                GameObject enemyInstance = Instantiate(enemyTypes[randomEnemyType], new Vector3(bottomPathCells[0].x, bottomPathCells[0].y, 0f), Quaternion.identity);
                enemyInstance.GetComponent<EnemyController>().SetPathRoute(bottomPathCells);
                enemiesInScene.Add(enemyInstance);
                enemyInstance.GetComponent<EnemyController>().OnDestroyed += () => enemiesInScene.Remove(enemyInstance);
                count++;

               // yield return new WaitForSeconds(spawnInterval);
            }
        }


    }


    public void SpawnEnemies()
    {
        if (pathCells != null && pathCells.Count > 1 && nextPathCellIndex < pathCells.Count)
        {
            Vector3 currentPos = enemyInstance.transform.position;
            Vector3 nextPos = new Vector3(pathCells[nextPathCellIndex].x, pathCells[nextPathCellIndex].y, 0f);

            enemyInstance.transform.position = Vector3.MoveTowards(currentPos, nextPos, Time.deltaTime * 4);

            Vector2 direction = nextPos - currentPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //Quaternion currentRotation = enemyInstance.transform.rotation;
            //Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            //enemyInstance.transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, Time.deltaTime * 1000);


            enemyInstance.GetComponent<Rigidbody2D>().rotation = angle;
            if (Vector3.Distance(currentPos, nextPos) < 0.05f)
            {
                nextPathCellIndex++;
            }
            if (enemyInstance.gameObject.transform.position.x >= pathCells[pathCells.Count - 1].x - 0.1f && enemyInstance.gameObject.transform.position.y >= pathCells[pathCells.Count - 1].y - 0.1f)
            {
                Destroy(enemyInstance);
            }

        }
    }

    public void SetPathCells(List<Vector2Int> pathRoute)
    {
        pathCells = pathRoute;
        /*
        foreach(Vector2Int cell in pathRoute)
        {
            Debug.Log(cell);
        }
        */
    }
}
