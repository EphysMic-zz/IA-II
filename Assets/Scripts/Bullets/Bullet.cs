using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bullet : MonoBehaviour
{
    Queries myQueries;
    public GameObject collisionVFX;

    public float speed;
    private void Start()
    {
        myQueries = GetComponent<Queries>();
    }
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    public void OnTriggerEnter(Collider c)
    {
        var enemies = myQueries.Query();

        if (c.gameObject.tag == "enemy")
        {
            Debug.Log("sarasa");
            enemies.Select(x => x.GetComponent<Enemies>()).OrderBy(x => x.life).Take(2).ToList().ForEach(x => x.TakeDamage(20));
            GameObject go = Instantiate(collisionVFX);
            go.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
