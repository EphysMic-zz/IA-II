using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Skills : MonoBehaviour
{
    //IA-P2
    Queries myQueries;
    public GameObject explotionVFX;

    public void Awake()
    {
        myQueries = GetComponent<Queries>();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //IA - P1 (where, select)
    public void Expl()
    {
        var enemies = myQueries.Query();
        //enemies.OfType<Enemy>().Where(x => x != null && x.hp < 20).ToList().ForEach(x => x.Death());
        GameObject go = Instantiate(explotionVFX);
        go.transform.position = transform.position;
        enemies.Select(x => x.GetComponent<Enemies>()).Where(x => x.life < 20).ToList().ForEach(x => x.Death());
    }
}
