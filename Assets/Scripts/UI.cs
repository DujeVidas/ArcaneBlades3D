using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public GameObject pause;
    private bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        pause.SetActive(false);
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

    /*
    public void Retry()
    {
        LevelManager.score = 0;
        LevelManager.level = 0;
        SceneManager.LoadScene(1);
    */

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
