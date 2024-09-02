using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public GameObject pause;
    public GameObject deathScreen;
    public PlayerController player;
    //tmpro text
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI bulletsText;
    private bool isPaused;
    private bool playerDead = false;
    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        pause.SetActive(false);
        deathScreen.SetActive(false);
        healthText.SetText("HP: " + player.health.ToString());
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Time.timeScale == 0f)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        int health = player.health;
        if (health < 0)
            health = 0;

        healthText.SetText("HP: " + health.ToString());
    }
    public void Resume()
    {
        pause.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;

    }

    public void Pause()
    {
        pause.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }

    public void Death()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        playerDead = true;
    }


    public void Retry()
    {
        //LevelManager.score = 0;
        //LevelManager.level = 0;
        SceneManager.LoadScene(1);
    }

    public void ReturnToMain()
    {
        //LevelManager.score = 0;
        //LevelManager.level = 0;
        SceneManager.LoadScene(0);
    }

        // This function should be called from other scripts to check if the game is paused
    public bool IsGamePaused()
    {
        return isPaused;
    }

    public bool IsPlayerDead()
    {
        return playerDead;
    }

    public void SetBulletsText(int bullets, int magSize)
    {
        bulletsText.SetText("Ammo: " + bullets.ToString() + "/" + magSize.ToString());
    }
}
