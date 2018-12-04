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
    public IEnumerable<Enemies> enemies;

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
        Tuple<float, float> promSum = enemies.Select(x => x.GetComponent<Enemies>()).Aggregate((Tuple.Create(0f, 0f)), (acum, curr) =>
        {
            return Tuple.Create(acum.Item1 + curr.life, acum.Item2 + 1);
        });

        float prom = promSum.Item1 / promSum.Item2;
        if (prom < 30)
        {
            //enemies.OfType<Enemy>().Where(x => x != null && x.hp < 20).ToList().ForEach(x => x.Death());
            GameObject go = Instantiate(explotionVFX);
            go.transform.position = transform.position;
            enemies.Select(x => x.GetComponent<Enemies>()).ToList().ForEach(x => x.Death());
        //    Debug.Log("Total prom: " + prom);
         //   Debug.Log("Total Life: " + promSum.Item1 + "TotalEnemies: " + promSum.Item2);
        }
        else
            enemies.Select(x => x.GetComponent<Enemies>()).Where(x => x.life < 70).ToList().ForEach(x => x.TakeDamage(10));


    }

    //IA - P1 (Select, Where, Concat)
    public void test()
    {
        var enemies = myQueries.Query();

        enemies.Select(x => x.GetComponent<Enemies>())
               .Where(x => x.life >= 60)
               .Concat(enemies.Select(y => y.GetComponent<Enemies>()).Where(y => y.coward))
               .ToList()
               .ForEach(x => x.TakeDamage(100));
    }
}
