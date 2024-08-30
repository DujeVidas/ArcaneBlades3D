using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePreferences : MonoBehaviour
{
    public static float sensitivity = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }

}
