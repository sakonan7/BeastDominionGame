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
    private float speed = 100;
    private Rigidbody gorillaRb;
    private Rigidbody playerRb;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    private bool attack = false;
    private bool beginningIdle = true;
    private bool idle = false;
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
    public GameObject regularShockWave;
    public ParticleSystem attackEffect;
    public GameObject fireCrater;
    private Projectile craterScript;
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
    private int HP = 60; //7
    private bool testingStun = true;
    private bool testingBehaviors = false;
    private bool moveLeft = false;
    private bool moveRight = true;
    private Vector3 originalPosition;
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

        //gorillaRb = GetComponent<Rigidbody>();
        
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
        enemyScript.IsGiantEnemy();

        cameraRef = GameObject.Find("Main Camera");

        //StartCoroutine(IdleAnimation());
        StartCoroutine(Cutscene());
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        followDirection = (transform.position - player.transform.position).normalized;
        if (idle == true)
        {
            animation.Play("Idle");
        }
        if (Input.GetKeyDown(KeyCode.H) && attackFinished == false)
        {
            idle = false;
            //chase = true;
            slamAttackRange.SetActive(true);
            StartCoroutine(CloseTheDistance());
            distance = Vector3.Distance(player.transform.position, transform.position);
            lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
            
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                        //StartCoroutine(AttackCountdown());
            Debug.Log(distance);
            //if (chase == true) {
            //if (distance == 21)
            //{
            //chase = false;
            //gorillaRb.velocity = Vector3.zero;



            //}
            //}
            attackFinished = true;
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
    IEnumerator CloseTheDistance()
    {
        transform.Translate(followDirection * speed * Time.deltaTime);
        yield return new WaitForSeconds(1);
        StartCoroutine(SlamDown());
        enemyScript.SetDamage(3);
        enemyScript.SetForce(15);
        enemyScript.SetComboFinisher();

    }
    IEnumerator SlamDown()
    {
        
        enemyScript.RightKnockBack();
        yield return new WaitForSeconds(0.5f);
        animation.Play("Single Slam");
        //Potentially move the Gorill move a few inches closer so that it's fist is closer to the aren
        StartCoroutine(SlamAttackDuration());
        StartCoroutine(ShockWaveAppears());
    }
    IEnumerator ShockWaveAppears()
    {
        yield return new WaitForSeconds(0.5f);
        regularShockWave.SetActive(true);
    }
    IEnumerator SlamAttackDuration()
    {
        
        attackFinished = true;
        Vector3 placeForFireCrater = regularShockWave.transform.position;
        yield return new WaitForSeconds(1f);
        slamAttackRange.SetActive(false);
        regularShockWave.SetActive(false);
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
        enemyScript.ResetKnockbacks();
        //}
        idle = true;
        transform.position = originalPosition;
        attackFinished = false;
        GameObject newCrater = fireCrater;
        craterScript = newCrater.GetComponent<Projectile>();
        craterScript.SetDamage(2);
        //craterScript.IsMoving(false);
        //craterScript.SetLifeTime(3);
        //craterScript.IsDestroyable(false);
        Instantiate(newCrater, new Vector3(placeForFireCrater.x, fireCrater.transform.position.y, placeForFireCrater.z), fireCrater.transform.rotation);
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
    IEnumerator Cutscene()
    {
        animation.Play("Sitting Idle");
        yield return new WaitForSeconds(1);
        idle = true;
    }
}
