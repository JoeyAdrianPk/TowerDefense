using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathManager : MonoBehaviour

{
    public int gridWidth = 11;
    public int gridHeight = 8;
    public int gridDepth = 0;
    public int minPathLength = 35;
    private EnemyWaveManager waveManager;

    public GridCellObject[] gridCells;
    public GridCellObject[] sceneryCells;

    public GameObject pathTile;

    public PathGenerator pathGenerator;
    public List<Vector2Int> pathCells;
    public List<Vector2Int> pathRoute;
    public List<Vector2Int> leftPathRoute;
    public List<Vector2Int> topPathCells;
    public List<Vector2Int> topPathRoute;

    private LevelConfig currentLevelConfig;
    private Vector2Int exit;

    int topPathSize;
    int topAttempts;
    int pathSize;
    //int attempts = 100;

    public void SetupLevel(LevelConfig levelConfig)
    {
        currentLevelConfig = levelConfig;
        
        // Use levelConfig to set up enemy waves
       // Debug.Log($"Setting up enemy waves for difficulty: {levelConfig.biome.difficulty}");
        // Example: Configure waves based on levelConfig
    }

    // Start is called before the first frame update
    public void StartScene()
    {
        Debug.Log("Current scene config = " + currentLevelConfig);
        // SceneManager.sceneLoaded += OnSceneLoaded;
        exit = SetExitPoint(currentLevelConfig.endPoint);
        pathGenerator = new PathGenerator(currentLevelConfig.gridWidth, currentLevelConfig.gridHeight, gridDepth);
        waveManager = GetComponent<EnemyWaveManager>();

        ClearAllPaths();

        for (int i = 0; i < currentLevelConfig.numberOfEntrances; i++)
        {
            switch (currentLevelConfig.entranceDirections[i])
            {
                case "left":
                    pathCells = pathGenerator.GeneratePath("left", exit);

                    while (pathCells.Count < currentLevelConfig.minPathLength)
                    {
                        pathCells = pathGenerator.GeneratePath("left", exit);
                    }
                    leftPathRoute = pathCells;
                    Debug.Log("Setting up 'left' path. exit = " + exit);
                    break;
                case "top":
                    topPathCells = pathGenerator.GeneratePath("top", exit);

                    while (topPathCells.Count < currentLevelConfig.minPathLength)
                    {
                        topPathCells = pathGenerator.GeneratePath("top", exit);
                    }
                    topPathRoute = topPathCells;
                    Debug.Log("setting up 'top' path. exit = " + exit);
                    break;
              /*  default:
                    pathCells = pathGenerator.GeneratePath("left", exit);

                    while (pathCells.Count < minPathLength)
                    {
                        pathCells = pathGenerator.GeneratePath("left", exit);
                    }
                    break;
              */
            }
        }
        
        leftPathRoute = pathCells;
        topPathRoute = topPathCells;
        
        
        StartCoroutine(CreateGrid(exit, pathCells, leftPathRoute, topPathRoute));
    }

    IEnumerator CreateGrid(Vector2Int exit, List<Vector2Int> pathCells = null, List<Vector2Int> leftPathCells = null, List<Vector2Int> topPathCells = null)
    {
        // TODO -- Add right and bottom to everything... Pain the ass...


        if(pathCells != null)
        {
            yield return LayPathCells(leftPathCells);
        }

        if (topPathCells != null)
        {
            yield return LayPathCells(topPathCells);
        }

        
        yield return LayGrassCells(leftPathCells, topPathCells);
        waveManager.SetPathCells(pathGenerator.GenerateRoute(leftPathCells, topPathCells, null, null, exit));
    }

    private IEnumerator LayPathCells(List<Vector2Int> pathCells)
    {
        foreach (Vector2Int pathCell in pathCells)
        {
           // Debug.Log(pathCell);
            int neighborValue = pathGenerator.GetCellNeighborValue(pathCells, pathCell.x, pathCell.y);
            // Debug.Log("Tile " + " , " + pathCell.x + " , " + pathCell.y + " Neighbor Value = " + neighborValue);
            GameObject pathTile = gridCells[neighborValue].cellPrefab;
            if(pathCell == pathCells[pathCells.Count - 1])
            {
                pathTile = gridCells[16].cellPrefab;
            }
            GameObject pathTileCell = Instantiate(pathTile, new Vector3(pathCell.x, pathCell.y, 0f), Quaternion.identity);
            pathTileCell.transform.Rotate(0f, 0f, gridCells[neighborValue].zRotation, Space.Self);
             yield return new WaitForSeconds(0.01f);
        }
        yield return null; 
    }

    private IEnumerator LayGrassCells(List<Vector2Int> leftPathCells, List<Vector2Int> topPathCells = null, List<Vector2Int> rightPathCells = null, List<Vector2Int> bottomPathCells = null)
    {
        List<Vector2Int> pathCells = new List<Vector2Int>();
        pathCells.AddRange(leftPathCells);
        if(rightPathCells != null)
        {
            pathCells.AddRange(rightPathCells);
        }
        if(topPathCells != null)
        {
            pathCells.AddRange(topPathCells);
        }
        if(bottomPathCells != null)
        {
            pathCells.AddRange(bottomPathCells);
        }
        
        for (int x = 0; x < gridWidth +1; x++)
        {
            for (int y = 0; y < gridHeight +2; y++)
            {
                if (CellIsFree(pathCells, x , y))
                {
                    int randomSceneryCell = Random.Range(0, sceneryCells.Length);
                    int randomPlacement = Random.Range(0, 10);
                    Instantiate(sceneryCells[0].cellPrefab, new Vector3(x, y, 0f), Quaternion.identity);
                    if(randomPlacement <= 1 && x < gridWidth && y < gridHeight)
                    {
                        Instantiate(sceneryCells[randomSceneryCell].cellPrefab, new Vector3(x, y, 0f), Quaternion.identity);
                    }
                    // yield return new WaitForSeconds(0.01f);
                }
            }
        }

        yield return null;
    }
    // x = -10 y = -7
    private IEnumerator LaySceneryCells()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (CellIsFree(pathCells, x, y))
                {
                    int randomSceneryCell = Random.Range(0, sceneryCells.Length);
                    Instantiate(sceneryCells[randomSceneryCell].cellPrefab, new Vector3(x, y, 0f), Quaternion.identity);
                    // yield return new WaitForSeconds(0.1f);
                }
            }
        }

        yield return null;
    }

    public bool CellIsFree(List<Vector2Int> pathCells, int x, int y)
    {
       // Debug.Log(pathCells.Count);
        return !pathCells.Contains(new Vector2Int(x, y));
    }

    public bool CellIsTaken(List<Vector2Int> pathCells, int x, int y)
    {
       // Debug.Log(pathCells.Count);
        return pathCells.Contains(new Vector2Int(x, y));
    }

    private Vector2Int SetExitPoint(string exit)
    {
        int x = 0;
        int y = 0;
        if(exit == "middle")
        {
            x = (currentLevelConfig.gridWidth / 2) % 2 != 0 ? currentLevelConfig.gridWidth / 2 + 1 : currentLevelConfig.gridWidth / 2;
            y = currentLevelConfig.gridHeight / 2;
        }
        else if(exit == "right")
        {
            x = currentLevelConfig.gridWidth - 1;
            y = Random.Range(1, currentLevelConfig.gridHeight - 2);
        }
        
        return new Vector2Int(x,y);
    }

    private void ClearAllPaths()
    {
        if (pathCells.Count > 0)
        {
            pathCells.Clear();
        }
        if (leftPathRoute.Count > 0)
        {
            leftPathRoute.Clear();
        }
        if (topPathCells.Count > 0)
        {
            topPathCells.Clear();
        }
        if(topPathRoute.Count > 0)
        {
            topPathRoute.Clear();
        }
    }
}
