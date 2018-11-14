using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Bullet : MonoBehaviour
{

    void Update()
    {
        
    }

    public void Shoot()
    {
        RaycastHit[] myRaycast = Physics.RaycastAll(transform.position, transform.forward);


    }
}
