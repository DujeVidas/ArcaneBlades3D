using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //public GameObject help;
    public GameObject options;
    public GameObject mainMenu;
    public GameObject helpScreen;
    public AudioMixer audioMixer;
    // Start is called before the first frame update
    void Start()
    {
        helpScreen.SetActive(false);
        options.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenOptions()
    {
        options.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void CloseOptions()
    {
        options.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenHelpScreen()
    {
        mainMenu.SetActive(false);
        helpScreen.SetActive(true);
    }

    public void CloseHelpScreen()
    {
        mainMenu.SetActive(true);
        helpScreen.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        //Debug.Log(volume);
        audioMixer.SetFloat("volume", volume);
    }

    public void SetSensitivity(float sensitivity)
    {
        GamePreferences.SetSensitivity(sensitivity);
        Debug.Log(GamePreferences.sensitivity);
    }
}
