using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableObject : MonoBehaviour
{
    public float health = 5;
    public bool destroyOnDeath = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false); // maybe add death animation
            }
        }
    }

    public void TakeShot()
    {
        Destroy(gameObject);
    }

    public void TakeShot(float bulletDamage)
    {
        health -= bulletDamage;
    }
}
