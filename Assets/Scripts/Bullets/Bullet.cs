using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bullet : MonoBehaviour
{
    Queries myQueries;

    public float speed;
    private void Start()
    {
        myQueries = GetComponent<Queries>();
    }
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnCollisionEnter(Collision c)
    {
        var enemies = myQueries.Query();

        if (c.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            Debug.Log("sarasa");
            enemies.Select(x => x.GetComponent<Enemies>()).OrderBy(x => x.life).Take(2).ToList().ForEach(x => x.Death());
        }
    }
}
