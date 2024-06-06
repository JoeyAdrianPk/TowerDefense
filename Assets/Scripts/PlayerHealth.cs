using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public TMPro.TextMeshProUGUI healthUI;

    public void Start()
    {
        healthUI.text = currentHealth.ToString();
    }

    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
        if(currentHealth - damage <= 0)
        {
            currentHealth = 0;
            healthUI.text = currentHealth.ToString();
            PlayerDied();
        }
        healthUI.text = currentHealth.ToString();

    }

    private void PlayerDied()
    {
        Debug.Log("You ded");
    }

    private void Update()
    {
        //healthUI.text = currentHealth.ToString();
    }

}
