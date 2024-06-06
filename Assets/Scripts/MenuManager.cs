using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button startGameButton;
    private bool isPaused = false;
    private LevelConfig currentLevelConfig;

    public LevelManager levelManager;
    public Button pauseButton;


    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChange;
    }

    public void SetupLevel(LevelConfig levelConfig)
    {
        currentLevelConfig = levelConfig;
        
    }

    void Start()
    {
        victoryScreen = GameObject.Find("Victory Screen");
        levelManager = FindObjectOfType<LevelManager>();
        pauseButton?.onClick.AddListener(TogglePause);
        continueButton?.onClick.AddListener(NextRound);
        startGameButton?.onClick.AddListener(PlayGame);
        if(victoryScreen != null)
        {
            victoryScreen?.SetActive(false);
        }
        
        //  quitButton.onClick.AddListener(QuitGame);
    }

    private void HandleGameStateChange(GameState state)
    {
       // Debug.Log("GameState changed to - " + state);
        if (victoryScreen != null)
        {
            Debug.Log(state);
            victoryScreen.SetActive(state == GameState.Victory);
        }

    }

    void NextRound()
    {
        GameManager.Instance.SetState(GameState.Battle);
        levelManager.LoadNextLevel();
        //GameManager.Instance.SetState(GameState.Overworld);
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PlayGame()
    {
        levelManager.LoadNextLevel();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        //  pauseMenu.SetActive(true); // Show the pause menu
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        //  pauseMenu.SetActive(false); // Hide the pause menu
    }

    public void QuitGame()
    {
        // Implement quit functionality
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }
}
