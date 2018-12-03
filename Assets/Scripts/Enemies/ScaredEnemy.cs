using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IAII;

public class ScaredEnemy : Enemies
{
    public float hp;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Scared enemy take damage");
        hp -= damage;
    }
}
