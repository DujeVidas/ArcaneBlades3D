using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject help;
    // Start is called before the first frame update
    void Start()
    {
        help.SetActive(false);
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

    public void ToggleHelp()
    {
        if (help.activeSelf)
            help.SetActive (false);
        else
            help.SetActive(true);
    }
}
