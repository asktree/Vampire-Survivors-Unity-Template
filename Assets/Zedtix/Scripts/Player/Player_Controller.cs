using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{

    public float MOVE_SPEED = 5f;

    private enum State
    {
        Normal,
        Dashing,
    }
    private Rigidbody2D rigidbody2D;
    private Vector3 moveDir;
    private Vector3 rollDir;
    private Vector3 lastMoveDir;
    public float rollSpeed, role = 75, publicdashAmount = 15, cooltime = 2;
    private bool isDashButtonDown;
    private State state;
    float cool;
    public float runSpeed = 20.0f;
    public bool Dashing;
    private TrailRenderer Trail;
    private void Awake()
    {    
        state = State.Normal;
        
    }
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        Trail = GetComponent<TrailRenderer>();
    }
    private void Update()
    {

        Trail.emitting = Dashing;
        var delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
       
        switch (state)
        {
            case State.Normal:
                float moveX = 0f;
                float moveY = 0f;

                if (Input.GetKey(KeyCode.W))
                {
                    moveY = +1f;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    moveY = -1f;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    transform.localScale = new Vector3(-1, 1, 1); // or activate look right some other way
                    moveX = -1f;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.localScale = new Vector3(1, 1, 1); // activate looking left
                    moveX = +1f;
                }

                moveDir = new Vector3(moveX, moveY).normalized;
                if (moveX != 0 || moveY != 0)
                {
                    // Not idle
                    lastMoveDir = moveDir;
                }

                if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift)) && cool <= 0)
                {
                    AudioManager.instance.PlaySound("Dash");
                    cool = cooltime;
                    rollDir = lastMoveDir;
                    rollSpeed = role;
                    state = State.Dashing;

                    Dashing = true;

                }
                else if (cool >= 0)
                {
                    Dashing = false;
                    cool -= Time.deltaTime;

                }
                break;
            case State.Dashing:
                float rollSpeedDropMultiplier = 5f;
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                float rollSpeedMinimum = role / 2;
                if (rollSpeed < rollSpeedMinimum)
                {
                    state = State.Normal;
                }
                break;
        }



      
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                rigidbody2D.velocity = moveDir * MOVE_SPEED;
               
                break;
            case State.Dashing:
                rigidbody2D.velocity = rollDir * rollSpeed;
                break;
        }
    }
}
