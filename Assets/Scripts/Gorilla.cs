using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorilla : MonoBehaviour
{
    private new Animation animation;
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
    private bool attack = false;
    private bool beginningIdle = true;
    private bool idle = true;
    private bool chase = false;
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
    public GameObject slamAttackRange;
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
    private int HP = 7; //7
    private bool testingStun = true;
    private bool testingBehaviors = false;
    private bool moveLeft = false;
    private bool moveRight = true;
    // Start is called before the first frame update

    private bool ultimateAttackStart = false;
    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animation>();
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

        //StartCoroutine(IdleAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        if (idle == true)
        {
            animation.Play("Sitting Idle");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            idle = false;
            StartCoroutine(SlamDown());
            enemyScript.SetDamage(3);
            enemyScript.SetForce(10);
            enemyScript.SetComboFinisher();
        }
    }
    IEnumerator IdleAnimation()
    {
        idle = true;
        //animation.Play("Idle");
        //animator.SetBool("Idle", true);
        //For the timebeing, turn off the player's monkey range
        //playerScript.monkeyRange.SetActive(false);
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
        animator.SetBool("Idle", false);
        animator.SetBool("Chase", true);
        //playerScript.monkeyRange.SetActive(true);
        //Debug.Log("Cooldown finished");
    }
    IEnumerator SlamDown()
    {
        slamAttackRange.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        animation.Play("Single Slam");
        //Potentially move the Gorill move a few inches closer so that it's fist is closer to the aren
        StartCoroutine(SlamAttackDuration());
    }
    IEnumerator SlamAttackDuration()
    {
        
        attackFinished = true;
        yield return new WaitForSeconds(1f);
        slamAttackRange.SetActive(false);
        //armadilloCollide.isTrigger = false;
        attackFinished = false;
        attack = false;

        //if (whichAttack == attackOne)
        //{
            //attackRange.transform.localScale -= new Vector3(0.2f, 0, 0.2f);
        //}
        //else if (whichAttack == attackTwo)
        //{
            enemyScript.SetComboFinisher();
        //}
        idle = true;
    }

    //For the arena lighting up, warning the player that the whole arena will be consumed in fire
    //I think I will adjust the light source itself to create the light lighting up and down
    //Do another version for the single hand smash shock
    IEnumerator UltimateAttackChargeUp()
    {
        while(ultimateAttackStart)
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}
