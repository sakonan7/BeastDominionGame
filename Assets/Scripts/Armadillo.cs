using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armadillo : MonoBehaviour
{
    private Animator animator;
    //public GameObject HPBar;
    private GameObject cameraRef;
    private GameObject player;
    private PlayerController playerScript;
    private Enemy enemyScript;
    private float speed = 220;
    private Rigidbody monkeyRb;
    private Rigidbody playerRb;
    private Collider monkeyAttackReach;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    private float jumpForce = 70; //Slight jump before attack
    private float attackForce = 1; //May remove attackForce because Monkey doesn't knock chaarcter back a
    private bool attack = false;
    private bool beginningIdle = true;
    private bool idle = true;
    private bool tunnelChase = false;
    private bool playerStunned = false; //For if the Tiger is hit by the first claw. Tiger will always get hit twice
    private int damage = 1;
    private bool hitThrown = false;
    //This will account for multiple attacks and multiple effects. Might use an array instead
    //I will need to feed this into Enemy, possibly with a method like WhichAttack, so I can feed it into AttackLanded(
    public int attack1 = 0;
    public int attack2 = 1;
    public int attack3 = 2;

    public GameObject firstClawSlash;
    public GameObject secondClawSlash;
    public GameObject attackRange;
    public ParticleSystem attackEffect;
    private AudioSource audio;
    public AudioClip monkeyAttack;
    private float attackVol;
    private float firstAttackVol = 0.1f;
    private float secondAttackVol = 0.3f;
    private bool playOnce = true;
    public bool isOnGround = false;
    private bool attackFinished = false;
    private float distance;

    private bool stunned = false; //Freeze Monkey when i don't want it to move and when the Monkey is being stunlocked by att
    private float idleTime;
    private float usualIdleTime = 9;
    private float damageIdleTime = 6;

    private GameManager gameManager;
    private int HP = 10; //7
    private bool testingStun = true;
    private bool testingBehaviors = false;
    private bool moveLeft = false;
    private bool moveRight = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyScript = GetComponent<Enemy>();

        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody>();
        playerScript = player.GetComponent<PlayerController>();

        monkeyRb = GetComponent<Rigidbody>();
        monkeyAttackReach = GetComponent<Collider>();

        Physics.gravity *= 0.5f;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (gameManager.difficulty == "Normal")
        {
            damage = 1;
        }
        else if (gameManager.difficulty == "Hard")
        {
            damage = 2;
        }
        else if (gameManager.difficulty == "Very Hard")
        {
            damage = 3;
        }
        enemyScript.SetDamage(damage);
        enemyScript.attackEffect[0] = attackEffect;
        audio = GetComponent<AudioSource>();
        enemyScript.enemySounds[0] = monkeyAttack;
        enemyScript.SetHP(HP);

        cameraRef = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        if (testingStun == false)
        {

            if (idle == false && tunnelChase == true && stunned == false)
            {
                //animation.Play("Run");

                //I think i should do the direction following outside of this
                //Either way, it looks like the Monkey only does it at the start and never rotates to face the tiger again
                //Vector3 newDirection;
                //if (playerScript.tigerActive == true)
                //{
                followDirection = (player.transform.position - transform.position).normalized;
                //newDirection = Vector3.RotateTowards(transform.forward, tiger.transform.position, speed * Time.deltaTime, 0.0f);
                //transform.rotation = Quaternion.LookRotation(newDirection);
                ///}
                //else if (playerScript.birdActive == true)
                //{
                //followDirection = (bird.transform.position - transform.position).normalized;
                //newDirection = Vector3.RotateTowards(transform.forward, bird.transform.position, speed * Time.deltaTime, 0.0f);
                //transform.rotation = Quaternion.LookRotation(newDirection);
                //}
                distance = Vector3.Distance(player.transform.position, transform.position);
                //playerPosition = tiger.transform.position;
                //attackRange.transform.position = tiger.transform.position;
                //distance = Vector3.Distance(player.transform.position, transform.position);
                lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                monkeyRb.AddForce(followDirection * speed);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                            //StartCoroutine(AttackCountdown());
                if (distance <= 1.8)
                {
                    //animator.SetBool("Chase", false);
                    tunnelChase = false;
                    //jumpForce = 60; //Went from 50 to 60 because I want some knockback force from the first att
                    //StartCoroutine(FirstClaw());
                    //}
                    //else if (distance <= 3)
                    //{
                    //animator.SetBool("Chase", false);
                    //chase = false;
                    //jumpForce = 3;
                    //StartCoroutine(FirstClaw());
                   if (attackFinished == false)
                    {
                        PopUp();
                    }
                }
            }
            if (attackFinished == true && isOnGround == true)
            {
                attackFinished = false;
                idleTime = usualIdleTime;
                StartCoroutine(IdleAnimation());
            }
        }
    }
    public void PopUp()
    {
        monkeyRb.AddForce(Vector3.up * 100, ForceMode.Impulse);
        isOnGround = false;
    }
    IEnumerator IdleAnimation()
    {
        idle = true;
        animator.SetBool("Idle", true);
        if (beginningIdle == true)
        {
            yield return new WaitForSeconds(Random.Range(6, 12));
        }
        else
        {
            yield return new WaitForSeconds(idleTime);
        }
        beginningIdle = false;
        idle = false;
        tunnelChase = true;
        //animator.SetBool("Idle", false);
        //animator.SetBool("Chase", true);
    }
    public void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Player") && attack == true && playerScript.dodge == false)
        //{
        //Forgot to change player.transform to tiger.transform, may have messed up the attack cod

        //attack = false; //Doing it over here so that there isn't multiple hits per contact
        //Debug.Log("Hit");
        //}
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Aerial attack IEnumerator will make isOnGround == false
            isOnGround = true;
        }
        //else
        //{
        //isOnGround = false;
        //}
    }
    public void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Sensor"))
        //{
        //playerScript.EnableLockOn(); //It looks like I either can't use a method outside the class or Tiger Sensor specifically
        //isn't instantiated because Monkey Attack Range is for some reason
        //Debug.Log("Can Lock On?");
        //}
        //attack == first is to trigger the attack method when Monkey is chasing the play
        //Need isOnGround because the Monkey triggers this two times by running into the collider and then falling into it
        if (other.CompareTag("Tiger Attack Regular"))
        {
            //For now, just trigger stun. I will use both of their directions to perform the knockback
            //TakeDamage();

            //enemyScript.HP -= 2;
            Damaged();
            //playerScript.PlayTigerRegularStrike(transform.position);
            //Vector3 knockbackDirection = (transform.position - tiger.transform.position).normalized;
            //knockback force is inconsistent. Sometimes it doesn't knockback at all. Sometimes it knocks back too much
            //It doesn't matter what the value is.
            //It may not matter because I will have the attack lag minimized
            //But I don't want the player to whiff attacks, so I think I will make sure the tiger is the right distance from the wolf
            //Unless I can make a force play until a certain distance is reached
            //I can't use forcemode.impulse then
            //wolfRb.AddForce(playerScript.attackDirection * 15, ForceMode.Impulse);
            //playerScript.AttackLandedTrue();
        }
        if (other.CompareTag("Tiger Special"))
        {
            //For now, just trigger stun. I will use both of their directions to perform the knockback
            //TakeDamage();

            //enemyScript.HP -= 7;
            Damaged();
            //playerScript.PlayTigerSpecialStrike(transform.position);
            //Vector3 knockbackDirection = (transform.position - tiger.transform.position).normalized;
            //knockback force is inconsistent. Sometimes it doesn't knockback at all. Sometimes it knocks back too much
            //It doesn't matter what the value is.
            //It may not matter because I will have the attack lag minimized
            //But I don't want the player to whiff attacks, so I think I will make sure the tiger is the right distance from the wolf
            //Unless I can make a force play until a certain distance is reached
            //I can't use forcemode.impulse then
            //wolfRb.AddForce(playerScript.attackDirection * 20, ForceMode.Impulse);
            //playerScript.AttackLandedTrue();
        }
    }
    public void Damaged()
    {
        if (attack == false)
        {
            Stunned();
        }
    }
    public void Stunned()
    {
        StartCoroutine(StunnedDuration());
    }
    IEnumerator StunnedDuration()
    {
        stunned = true;
        //animation.Play("Damage Monkey");
        animator.SetBool("Damaged", true);
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("Damaged", false);
        stunned = false;
        idleTime = damageIdleTime;
        StartCoroutine(IdleAnimation());
    }
}
