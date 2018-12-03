using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IAII;

public class Hero : MonoBehaviour
{
    //IA2- P3
    public enum PlayerInputs { MOVE, EXPLOTION, DIE, IDLE, JUMP, SHOOT, NOSECOMOLLAMARLO }
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
        var nosecomollamarlo = new State<PlayerInputs>("NOSECOMOLLAMARLO");
        #endregion

        #region transiciones

        StateConfigurer.Create(idle)
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.SHOOT, shoot)
            .SetTransition(PlayerInputs.JUMP, jump)
            .SetTransition(PlayerInputs.NOSECOMOLLAMARLO, nosecomollamarlo)
            .Done();

        StateConfigurer.Create(movement)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.SHOOT, shoot)
            .SetTransition(PlayerInputs.JUMP, jump)
            .SetTransition(PlayerInputs.NOSECOMOLLAMARLO, nosecomollamarlo)
            .Done();

        StateConfigurer.Create(jump)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.SHOOT, shoot)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .SetTransition(PlayerInputs.NOSECOMOLLAMARLO, nosecomollamarlo)
            .Done();

        StateConfigurer.Create(explotion)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.SHOOT, shoot)
            .SetTransition(PlayerInputs.JUMP, jump)
            .SetTransition(PlayerInputs.NOSECOMOLLAMARLO, nosecomollamarlo)
            .Done();

        StateConfigurer.Create(shoot)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.JUMP, jump)
            .SetTransition(PlayerInputs.NOSECOMOLLAMARLO, nosecomollamarlo)
            .Done();

        StateConfigurer.Create(nosecomollamarlo)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.MOVE, movement)
            .SetTransition(PlayerInputs.EXPLOTION, explotion)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.JUMP, jump)
            .SetTransition(PlayerInputs.SHOOT, shoot)
            .Done();

        StateConfigurer.Create(die).Done();
        #endregion

        #region Seteo de estados

        #region idle
        idle.OnUpdate += () =>
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                SendInputToFSM(PlayerInputs.MOVE);
            if (Input.GetKeyDown(KeyCode.Space))
                SendInputToFSM(PlayerInputs.JUMP);
            if (Input.GetKeyDown(KeyCode.R))
                SendInputToFSM(PlayerInputs.SHOOT);
            if (Input.GetKeyDown(KeyCode.E))
                SendInputToFSM(PlayerInputs.EXPLOTION);
            if (isDead)
                SendInputToFSM(PlayerInputs.DIE);
            if (Input.GetKeyDown(KeyCode.M))
                SendInputToFSM(PlayerInputs.NOSECOMOLLAMARLO);
        };
        #endregion

        #region movimiento
        movement.OnUpdate += () =>
        {
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
                SendInputToFSM(PlayerInputs.IDLE);
            if (Input.GetKeyDown(KeyCode.Space))
                SendInputToFSM(PlayerInputs.JUMP);
            if (Input.GetKeyDown(KeyCode.R))
                SendInputToFSM(PlayerInputs.SHOOT);
            if (Input.GetKeyDown(KeyCode.E))
                SendInputToFSM(PlayerInputs.EXPLOTION);
            if (isDead)
                SendInputToFSM(PlayerInputs.DIE);
            if (Input.GetKeyDown(KeyCode.M))
                SendInputToFSM(PlayerInputs.NOSECOMOLLAMARLO);

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
            //print("Velocidad en y: " + rb.velocity.y);
            if (rb.velocity.y < 0) rb.AddForce(-transform.up * falloffForce, ForceMode.Force);

            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (Input.GetKeyDown(KeyCode.E))
                SendInputToFSM(PlayerInputs.EXPLOTION);
        };

        #endregion

        #region dead
        die.OnUpdate += () =>
        {
            transform.position = new Vector3(1, 1, 1);
            isDead = true;
        };
        #endregion

        #region explotion
        explotion.OnEnter += x =>
        {
            mySkills.Expl();
            // Debug.Log("OnEnter");
        };
        explotion.OnUpdate += () =>
        {
            // Debug.Log("OnUpdate");
            SendInputToFSM(PlayerInputs.IDLE);
        };
        explotion.OnExit += x =>
        {
            // print("Salí de Explotion");
        };
        #endregion

        #region Shoot
        shoot.OnEnter += x =>
        {
            //   print("Entré en Shoot");
            Shoot();
        };
        shoot.OnUpdate += () =>
        {
            //  print("Estoy en Shoot");
            SendInputToFSM(PlayerInputs.IDLE);
        };
        shoot.OnExit += (x) =>
        {
            // print("Sali de Shoot");
        };
        #endregion
        #region Test
        nosecomollamarlo.OnEnter += x =>
        {
            print("Worth!");
            mySkills.test();
            SendInputToFSM(PlayerInputs.IDLE);
        };
        #endregion
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
        if (bullet != null)
        {
            var newBullet = Instantiate(bullet);
            newBullet.transform.position = new Vector3(transform.position.x + 1, transform.position.y + 1, transform.position.z + 1);
        }
    }

    //---------------------------Colisiones--------------------------------------------
    private void OnCollisionEnter(Collision collision)
    {
        SendInputToFSM(PlayerInputs.IDLE);
    }

}

