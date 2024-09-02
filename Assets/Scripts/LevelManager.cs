using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static int level = 1;
    public static int finalLevel = 3;
    public static int score = 0;
    public static int scoreIncrement = 5;

    public static string levelString = "Level: ";
    public static string scoreString = "Score: ";
    public static float addEnemySpeedOnLevelUp = 2f;
    public static float enemySpeed = 4f;
    public static int enemyDamage = 10;
    public static int addEnemyDamageOnLevelUp = 4;

    private static float enemySpeedDefault = 4f;
    private static int enemyDamageDefault = 10;
    void Start()
    {
        level = 1;
        score = 0;
        enemySpeedDefault = enemySpeed;
        enemyDamageDefault = enemyDamage;
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public static void IncrementLevel()
    {
        level++;
        if (level == finalLevel)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            enemySpeed += addEnemySpeedOnLevelUp;
            enemyDamage += addEnemyDamageOnLevelUp;
            SceneManager.LoadScene(1);
        }

    }

    public static void OnDeath()
    {
        level = 1;
        enemySpeed = enemySpeedDefault;
        enemyDamage = enemyDamageDefault;
    }

    public static void incrementScore()
    {
        score += scoreIncrement * level;
    }

}
