using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : MonoBehaviour
{
    private new Animation animation;
    //private Animator animator;
    //public GameObject HPBar;
    private GameObject cameraRef;
    private GameObject player;
    private PlayerController playerScript;
    private Enemy enemyScript;
    private float speed = 5;
    private float distanceCloserSpeed = 30;
    private int walkDirection = 0;
    private bool directionChosen = false;
    private Rigidbody eagleRb;
    private Rigidbody playerRb;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    private float jumpForce = 70; //Slight jump before attack
    private bool attack = false;
    private bool beginningIdle = true;
    private bool idle = true;
    private bool chase = false;
    private bool distanceCloser;
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
    public ParticleSystem hitEffect;
    private AudioSource audio;
    public AudioClip eagleCry;
    public AudioClip eagleAttack;
    private float attackVol;
    private float firstAttackVol = 0.1f;
    private float secondAttackVol = 0.3f;
    private bool playOnce = true;
    public bool isOnGround = false;
    private bool attackFinished = false;
    private float distance;
    
    private bool stunLocked = false;
    private float idleTime;
    private float usualIdleTime = 9;//9
    private float damageIdleTime = 6;

    private GameManager gameManager;
    private int HP = 8; //7
    public bool testingStun = false;
    private bool testingBehaviors = false;
    private bool moveLeft = false;
    private bool moveRight = true;
    // Start is called before the first frame update
    void Start()
    {
      animation = GetComponent<Animation>();
        enemyScript = GetComponent<Enemy>();

        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody>();
        playerScript = player.GetComponent<PlayerController>();

        eagleRb = GetComponent<Rigidbody>();
        
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
        enemyScript.enemySounds[0] = eagleAttack;
        enemyScript.SetHP(HP);
        enemyScript.SetFlying();
        enemyScript.HasRevengeValue();

        cameraRef = GameObject.Find("Main Camera");

        StartCoroutine(IdleAnimation());
        ChooseDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if (testingBehaviors == true)
        {
            if (moveLeft == true)
            {
                eagleRb.AddForce(Vector3.left * speed / 2);
            }
            else if (moveRight == true)
            {
                eagleRb.AddForce(Vector3.right * speed / 2);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                moveLeft = !moveLeft;
                moveRight = !moveRight;
            }
        }
        if (testingStun == false)
        {
            if (stunLocked == false)
            {
                Vector3 currentPlayerPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                //Rewriting this so that the characters don't turn on the x-
                ///Ifthisworks, I will carry it over to player and other enemies so they always attack straight
                lookRotation = Quaternion.LookRotation(currentPlayerPosition - transform.position);
                //Did not put the transform.rotationhere because I want the enemies to only be looking
                //when they're not att
                if (idle == true)
                {//Modify to change directions every 2 sec
                    if (walkDirection == 0)
                    {
                        eagleRb.AddForce(Vector3.left * speed);
                    }
                    else if (walkDirection == 1)
                    {
                        eagleRb.AddForce(Vector3.right * speed);
                    }
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
                }
                if (idle == false && chase == true)
                {
                    //animation.Play("Run");
                    
                    
                    followDirection = (player.transform.position - transform.position).normalized;
                    followDirection = new Vector3(followDirection.x, 0, followDirection.z);//NeedEagle to not lower itself
                    //while chasing play
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


                    eagleRb.AddForce(followDirection * speed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
                    //Turned from 5 to 3 for smooth
                    //StartCoroutine(AttackCountdown());
                    if (distance <= 7.5f)
                    {
                        //Switched from tigerActive to isFlying because the eaglewill use the swooping attacking when
                        //the bird is low
                        chase = false;
                        if (attackFinished==false)
                        {
                            Swoop();
                            attackFinished = true;
                        }
                    }
                }

            }
            //if (attackFinished == true && isOnGround == true)
            //{
                //attackFinished = false;
                //idleTime = usualIdleTime;
                //StartCoroutine(IdleAnimation());
            //}
        }

        //HPBar.transform.LookAt(HPBar.transform.position - (cameraRef.transform.position - HPBar.transform.position));

    }
    private void LateUpdate()
    {
        if (enemyScript.hitLanded == true && playOnce == true)
        {
            playOnce = false;
            attackEffect.Play();
            audio.PlayOneShot(eagleAttack, attackVol);
        }
    }
    public void Swoop()
    {
        
if (playerScript.isFlying == false)
        {
            transform.Translate(0, -1.5f, 0);
            enemyScript.SetFlying();

        }
        StartCoroutine(AttackDuration());
        eagleRb.AddForce(attackDirection * 40, ForceMode.Impulse);//Changed from 8 to 12
        attackEffect.Play();
        //StartCoroutine(FreezeRotations());
        //}
    }
    IEnumerator AttackDuration()
    {
        attack = true;
        attackRange.SetActive(true);
        
        yield return new WaitForSeconds(1.5f);
        attack = false;
        eagleRb.velocity = Vector3.zero;
        attackRange.SetActive(false);
        attackFinished = false;
        StartCoroutine(Lag());
        idleTime = usualIdleTime;
    }
    IEnumerator Lag()
    {
        yield return new WaitForSeconds(2);
        if (enemyScript.isFlying == false)
        {
            ReturnToTheAir();
        }
        else if (enemyScript.isFlying == true)
        {
            StartCoroutine(IdleAnimation());
        }

        //StartCoroutine(IdleAnimation());
    }
    public void ReturnToTheAir()
    {
        enemyScript.ResetRevengeValue(); //Makessenseto put it here because if the character can do their idle animation
        //that means they are no longer being stun
        //I think I should make it so that idle is definitely false when stun duration is going on just to be safe
        //To be extra safe, if stun is still equal to truebythe end of the IEnumerator, then don't do ResetRev
        enemyScript.SetFlying(); //Need to do this when revenge value is trig
        //Or I will Put it here instead because that way I don't have to Set it in Start and I don't have to do this in Lag
        transform.Translate(0, 1.5f, 0);
        StartCoroutine(PauseBeforeIdle());
        Debug.Log("Return to");
    }
    IEnumerator PauseBeforeIdle()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(IdleAnimation());
    }
    public void ChooseDirection()
    {
        //0 == left walk, 1 == right walk
        walkDirection = Random.Range(0, 2);
        directionChosen = true;
        //Debug.Log(walkDirection);
        Debug.Log("Direction Chosen");
    }
    IEnumerator IdleAnimation()
    {
        idle = true;
        animation.Play("Eagle Idle");

        if (beginningIdle == true)
        {
            yield return new WaitForSeconds(Random.Range(6, 12));
        }
        //if (playerScript.tigerActive == true)
        //{
        else
        {
            yield return new WaitForSeconds(idleTime);
        }
        //}
        //else if (playerScript.birdActive == true)
        //{
        //monkeyRb.AddForce(Vector3.down * 2, ForceMode.Impulse);
        //yield return new WaitForSeconds(7);
        //}
        beginningIdle = false;
        idle = false;
        chase = true;
        directionChosen = false;
        eagleRb.velocity = Vector3.zero;
    }
    public void OnCollisionEnter(Collision collision)
    {
                if (collision.gameObject.CompareTag("Wall") && idle ==true)
        {
            if (walkDirection == 0)
            {
                walkDirection = 1;
            }
                        else if (walkDirection == 1)
            {
                walkDirection = 0;
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        //Attacks will reset its revenge value, but for the bird, nothing will happen when it reaches
        if (other.CompareTag("Tiger Attack Regular"))
        {
            Damaged();
        }
        if (other.CompareTag("Tiger Special"))
        {
            Damaged();
        }
        if (other.CompareTag("Bird Attack Range"))
        {
            Damaged();
        }
        if (other.CompareTag("Bird Special"))
        {
            Damaged();
        }
    }
    //I need a code that takes into account if the player stops hitting the bird before its revenge value is
    public void Damaged()
    {
        if (attack == false)
        {
            enemyScript.RevengeValueUp();
            if (stunLocked == false)
            {
                stunLocked = true;
                StartCoroutine(StunLock());
            }
            else if (stunLocked == true)
            {
                //stunned = true;
                StopCoroutine(StunLock());
            }
            //animator.SetTrigger("Damaged");
        }
        if (enemyScript.currentRevengeValue == enemyScript.revengeValueCount)
        {
            ReturnToTheAir();
        }
    }


    IEnumerator StunLock()
    {
        yield return new WaitForSeconds(3);
        stunLocked = false;//Almostforgot 
        idleTime = damageIdleTime;
        ReturnToTheAir();
        
    }
}
