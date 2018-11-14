using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IAII;
using System.Linq;
public class Hero : MonoBehaviour
{

    //IA2- P3
    public enum PlayerInputs { MOVE, EXPLOTION, DIE, IDLE, JUMP }
    private EventFSM<PlayerInputs> myFSM;
    private Rigidbody rb;

    [Header("Movement")]
    public float speed;
    Vector3 movehorizontal;
    Vector3 movevertical;
    [Header("Jump")]
    public float jumpForce;

    public bool isDead;

    public Bullets bullet;

    public Queries myQuery;

    public Skills mySkills;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mySkills = GetComponent<Skills>();
        states();
    }


    void states()
    {
        #region estados
        //estados
        var idle = new State<PlayerInputs>("IDLE");
        var movement = new State<PlayerInputs>("MOVE");
        var jump = new State<PlayerInputs>("JUMP");
        var explotion = new State<PlayerInputs>("EXPLOTION");
        var die = new State<PlayerInputs>("DIE");
        #endregion

        #region transiciones
        //transiciones
        StateConfigurer.Create(idle).
            SetTransition(PlayerInputs.MOVE, movement).
            SetTransition(PlayerInputs.EXPLOTION, explotion).
            SetTransition(PlayerInputs.DIE, die).
            SetTransition(PlayerInputs.JUMP, jump)
            .Done();

        StateConfigurer.Create(movement)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.JUMP, jump)
            .Done();

        StateConfigurer.Create(jump)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .Done();

        StateConfigurer.Create(explotion)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.JUMP, jump)
            .Done();

        StateConfigurer.Create(die).Done();
        #endregion

        #region seteo de estados
        #region idle
        idle.OnUpdate += () =>
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                SendInputToFSM(PlayerInputs.MOVE);
            else if (Input.GetKeyDown(KeyCode.Space))
                SendInputToFSM(PlayerInputs.JUMP);
            else if (isDead)
                SendInputToFSM(PlayerInputs.DIE);
        };
        #endregion

        #region movimiento

        movement.OnUpdate += () =>
        {
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
                SendInputToFSM(PlayerInputs.IDLE);
            else if (Input.GetKeyDown(KeyCode.Space))
                SendInputToFSM(PlayerInputs.JUMP);
            else if (isDead)
                SendInputToFSM(PlayerInputs.DIE);

        };
        movement.OnFixedUpdate += () =>
        {
            //movevertical = transform.forward * Input.GetAxis("Vertical") * speed;
            //movehorizontal = transform.right * Input.GetAxis("Horizontal") * speed;
            rb.velocity += (transform.forward * Input.GetAxis("Vertical") * speed + transform.right * Input.GetAxis("Horizontal") * speed) * Time.deltaTime;
            //Debug.Log("movement");
        };
        movement.OnExit += x =>
        {
            if (x != PlayerInputs.JUMP)
                rb.velocity = Vector3.zero;
        };
        #endregion

        #region Jump
        jump.OnEnter += x =>
          {
              rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
          };
        jump.OnUpdate += () =>
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SendInputToFSM(PlayerInputs.JUMP);
        };

        #endregion

        #region dead
        die.OnUpdate += () =>
        {
            transform.position = new Vector3(1, 1, 1);
            isDead = true;
        };
        #endregion

     /*   #region explotion

        explotion.OnEnter += x =>
        {
            mySkills.Expl();
            Debug.Log("OnEnter");
        };
        explotion.OnUpdate += () =>
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("OnUpdate");
                SendInputToFSM(PlayerInputs.EXPLOTION);
            }
        };

        #endregion*/


        //estado inicial
        myFSM = new EventFSM<PlayerInputs>(idle);

        #endregion
    }

    private void SendInputToFSM(PlayerInputs input)
    {
        myFSM.SendInput(input);
    }
    private void Update()
    {
        myFSM.Update();


        #region negrada
        if (Input.GetKeyDown(KeyCode.E))
            mySkills.Expl();

        #endregion
    }
    private void FixedUpdate()
    {
        myFSM.FixedUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        SendInputToFSM(PlayerInputs.IDLE);
    }
}

