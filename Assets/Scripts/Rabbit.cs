using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : MonoBehaviour
{
    private Animator animator;
    private GameObject cameraRef;
    private GameObject player;
    private PlayerController playerScript;
    private Enemy enemyScript;
    private float speed = 50;
    private Rigidbody rabbitRb;
    private Rigidbody playerRb;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    private float jumpForce = 70; //Slight jump before attack
    private float attackForce = 1; //May remove attackForce because Monkey doesn't knock chaarcter back a
    private bool attack = false;

    public bool beginningIdle = true;

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
    private Quaternion lookAwayRotation;
    private bool runAway = false;
    private bool alreadyRanAway = false;
    private Vector3 runDirection;
    private bool pauseThenShoot = false;

    public GameObject arrow;
    public Transform firingPosition;
    
    public ParticleSystem attackEffect;
    private AudioSource audio;
    public AudioClip rabbitAttack;
    private float attackVol;
    private float firstAttackVol = 0.1f;
    private float secondAttackVol = 0.3f;
    private bool playOnce = true;
    public bool isOnGround = false;
    private bool attackFinished = false;
    private float distance;

    //The Armadillo will start with a random attack and then cycle between two of 
    private int whichAttack = 0;
    private int attackOne = 0;
    private int attackTwo = 1;
    private bool isTunneled = false;

    private bool stunned = false; //Freeze Monkey when i don't want it to move and when the Monkey is being stunlocked by att
    private float idleTime;
    private float usualIdleTime = 9;
    private float damageIdleTime = 6;

    private GameManager gameManager;
    private int HP = 8; //7
    public bool testingStun = true;
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

        rabbitRb = GetComponent<Rigidbody>();

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
        enemyScript.SetHP(HP);

        cameraRef = GameObject.Find("Main Camera");
        StartCoroutine(IdleAnimation());
        //animator.SetBool("Idle", true);
        whichAttack = attackOne;
    }

    // Update is called once per frame
    void Update()
    {
        //ATm, I will have the rabbit perform a single shot when the player is <= 7 meters away. Then it will keep performing single
        //shots at any range. Thinking about it, I could have the rabbit alternate between the two attacks if the player is too 
        //Prospectively, I would want the rabbit to alternate between the two attacks and then have the rabbit run away if the player is too close.
        //To run away, I could have it run backwards if it is too close to a wall, I got too complicated and thoughtI'd need to calculate
        //based on the rabbit's back beingto a wall

        //For both versions, he will cancel his second bow shot if the player gets too close. Nothing will come out.
        //I just thought of something. I will make rabbit run away a distance from the player if the player is within 5 meters of them.
        //I will simply set an IEnumerator to make the rabbit run for 2 seconds or until it hits a wall. I could also make the rabbit stoprunning
        //away if it reaches 7 meters away from the player for practice and to get something close to what I 
        //
        if (testingStun == false)
        {
            //I'm gonna take out stunned == false because each time a foe is in attack mode, it can't be flinched and
            //they will be set back into IdleAnimation and only have IdleAnimation happen if the foe is not stunned
            //Currently for suspension of disbelief, I will not properly have the rabbit turn right away from the player 
            //And just have the arrow seekthe player
            distance = Vector3.Distance(player.transform.position, transform.position);
            lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
if (stunned == false)
            {
                if (idle == true && alreadyRanAway == false)
                {
                    if (distance <= 7)
                    {
                        Debug.Log("Rabbit distance is " + distance);
                        runAway = true;
                        animator.SetBool("Run", true);
                    }
                }
                if (runAway == true && stunned == false)
                {
                    runDirection = transform.position - player.transform.position;
                    lookAwayRotation = Quaternion.LookRotation(transform.position - player.transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookAwayRotation, 3);
                    rabbitRb.AddForce(runDirection * 150);
                    if (distance >= 12)
                    {
                        runAway = false;
                        animator.SetBool("Run", false);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
                        StartCoroutine(PauseBeforeShoot());
                    }
                }
                if (idle == false && whichAttack == attackOne && runAway == false)
                {
                    attack = true;



                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
                    enemyScript.SetDamage(1);
                    enemyScript.SetForce(0);
                    //Need this for archer because then it will keep firingarrows lmao
                    if (attackFinished == false)
                    {
                        FireSingleArrow();
                        StartCoroutine(AttackDuration());
                    }
                }
                else if (idle == false && whichAttack == attackTwo && runAway == false)
                {
                    attack = true;

                    distance = Vector3.Distance(player.transform.position, transform.position);
                    lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                                //StartCoroutine(AttackCountdown());
                    enemyScript.SetDamage(1);
                    enemyScript.SetForce(0);
                    //Need this for archer because then it will keep firingarrows lmao
                    if (attackFinished == false)
                    {
                        Debug.Log("SecondShot");
                        FireSingleArrow();
                        StartCoroutine(DoubleShootDuration());
                    }
                }
            }
            //May not need this because Rabbit will technically not be off the ground
            if (attackFinished == true && isOnGround == true)
            {
                attackFinished = false;
                idleTime = usualIdleTime;
                StartCoroutine(IdleAnimation());
            }
        }
    }
    IEnumerator PauseBeforeShoot()
    {
        idle = true;
        alreadyRanAway = true;
        animator.SetBool("Idle", true);
        yield return new WaitForSeconds(1.5f);
        idle = false;
        alreadyRanAway = false;
        animator.SetBool("Idle", false);
    }
    public void FireSingleArrow()
    {
        //firing position works now that I've rearranged a fewthings..
        //Actually, I think the problem was using Translate and no Time.deltaTime
        ///I tested it out andyes, doing no Time.deltaTime makes the arrow disappear. I think it moved too fast because it
        ///is functioning with frames and there can be millions of frames per second
        animator.SetTrigger("Single Shoot");
        Instantiate(arrow,firingPosition.position, arrow.transform.rotation);
    }
    public void FireSecondArrow()
    {
        //The challenge is that using lookRotat in Projectile makes the arrow fire away from the play
        animator.SetTrigger("Double Shoot");//If this doesn't work, simply do FireSingleArrow()and then do this method and
        //set this as a trigger instead
        
        //Instantiate(arrow, firingPosition.position, firingPosition.rotation);
        if (distance > 8)
        {
            Instantiate(arrow, new Vector3(firingPosition.position.x, firingPosition.position.y + 1, firingPosition.position.z), arrow.transform.rotation);
        }
    }
    //I thought I wouldn't need an AttackDuration, but I need to deactivate the attackrange
    IEnumerator AttackDuration()
    {
        attackFinished = true;
        yield return new WaitForSeconds(0.5f);
        //armadilloCollide.isTrigger = false;
        attackFinished = false;
        attack = false;

        if (whichAttack == attackOne)
        {
            whichAttack = attackTwo;
        }
        else if (whichAttack == attackTwo)
        {
            whichAttack = attackOne;
        }
        //else if (whichAttack == attackTwo)
        //{
        //enemyScript.SetComboFinisher();
        //}
        idleTime = usualIdleTime;
        StartCoroutine(IdleAnimation());
    }
    IEnumerator DoubleShootDuration()
    {
        attackFinished = true;
        yield return new WaitForSeconds(1.5f);
        //armadilloCollide.isTrigger = false;
        attackFinished = false;

        distance = Vector3.Distance(player.transform.position, transform.position);
        lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                    //StartCoroutine(AttackCountdown());
        enemyScript.SetDamage(1);
        enemyScript.SetForce(0);
        if (attackFinished == false)
        {
            FireSecondArrow();
            StartCoroutine(AttackDuration());
        }

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
        //Debug.Log("Attack Start");
        animator.SetBool("Idle", false);
        //animator.SetBool("Chase", true);
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (runAway == true && collision.gameObject.CompareTag("Wall"))
        {
            runAway = false;
            animator.SetBool("Run", false);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
            StartCoroutine(PauseBeforeShoot());
        }
    }
    public void OnCollisionStay(Collision collision)
    {
        if (runAway == true && collision.gameObject.CompareTag("Wall"))
        {
            runAway = false;
            animator.SetBool("Run", false);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
            StartCoroutine(PauseBeforeShoot());
        }
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
            Damaged();
        }
        if (other.CompareTag("Tiger Special"))
        {
            Damaged();
        }
        if (other.CompareTag("Bird Attack Regular"))
        {
            Damaged();
        }
        if (other.CompareTag("Bird Special"))
        {
            Damaged();
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
