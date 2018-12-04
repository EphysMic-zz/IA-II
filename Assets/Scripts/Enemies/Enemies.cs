using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IAII;

public class Enemies : MonoBehaviour
{
    public enum StateInput { EVADE, ATTACK, IDLE, SEARCH, DIE }
    public float life;
    public float speed;

    [Header("Line Of Sight")]
    public GameObject target;
    public Vector3 ViewDirection;
    public float viewAngle;
    public float viewDistance;
    Vector3 _dirToTarget;
    float _angleToTarget;
    float _distanceToTarget;
    bool _targetInSight = false;
    public GameObject explotionVFX;

    EventFSM<StateInput> myFSM;
    public bool coward;
    //Rigidbody rb;

    //------------------------Mono Methods---------------------------------------------

    public void Awake()
    {
        //rb = GetComponent<Rigidbody>();
        target = FindObjectOfType<Hero>().gameObject;
        SetStates();
        if (coward)
        {
            CorreCorreCorre();
        }
    }
    private void Update()
    {
        myFSM.Update();
        if (life <= 0)
            SendInputToFSM(StateInput.DIE);
    }
    private void FixedUpdate()
    {
        myFSM.FixedUpdate();

    }

    //------------------------State Sets-----------------------------------------------

    void SetStates()
    {
        #region estados
        var evade = new State<StateInput>("EVADE");
        var attack = new State<StateInput>("ATTACK");
        var idle = new State<StateInput>("IDLE");
        var search = new State<StateInput>("SEARCH");
        var die = new State<StateInput>("DIE");
        #endregion

        #region transiciones
        StateConfigurer.Create(evade)
         .SetTransition(StateInput.ATTACK, attack)
         .SetTransition(StateInput.IDLE, idle)
         .SetTransition(StateInput.SEARCH, search)
         .SetTransition(StateInput.DIE, die)
         .Done();

        StateConfigurer.Create(attack)
         .SetTransition(StateInput.EVADE, evade)
         .SetTransition(StateInput.IDLE, idle)
         .SetTransition(StateInput.SEARCH, search)
         .SetTransition(StateInput.DIE, die)
         .Done();

        StateConfigurer.Create(idle)
         .SetTransition(StateInput.ATTACK, attack)
         .SetTransition(StateInput.EVADE, evade)
         .SetTransition(StateInput.SEARCH, search)
         .SetTransition(StateInput.DIE, die)
         .Done();

        StateConfigurer.Create(search)
         .SetTransition(StateInput.ATTACK, attack)
         .SetTransition(StateInput.IDLE, idle)
         .SetTransition(StateInput.EVADE, evade)
         .SetTransition(StateInput.DIE, die)
         .Done();

        StateConfigurer.Create(die).Done();

        #endregion estados

        #region Estados

        #region idle
        idle.OnEnter += (x) =>
        {
            //print("Entré en idlle");
        };
        idle.OnUpdate += () =>
        {
            //   print("Idlle Update");
            bool enemyOnSight = LineOfSight();
            //print("El enemigo esta en vista?: " + enemyOnSight);
            SendInputToFSM(enemyOnSight ? StateInput.SEARCH : StateInput.IDLE);
        };
        idle.OnExit += (x) =>
        {
            //print("Sali de idlle");
        };
        #endregion

        #region search
        search.OnEnter += (x) => { print("Entre en Search"); }; //On Enter Search.
        search.OnUpdate += () => //On Update Search.
        {
            if (LineOfSight())
            {
                //Acá nunca reviso si estoy cerca del enemigo para pasar al estado de ataque.
                if (distanceToAttack()) SendInputToFSM(StateInput.ATTACK);

                //rb.velocity = new Vector3(-target.transform.position.x, -target.transform.position.y, -target.transform.position.z) * speed * Time.deltaTime;
                ViewDirection = target.transform.position - transform.position;
                transform.forward = ViewDirection.normalized;
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else
                SendInputToFSM(StateInput.IDLE);
        };
        search.OnExit += (x) => { print("Sali de search"); };
        #endregion

        #region attack
        attack.OnEnter += (x) => { print("Entre a attack"); };
        attack.OnUpdate += () =>
        {
            if (distanceToAttack()) Attack();
            else SendInputToFSM(StateInput.SEARCH);
        };
        #endregion

        #region die
        die.OnUpdate += () =>
        {
            Destroy(gameObject);
        };
        #endregion

        //estado inicial
        myFSM = new EventFSM<StateInput>(idle);
        #endregion
    }

    void SendInputToFSM(StateInput inp)
    {
        myFSM.Feed(inp);
    }

    //------------------------Class Methods--------------------------------------------

    // Chequea si hay un objetivo visible.
    // <returns>Verdadero si el objetivo es visible.</returns>
    public bool LineOfSight()
    {
        _dirToTarget = (target.transform.position - transform.position).normalized;
        _angleToTarget = Vector3.Angle(transform.forward, _dirToTarget);
        _distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (_angleToTarget <= viewAngle && _distanceToTarget <= viewDistance)
        {
            RaycastHit rch;
            bool obstaclesBetween = false;
            if (Physics.Raycast(transform.position, _dirToTarget, out rch, _distanceToTarget))
                if (rch.collider.gameObject.layer == Layers.Level)
                    obstaclesBetween = true;

            _targetInSight = !obstaclesBetween;
            return _targetInSight;//Retorna true, si no hay obstaculos.
        }
        else
        {
            //print("Puedo ver al objetivo: " + _targetInSight);
            return _targetInSight = false;
        }
    }
    // Chequea si el objetivo esta dentro del rango de ataque.
    // <returns>Verdadero si es posible atacar al objetivo.</returns>
    public bool distanceToAttack()
    {
        var distanceToAttack = Vector3.Distance(transform.position, target.transform.position);
        return distanceToAttack < 3;
    }

    void Attack()
    {
        Debug.Log("attacking");
    }
    public void Death()
    {
        GameObject go = Instantiate(explotionVFX);
        go.transform.position = transform.position;
        SendInputToFSM(StateInput.DIE);
    }
    public void TakeDamage(int damage)
    {
        Debug.Log(life);
        life -= damage;
        GameObject go = Instantiate(explotionVFX);
        go.transform.position = transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "bullet")
            TakeDamage(10);

    }
    public void CorreCorreCorre()
    {

    }
    //-------------------------------Debug------------------------------------------
    /*  void OnDrawGizmos()
      {
          Gizmos.color = Color.blue;
          Gizmos.DrawWireSphere(transform.position, viewDistance);

          //Gizmos.DrawLine(transform.position, transform.position + (transform.forward * viewDistance));

          Gizmos.color = Color.cyan;
          Vector3 rightLimit = Quaternion.AngleAxis(viewAngle, transform.up) * transform.forward;
          Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistance));


          //        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistance));

          Gizmos.color = _targetInSight ? Color.green : Color.red;
          Gizmos.DrawLine(transform.position, target.transform.position);
      }*/
}
