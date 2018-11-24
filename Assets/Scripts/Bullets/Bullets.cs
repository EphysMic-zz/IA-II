using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    //Esta es la clase en la que parten todas las bullets
    //Tiene su propio daño, velocidad, sonido y direccion
    //Se creala funcion DispenseBullets para hacer un override luego.
    //La funcion Impact destruye el objeto y reproduce el sonido de disparo.
    //Tiene que haber SI O SI UN ROGIDBODY
    //El discharger es quien dispara
    //Initialize es una funcion para cargar los valores default de la bala y poder reiniciarla luego
    //resetbullet es para reiniciar la bala con esos valores default
    public int speed,
               dmg;
    public float delay;
    private int baseSpeed,
        baseDmg;
    private float baseDelay;
    public float distance;
    public GameObject discharger;
    public Vector3 direction;
    public virtual void DispenseBullets()
    {

    }
    public void Impact()
    {
        Destroy(this.gameObject, 0.1f);
    }
    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.layer == LayerMask.NameToLayer("Level"))
        {
            Impact();
        }
        if (c.gameObject.layer == LayerMask.NameToLayer("Bat") || c.gameObject.layer == LayerMask.NameToLayer("Ghost") ||
             c.gameObject.layer == LayerMask.NameToLayer("Rabbit"))
        {
            Impact();
        }
        if (c.gameObject.layer == LayerMask.NameToLayer("SlimeEvil") && discharger.layer != LayerMask.NameToLayer("SlimeEvil"))
        {
            Impact();
        }
        if (c.gameObject.layer == LayerMask.NameToLayer("SlimeHero") && discharger.layer != LayerMask.NameToLayer("SlimeHero"))
        {
            Impact();
        }
    }
    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.layer == LayerMask.NameToLayer("Level"))
        {
            Impact();
        }
    }
    public void Initialize()
    {
        baseSpeed = speed;
        baseDelay = delay;
        baseDmg = dmg;
    }
    public void ResetBulets()
    {
        speed = baseSpeed;
        delay = baseDelay;
        dmg = baseDmg;
    }
}
