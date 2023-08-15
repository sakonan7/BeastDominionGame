using UnityEngine;
using System.Collections;

public class ArmadilloCharacter : MonoBehaviour
{
    Animator armadilloAnimator;

    public float groundCheckDistance = 0.6f;
    public float groundCheckOffset = 0.01f;
    public bool isGrounded = true;
    public bool isLived = true;
    public bool isBalling = false;

    Rigidbody armadilloRigid;
    public float forwardSpeed;
    public float turnSpeed;
    public float walkMode = 1f;



    public float maxWalkSpeed = 1f;

    void Start()
    {
        armadilloAnimator = GetComponent<Animator>();
        armadilloRigid = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        CheckGroundStatus();
        Move();

        maxWalkSpeed = Mathf.Lerp(maxWalkSpeed, walkMode, Time.deltaTime);
    }

    public void Attack()
    {
        armadilloAnimator.SetTrigger("Attack");
    }

    public void Hit()
    {
        armadilloAnimator.SetTrigger("Hit");
    }

    public void EatStart()
    {
        armadilloAnimator.SetBool("IsEating",true);
    }
    public void EatEnd()
    {
        armadilloAnimator.SetBool("IsEating", false);
    }


    public void Gallop()
    {
        walkMode = 4f;
    }


    public void Walk()
    {
        walkMode = 1f;
    }

    public void BallStart()
    {
        armadilloAnimator.SetBool("IsBall", true);
    }

    public void BallEnd()
    {
        armadilloAnimator.SetBool("IsBall", false);
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
        isGrounded = Physics.Raycast(transform.position + (transform.up * groundCheckOffset), Vector3.down, out hitInfo, groundCheckDistance);


        if (isGrounded)
        {
            armadilloAnimator.applyRootMotion = true;
            armadilloAnimator.SetBool("IsGrounded", true);
        }
        else
        {

            armadilloAnimator.applyRootMotion = false;
            armadilloAnimator.SetBool("IsGrounded", false);

        }
    }

    public void Move()
    {
        armadilloAnimator.SetFloat("Forward", forwardSpeed);
        armadilloAnimator.SetFloat("Turn", turnSpeed);
    }
}
