using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IAII;

public class Enemies : MonoBehaviour
{

    public float life;
    public float speed;
    public Rigidbody rb;
    public enum PlayerInputs { EVADE, ATTACK, IDLE, SEARCH, DIE }
    private EventFSM<PlayerInputs> myFSM;


    [Header("Line Of Sight")]
    public Vector3 _dirToTarget;
    public float _angleToTarget;
    public float _distanceToTarget;
    public float viewAngle;
    public float viewDistance;
    public bool _targetInSight;
    public GameObject target;

    public Vector3 dir;


    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        target = FindObjectOfType<Hero>().gameObject;
        States();
    }
    public void States()
    {
        #region estados
        var evade = new State<PlayerInputs>("EVADE");
        var attack = new State<PlayerInputs>("ATTACK");
        var idle = new State<PlayerInputs>("IDLE");
        var search = new State<PlayerInputs>("SEARCH");
        var die = new State<PlayerInputs>("DIE");
        #endregion

        #region transiciones
        StateConfigurer.Create(evade)
            .SetTransition(PlayerInputs.ATTACK, attack)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.SEARCH, search)
            .SetTransition(PlayerInputs.DIE, die)
            .Done();

        StateConfigurer.Create(attack)
         .SetTransition(PlayerInputs.EVADE, evade)
         .SetTransition(PlayerInputs.IDLE, idle)
         .SetTransition(PlayerInputs.SEARCH, search)
         .SetTransition(PlayerInputs.DIE, die)
         .Done();

        StateConfigurer.Create(idle)
         .SetTransition(PlayerInputs.ATTACK, attack)
         .SetTransition(PlayerInputs.EVADE, evade)
         .SetTransition(PlayerInputs.SEARCH, search)
         .SetTransition(PlayerInputs.DIE, die)
         .Done();

        StateConfigurer.Create(search)
         .SetTransition(PlayerInputs.ATTACK, attack)
         .SetTransition(PlayerInputs.IDLE, idle)
         .SetTransition(PlayerInputs.EVADE, evade)
         .SetTransition(PlayerInputs.DIE, die)
         .Done();

        StateConfigurer.Create(die).Done();

        #endregion estados

        #region lo que hace

        #region idle
        idle.OnUpdate += () =>
        {
            if (LineOfSight()) SendInputToFSM(PlayerInputs.SEARCH);
            else SendInputToFSM(PlayerInputs.IDLE);
        };
        #endregion

        #region attack
        attack.OnUpdate += () =>
        {
            if (LineOfSight())
            {
                if (distanceToAttack())
                    Attack();
            }
            else
                SendInputToFSM(PlayerInputs.SEARCH);
        };
        #endregion

        #region search
        search.OnUpdate += () =>
        {
            if (LineOfSight())
            {
              //  rb.velocity = new Vector3(-target.transform.position.x, -target.transform.position.y, -target.transform.position.z) * speed * Time.deltaTime;
                dir = target.transform.position - transform.position;
                transform.forward = dir;
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else
                SendInputToFSM(PlayerInputs.IDLE);
        };
        #endregion

        #region die
        die.OnUpdate += () =>
        {
            Destroy(gameObject);
            Debug.Log("Enemy Dead");

        };
        #endregion

        //estado inicial
        myFSM = new EventFSM<PlayerInputs>(idle);
        #endregion



    }
    private void SendInputToFSM(PlayerInputs inp)
    {
        myFSM.SendInput(inp);
    }
    private void Update()
    {
        myFSM.Update();
    }
    private void FixedUpdate()
    {
        myFSM.FixedUpdate();
    }

    #region LineOfSight
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

            if (!obstaclesBetween)
                return _targetInSight = true;
            else
                return _targetInSight = false;
        }
        else
            return _targetInSight = false;
    }
    #endregion

    public bool distanceToAttack()
    {
        var distanceToAttack = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToAttack < 3)
            return true;
        else return false;

    }

    #region  Attack
    void Attack()
    {
        Debug.Log("attacking");
    }
    #endregion

    void OnDrawGizmos()
    {

        if (_targetInSight)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.transform.position);


        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * viewDistance));

        Vector3 rightLimit = Quaternion.AngleAxis(viewAngle, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistance));

        Vector3 leftLimit = Quaternion.AngleAxis(-viewAngle, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistance));
    }
}
