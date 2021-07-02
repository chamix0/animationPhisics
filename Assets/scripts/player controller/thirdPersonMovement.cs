using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thirdPersonMovement : MonoBehaviour
{
    #region variables

    public CharacterController Controller;
    public Transform cam;
    public float walkSpeed, runSpeed;
    private float speed;
    private bool aux;
    private float turnSmoothVel;
    private float turnSmoothTime = 0.1f;
    float gravity = -9.81f;
    private float vSpeed = 0; // current vertical velocity
    private Animator _animator;
    private GameObject espada;
    private MeshRenderer espadaRen;
    private bool alante, atras, correr, robot, pino, atacar, cogerArma, idle, tecnica;

    #endregion

    private void Start()
    {
        aux = false;
        speed = walkSpeed;
        _animator = GetComponentInChildren<Animator>();
        espada = GameObject.FindGameObjectWithTag("espada");
        espadaRen = espada.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!animate())
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            //  Vector3 vel = transform.forward * vertical * speed;
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle =
                    Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                vSpeed += gravity * Time.deltaTime;
                moveDirection.y = vSpeed;
                Controller.Move(Time.deltaTime * speed * moveDirection);
            }
        }
    }

    bool animate()
    {
        espadaRen.enabled = true;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            espadaRen.enabled = false;
            tecnica = false;
            robot = false;
            pino = false;
            atacar = false;

            speed = walkSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                correr = true;
                speed = runSpeed;
            }
            else
            {
                correr = false;
            }

            atacar = false;

            idle = false;

            alante = true;

            aux = false;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            atacar = true;
            aux = true;
        }
        else if (Input.GetKey(KeyCode.Alpha1))
        {
            espadaRen.enabled = false;
            pino = true;
            aux = true;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            espadaRen.enabled = false;
            robot = true;
            aux = true;
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            espadaRen.enabled = false;
            tecnica = true;
            aux = true;
        }

        else
        {
            alante = false;
            correr = false;
            idle = true;
            if (pino || tecnica || robot)
            {
                espadaRen.enabled = false;
            }
        }


        updateAnimParam();
        return aux;
    }

    public void updateAnimParam()
    {
        _animator.SetBool("alante", alante);
        _animator.SetBool("atras", atras);
        _animator.SetBool("alanteCorrer", correr);
        _animator.SetBool("baileRobot", robot);
        _animator.SetBool("hacerPino", pino);
        _animator.SetBool("atacar", atacar);
        _animator.SetBool("cogerArma", cogerArma);
        _animator.SetBool("idle", idle);
        _animator.SetBool("tecnica", tecnica);
    }
}