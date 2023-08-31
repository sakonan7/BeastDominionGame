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
    private float speed = 220;
    private Rigidbody rabbitRb;
    private Rigidbody playerRb;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    private float jumpForce = 70; //Slight jump before attack
    private float attackForce = 1; //May remove attackForce because Monkey doesn't knock chaarcter back a
    private bool attack = false;

    private bool beginningIdle = false;

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
    private int whichAttack = 1;
    private int attackOne = 0;
    private int attackTwo = 1;
    private bool isTunneled = false;

    private bool stunned = false; //Freeze Monkey when i don't want it to move and when the Monkey is being stunlocked by att
    private float idleTime;
    private float usualIdleTime = 9;
    private float damageIdleTime = 6;

    private GameManager gameManager;
    private int HP = 10; //7
    private bool testingStun = false;
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
            if (idle == false && whichAttack == attackOne)
            {
                attack = true;
                
                distance = Vector3.Distance(player.transform.position, transform.position);
                lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                rabbitRb.AddForce(followDirection * speed);
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
            if (idle == false && whichAttack == attackTwo && tunnelChase == true && stunned == false)
            {
                attack = true;
                followDirection = (player.transform.position - transform.position).normalized;
                distance = Vector3.Distance(player.transform.position, transform.position);
                lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                rabbitRb.AddForce(followDirection * speed);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                            //StartCoroutine(AttackCountdown());
                enemyScript.SetDamage(2);
                enemyScript.SetForce(0);
                    if (attackFinished == false)
                    {
                        //PopUp();
                        StartCoroutine(AttackDuration());
                        enemyScript.SetCantBeHit();
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
    public void FireSingleArrow()
    {
        //firing position works now that I've rearranged a fewthings..
        //Actually, I think the problem was using Translate and no Time.deltaTime
        Instantiate(arrow,firingPosition.position, firingPosition.rotation);
    }
    public void FireSecondArrow()
    {
        //firing position works now that I've rearranged a fewthings..
        //Actually, I think the problem was using Translate and no Time.deltaTime
        Instantiate(arrow, new Vector3(firingPosition.position.x, firingPosition.position.y + 2, firingPosition.position.z), firingPosition.rotation);
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
        }
        else if (whichAttack == attackTwo)
        {
            enemyScript.SetComboFinisher();
        }
        idleTime = usualIdleTime;
        StartCoroutine(IdleAnimation());
    }

    IEnumerator IdleAnimation()
    {
        idle = true;
        //animator.SetBool("Idle", true);
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
        Debug.Log("Attack Start");
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
