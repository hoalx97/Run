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

    public float inxPos;
    private float xPos;
    private float yPos;
    public float dodgeSpeed;
    [HideInInspector]
    public bool isSwipeLeft, isSwipeRight, isSwipeUp, isSwipeDown;
    public bool isInJump;
    public bool isRoll;
    public float jumpHeight;

    void Start()
    {
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
        Vector3 moveVector = new Vector3(xPos - transform.position.x, yPos * Time.deltaTime, 0);
        xPos = Mathf.Lerp(xPos, newXPos, Time.deltaTime * dodgeSpeed);
        character.Move(moveVector);
        Jump();
    }

    public void Jump()
    {
        Debug.Log(character.isGrounded);
        if (character.isGrounded)
        {
            Debug.Log("Jump");
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                animator.Play("Rolling");
                isInJump = false;
            }
            if (gesture.SwipeUp)
            {
                yPos = jumpHeight;
                animator.CrossFadeInFixedTime("Jump", 0.1f);
                isInJump = true;
            }
            else
            {
                yPos -= jumpHeight * 2 * Time.deltaTime;
                if (character.velocity.y < -0.1f)
                    animator.Play("Falling");
            }
        }
    }

}
