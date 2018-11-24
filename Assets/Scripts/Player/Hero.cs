﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IAII;

public class Hero : MonoBehaviour
{
    //IA2- P3
    public enum PlayerInputs { MOVE, EXPLOTION, DIE, IDLE, JUMP, SHOOT }
    private EventFSM<PlayerInputs> myFSM;
    private Rigidbody rb;

    [Header("Stats")]
    public float life;
    public bool isDead;

    [Header("Movement")]
    public float speed;
    public float SmoothFactor = 0.05f;
    Vector3 DampVelocity = Vector3.zero;

    [Header("Jump")]
    public float jumpForce;
    public float falloffForce;
    public Bullet bullet;
    public Queries myQuery;
    public Skills mySkills;

    //------------------------Mono Methods---------------------------------------------
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mySkills = GetComponent<Skills>();
        SetStates();
    }
    private void Update()
    {
        myFSM.Update();


        #region negrada
        if (Input.GetKeyDown(KeyCode.E))
            mySkills.Expl();

        if (Input.GetKeyDown(KeyCode.R))
            Shoot();
        #endregion
    }
    private void FixedUpdate()
    {
        myFSM.FixedUpdate();
    }

    //------------------------State Sets-----------------------------------------------

    void SetStates()
    {
        #region Creación de estados

        var idle = new State<PlayerInputs>("IDLE");
        var movement = new State<PlayerInputs>("MOVE");
        var jump = new State<PlayerInputs>("JUMP");
        var explotion = new State<PlayerInputs>("EXPLOTION");
        var die = new State<PlayerInputs>("DIE");
        var shoot = new State<PlayerInputs>("SHOOT");
        #endregion

        #region transiciones

        StateConfigurer.Create(idle)
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.SHOOT, shoot)
            .SetTransition(PlayerInputs.JUMP, jump)
            .Done();

        StateConfigurer.Create(movement)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.SHOOT, shoot)
            .SetTransition(PlayerInputs.JUMP, jump)
            .Done();

        StateConfigurer.Create(jump)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.SHOOT, shoot)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .Done();

        StateConfigurer.Create(explotion)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.SHOOT, shoot)
            .SetTransition(PlayerInputs.JUMP, jump)
            .Done();

        StateConfigurer.Create(shoot).
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.JUMP, jump)
            .Done();

        StateConfigurer.Create(die).Done();
        #endregion

        #region Seteo de estados

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

            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
              print("Entre en Jump");
              rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
          };
        jump.OnUpdate += () =>
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //    SendInputToFSM(PlayerInputs.JUMP);
            print("Velocidad en y: " + rb.velocity.y);
            if (rb.velocity.y < 0)
            {
                print("La velocidad esta disminuyendo");
                rb.AddForce(-transform.up * falloffForce, ForceMode.Force);
            }

            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
    void SendInputToFSM(PlayerInputs input)
    {
        myFSM.Feed(input);
    }

    //------------------------Class Methods--------------------------------------------

    void Move(float horizontalAxis, float verticalAxis)
    {
        //rb.velocity += (transform.forward * Input.GetAxis("Vertical") * speed + transform.right * Input.GetAxis("Horizontal") * speed) * Time.deltaTime;
        Vector3 newdir = Vector3.zero;
        newdir += transform.forward * verticalAxis;
        newdir += transform.right * horizontalAxis;
        var desiredPosition = transform.position + newdir * speed;
        //newdir *= speed * Time.deltaTime;


        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref DampVelocity, SmoothFactor);
        transform.position = smoothPosition;
        //transform.Translate(newPos);
    }
    public void Shoot()
    {
        bullet.transform.position = new Vector3(transform.position.x + 1, transform.position.y + 1, transform.position.z + 1);
        Instantiate(bullet);
    }

    //---------------------------Colisiones--------------------------------------------
    private void OnCollisionEnter(Collision collision)
    {
        SendInputToFSM(PlayerInputs.IDLE);
    }
}
