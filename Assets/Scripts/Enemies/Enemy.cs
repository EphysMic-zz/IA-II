using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mob {
    //Esta es una clase que ve los enemigos, en donde se ve su cambio de material, se setea su dificultad y se nivelan sus valores
    //CurrMaterial = Es el material de este momento. Ese es cambiado luego segun la dificultad
    //eType = desde un menu se elige la difucltad del enemigo y luego en las funciones de "Set" se cambian sus valores
    //ORDEN DE EJECUCION
    // Awake --------> Carga de datos
    // SetDificulty -> Carga los datos de dificultad
    // Start --------> Inicia los enemigos con la dificultad cargada
    public GameObject model;
    public ENEMYTYPE eType;
    public Material [] enemyMaterial;
    public Material currMaterial;
    public int visionRange;
    // public slimeScript slimeHero;
    public Hero slimeHero;
    //Coins 
    public GameObject coin;
    public float probabilty;
    //Pw
    public GameObject pw;
    public bool dead;

    public delegate void Dificulty();
    public Dictionary<ENEMYTYPE, Dificulty> SetDificulty = new Dictionary<ENEMYTYPE, Dificulty>();

    public enum ENEMYTYPE {
        Easy,
        Medium,
        Hard,
    }

    public void OnDrawGizmos() {
        UnityEditor.Handles.DrawWireDisc(this.transform.position, Vector3.up, visionRange);
    }

    public float Vision() {
        float dist = Vector3.Distance(this.transform.position, slimeHero.transform.position);
        return dist;
    }

    public virtual void Awake() {
        SetDificulty [ ENEMYTYPE.Easy ] = SetEasy;
        SetDificulty [ ENEMYTYPE.Medium ] = SetMedium;
        SetDificulty [ ENEMYTYPE.Hard ] = SetHard;

        slimeHero = FindObjectOfType<Hero>();
    }
    public virtual void Start() {
        SetDificulty [ eType ].Invoke();
        model.GetComponent<Renderer>().material = currMaterial;//En start esto no anda :V es raro, check it
    }
    public override void Death() {
        base.Death();
        if ( probabilty > 65 ) {
            GameObject go = Instantiate(coin);
            go.transform.position = transform.position;
        }
        if ( probabilty > 85 ) {
            GameObject go = Instantiate(pw);
            go.transform.position = transform.position;
        }
        Destroy(gameObject);
        dead = true;
    }
    //Velocidad, puntos, vida
    public virtual void SetEasy() {
        currMaterial = enemyMaterial [ 0 ];
        visionRange = 12;
        probabilty = Random.Range(0, 100);
    }
    public virtual void SetMedium() {
        currMaterial = enemyMaterial [ 1 ];
        visionRange = 14;
        probabilty = Random.Range(30, 100);
    }
    public virtual void SetHard() {
        currMaterial = enemyMaterial [ 2 ];
        visionRange = 16;
        probabilty = Random.Range(50, 100);
    }
}
