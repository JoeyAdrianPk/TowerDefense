using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public int gold = 20;
    public int maxHealth = 100;
    public int currentHealth = 100;

    public TMPro.TextMeshProUGUI healthUI;
    public TMPro.TextMeshProUGUI goldUI;

    private void Awake()
    {
        // Check if there is an existing instance of GameManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy this instance if it is not the first
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist this instance across scenes
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") // Replace "MainMenu" with the actual main menu scene name
        {
            Destroy(gameObject); // Destroy the PlayerManager when returning to the main menu
        }
        else
        {
            // If needed, reset player stats here when entering specific scenes
        }

        if(scene.name == "SampleScene")
        {
            healthUI = GameObject.Find("Health Value").GetComponent<TextMeshProUGUI>();
            goldUI = GameObject.Find("Gold Value").GetComponent<TextMeshProUGUI>();
            healthUI.text = currentHealth.ToString();
            goldUI.text = gold.ToString();
        }
    }

    public void StartScene()
    {
        healthUI.text = currentHealth.ToString();
        goldUI.text = gold.ToString();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.text = currentHealth.ToString();
    }

    public void GainGold(int value)
    {
        gold += value;
        goldUI.text = gold.ToString();
    }

    public void LoseGold(int value)
    {
        gold -= value;
        goldUI.text = gold.ToString();
    }

    private void Update()
    {
        //healthUI.text = currentHealth.ToString();
    }

}
