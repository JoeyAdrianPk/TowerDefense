using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<BiomeConfig> biomes;
    private Queue<int> levelQueue;

    public static LevelManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one instance
        }
    }

    void Start()
    {
        levelQueue = new Queue<int>();

        foreach (var biome in biomes)
        {
            foreach (var level in biome.levels)
            {
                levelQueue.Enqueue(level.buildIndex);
            }
        }

        ShuffleQueue(levelQueue);
    }

    void ShuffleQueue(Queue<int> queue)
    {
        var list = new List<int>(queue);
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        queue.Clear();
        foreach (int item in list)
        {
            queue.Enqueue(item);
        }
    }

    public void LoadNextLevel()
    {
        Debug.Log("loading next level");
        if (levelQueue.Count > 0)
        {
            
            int nextLevelIndex = levelQueue.Dequeue();
            // Change this to the proper index. 
            // Currently set to always load the Procedural Generation Level
            // Sometimes needs to load Overworld
            StartCoroutine(LoadSceneAndSetupLevel(1));
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }

    IEnumerator LoadSceneAndSetupLevel(int buildIndex)
    {

        // Load the scene asynchronously
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("SampleScene");

        // Check if asyncLoad is not null
        if (asyncLoad == null)
        {
            Debug.LogError("Failed to load the scene. Please check the scene name.");
            yield break;
        }

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

       // Debug.Log("Scene loaded. Setting up level...");

        // Set up the level after the scene has loaded
        // CHANGE TO GetLevelConfig(buildIndex) to control level difficulty
        // use RandomLevelConfig() to completely randomize based on level presets for that biome
        LevelConfig levelConfig = RandomLevelConfig();
        
        if (levelConfig != null)
        {

            PathManager pathManager = FindObjectOfType<PathManager>();
            if (pathManager != null)
            {
                
                pathManager.SetupLevel(levelConfig);
            }

            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (menuManager != null)
            {
                
                menuManager.SetupLevel(levelConfig);
            }

            if (pathManager != null)
            {
                pathManager.StartScene();
            }

            EnemyWaveManager enemyWaveManager = FindObjectOfType<EnemyWaveManager>();
            if (enemyWaveManager != null)
            {

                enemyWaveManager.SetupLevel(levelConfig);
            }

            if (enemyWaveManager != null)
            {
                enemyWaveManager.StartScene();
            }
        }
        else
        {
            Debug.LogWarning("LevelConfig is null for buildIndex: " + buildIndex);
        }
    }

    // USE THIS TO RANDOMIZE LEVEL DIFFICULTY

    public LevelConfig RandomLevelConfig()
    {
        int randomLevelToLoad = Random.Range(1, 6);
        foreach ( var biome in biomes)
        {
            foreach(var level in biome.levels)
            {
                if(level.buildIndex == randomLevelToLoad)
                {
                    Debug.Log("returning level " + level);
                    return level;
                }
            }
        }
        Debug.Log("Didn't find level, returning first level of first biome");
        return biomes[0].levels[0];
    }

    // KEEP IT THE SAME TO SET UP SPECIFIC DIFFICULTIES FOR EACH LEVEL OF EACH BIOME
    public LevelConfig GetLevelConfig(int buildIndex)
    {
        foreach (var biome in biomes)
        {
            foreach (var level in biome.levels)
            {
                if (level.buildIndex == buildIndex)
                {
                    return level;
                }
            }
        }
        return null;
    }
}

