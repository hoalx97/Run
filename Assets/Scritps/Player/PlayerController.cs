using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SIDE { Left, Mid, Right }
public class PlayerController : MonoBehaviour
{
    public SIDE m_Side = SIDE.Mid;

    public GestureController gesture;
    public CharacterController character;
    public Animator animator;
    float newXPos = 0f;
    public float moveSpeed = 3;
    public float leftRightSpeed = 4;

    private bool counterStarted = false;
    private float xPos;
    private float yPos;
    private float startTime;
    private float collHeight;
    private float collCenterY;

    public float duration = 0.08f;
    public float inxPos;
    public float dodgeSpeed;
    public float runSpeed;
    [HideInInspector]
    public bool isSwipeLeft, isSwipeRight, isSwipeUp, isSwipeDown;
    public bool isJump;
    public bool isRoll;
    public float jumpHeight;

    public RaycastHit hit;                                  //use this if you want to acces objects that are hit with the raycast
    public float distance = 1.2f;                       //set this to go beyond your collider
    public Vector3 direction = new Vector3(0f, -1f, 0f);

    internal float rollCounter = 0.2f;

    void Start()
    {
        collHeight = character.height;
        collCenterY = character.center.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);

        if (gesture.SwipeLeft)
        {
            if (m_Side == SIDE.Mid)
            {
                newXPos = -inxPos;
                m_Side = SIDE.Left;
                animator.Play("Left");
            }
            else if (m_Side == SIDE.Right)
            {
                newXPos = 0;
                m_Side = SIDE.Mid;
                animator.Play("Left");
            }
        }
        else if (gesture.SwipeRight)
        {
            if (m_Side == SIDE.Mid)
            {
                newXPos = inxPos;
                m_Side = SIDE.Right;
                animator.Play("Right");
            }
            else if (m_Side == SIDE.Left)
            {
                newXPos = 0;
                m_Side = SIDE.Mid;
                animator.Play("Right");
            }
        }
        Vector3 moveVector = new Vector3(xPos - transform.position.x, yPos * Time.deltaTime, runSpeed * Time.deltaTime);
        xPos = Mathf.Lerp(xPos, newXPos, Time.deltaTime * dodgeSpeed);
        character.Move(moveVector);
        Jump();
        Roll();
    }


    public void Roll()
    {
        if (IsGrounded())
        {
            rollCounter -= Time.deltaTime;

            if (rollCounter < 0f)
            {
                rollCounter = 0f;
                character.center = new Vector3(0, collCenterY, 0);
                character.height = collHeight;
                isRoll = false;
            }
            if (gesture.SwipeDown)
            {
                rollCounter = 0.2f;
                yPos -= 10;
                character.center = new Vector3(0, collCenterY/2, 0);
                character.height = collHeight/2;
                animator.CrossFadeInFixedTime("Dive", 0.1f);
                isJump = false;
                isRoll = true;
            }
        }

    }

    public void Jump()
    {
        if (IsGrounded())
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                //animator.Play("Dive");
                isJump = false;
            }
            if (gesture.SwipeUp)
            {
                yPos = jumpHeight;
                animator.CrossFadeInFixedTime("Jump", 0.1f);
                isJump = true;
            }
        }
        else
        {
            yPos -= jumpHeight * 2 * Time.deltaTime;
            if (character.velocity.y < -0.1f)
                animator.Play("Jump");
        }
    }

    public bool IsGrounded()
    {
        return IsGroundedByCController() || IsGroundedByRaycast();      //this also doesn't call raycast if we know we are grounded

    }

    public float CountTime()
    {
        return Time.time - startTime;
    }

    public bool IsGroundedByCController()
    {
        if (character.isGrounded == false)
        {
            if (counterStarted == false)
            {
                startTime = Time.time;
                counterStarted = true;
            }
        }
        else counterStarted = false;

        if (CountTime() > duration)
        {
            return false;
        }
        return true;
    }

    public bool IsGroundedByRaycast()
    {
        Debug.DrawRay(transform.position, direction * distance, Color.green);       //draw the line to be seen in scene window

        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {      //if we hit something
            return true;
        }
        return false;
    }
}
