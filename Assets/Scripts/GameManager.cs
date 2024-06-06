using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Overworld,
    Battle,
    Victory,
    Defeat
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<GameState> OnGameStateChanged;
    private PathManager pathManager;
    private EnemyWaveManager enemyWaveManager;
    private PlayerManager playerManager;

    private GameState currentState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
       // OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        pathManager = FindObjectOfType<PathManager>();
        enemyWaveManager = FindObjectOfType<EnemyWaveManager>();
        playerManager = FindObjectOfType<PlayerManager>();

        /*
        if(scene.name == "SampleScene")
        {
            playerManager.StartScene();
            pathManager.StartScene();
            enemyWaveManager.StartScene();
        }
        */
    }

    public GameState CurrentState
    {
        get => currentState;
        private set
        {
            if (currentState != value)
            {
                currentState = value;
                OnGameStateChanged?.Invoke(currentState);
            }
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
    }
}

