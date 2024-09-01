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
    private bool isPaused;
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

        healthText.SetText("HP: " + player.health.ToString());
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
}
