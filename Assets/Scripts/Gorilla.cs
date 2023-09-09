using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorilla : MonoBehaviour
{
    private new Animation animation;
    private Animator animator;
    //public GameObject HPBar;
    private GameObject cameraRef;
    private ThirdPersonCamera camScript;
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
    public GameObject slamAttackRange2;
    public GameObject regularShockWave;
    public GameObject bigShockWave;
    public GameObject DMShockWave;
    public ParticleSystem attackEffect;
    public GameObject fireCrater;
    public GameObject fireCraterDM;
    private Projectile craterScript;
    public GameObject warningLightSmall;
    public GameObject warningLightDM;
    private bool slamComing = false;
    private bool desperationMoveOn = false;
    public GameObject motionBlurObject;
    private AudioSource audio;
    public AudioClip monkeyAttack;
    private float attackVol;
    private float firstAttackVol = 0.1f;
    private float secondAttackVol = 0.3f;
    private bool playOnce = true;
    public bool isOnGround = false;
    private bool attackFinished = false;
    private float distance;

    public GameObject[] rage = new GameObject[13];

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
        animator = GetComponent<Animator>();
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
        camScript = cameraRef.GetComponent<ThirdPersonCamera>();

        //StartCoroutine(IdleAnimation());
        StartCoroutine(Cutscene());
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //3 Ways to make Gorilla initiate an attack. Cancel out it's closeTheDistance with 3
        //Either it hits a boundaryorit is <= 8 or 16distance away from the player. I think I don't need to have OnCollision or OnCollisionStay
        //becausethat counts as being < 8 distance
        followDirection = (transform.position - player.transform.position).normalized;
        if (idle == true)
        {
            //animator.SetBool("Idle", true);
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
        if (slamComing == true)
        {
            StartCoroutine(WarningLightRegular());

            warningLightSmall.transform.position = new Vector3(player.transform.position.x, warningLightSmall.transform.position.y, player.transform.position.z);
        }
        //I think this will be triggered by booldesperationMoveOn
        //Somethingis making the code not animation not cooperate. Not the lights or the way the animation is built. Try
        //getting rid of as many IEnumerators as possib
        if (Input.GetKeyDown(KeyCode.T))
        {
            enemyScript.SetDamage(8);
            enemyScript.SetForce(30);
            enemyScript.SetComboFinisher();
            
            for(int i = 0; i < rage.Length; i++)
            {
                rage[i].SetActive(true);
            }
            DMStart();
            
        }
        if (desperationMoveOn == true)
        {
            StartCoroutine(WarningLightDM());
            lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
        }
        if (desperationMoveOn == false)
        {
            for (int i = 0; i < rage.Length; i++)
            {
                rage[i].SetActive(false);
            }
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
        enemyScript.SetForce(30);
        enemyScript.SetComboFinisher();
        slamComing = true;
    }
    IEnumerator WarningLightRegular()
    {
        warningLightSmall.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        warningLightSmall.SetActive(false);
    }
    IEnumerator WarningLightBig()
    {
        warningLightSmall.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        warningLightSmall.SetActive(false);
    }
    IEnumerator WarningLightDM()
    {
        warningLightDM.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        warningLightDM.SetActive(false);
    }
   //What I could potential do is have the shockwaves trigger the second the Gorilla's arms touch the ground because they do touch theground 
    IEnumerator SlamDown()
    {
        
        enemyScript.RightKnockBack();
        yield return new WaitForSeconds(0.2f);
        animation.Play("Single Slam");
        //Potentially move the Gorill move a few inches closer so that it's fist is closer to the aren
        StartCoroutine(SlamAttackDuration());
        StartCoroutine(ShockWaveAppears());
        
        //Place warningLightSmall in exactly where the player is for a directhit
        //StartCoroutine(WarningLightRegular());
    }
    IEnumerator ShockWaveAppears()
    {
        yield return new WaitForSeconds(0.5f);
        regularShockWave.SetActive(true);
        motionBlurObject.SetActive(true);
        slamComing = false;
        warningLightSmall.SetActive(false);
        camScript.ScreenShakeMethod();
    }
    IEnumerator SlamAttackDuration()
    {
        
        attackFinished = true;
        Vector3 placeForFireCrater = regularShockWave.transform.position;
        yield return new WaitForSeconds(1f);
        slamAttackRange.SetActive(false);
        regularShockWave.SetActive(false);
        motionBlurObject.SetActive(false);
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
        enemyScript.UnsetPlayerDodged();
    }

    public void DMStart()
    {
        //animator.SetTrigger("DM Start Up");
        animation.Play("Roar");
        StartCoroutine(StartUpFists());
    }
    IEnumerator StartUpFists()
    {
        yield return new WaitForSeconds(0.5f);
        slamAttackRange.SetActive(true);
        slamAttackRange2.SetActive(true);
        //Remove the tags from the fists ATM so that they don't hurt the player during the charge 
        StartCoroutine(DesperationMove());
    }
    //For the arena lighting up, warning the player that the whole arena will be consumed in fire
    //I think I will adjust the light source itself to create the light lighting up and down
    //Do another version for the single hand smash shock
    IEnumerator DesperationMove()
    {
        //while(ultimateAttackStart)
        //{
        //yield return new WaitForSeconds(0.5f);
        //}
        //animator.SetBool("DM Charge Up", true);
        desperationMoveOn = true;
        yield return new WaitForSeconds(3);
        
        desperationMoveOn = false;

        DMSlamDown();
        //animator.SetBool("DM Charge Up", false);
    }
    //I turned this into a method so there isn't a wa it time for 
    public void DMSlamDown()
    {

        enemyScript.RightKnockBack();
        //yield return new WaitForSeconds(0.2f);
        animation.Play("Desperation Move"); //This isn't playing at all for some reason, even after I turned this into
        //a meth
        //Potentially move the Gorill move a few inches closer so that it's fist is closer to the aren
        StartCoroutine(DMSlamAttackDuration());
        StartCoroutine(DMShockWaveAppears());

        //Place warningLightSmall in exactly where the player is for a directhit
        //StartCoroutine(WarningLightRegular());
    }
    IEnumerator DMShockWaveAppears()
    {
        yield return new WaitForSeconds(0.5f);
        bigShockWave.SetActive(true);
        //DMShockWave.SetActive(true);
        Instantiate(DMShockWave, DMShockWave.transform.position, DMShockWave.transform.rotation);
        motionBlurObject.SetActive(true);
        //slamComing = false;
        warningLightSmall.SetActive(false);
        camScript.ScreenShakeMethod();
    }
    IEnumerator DMSlamAttackDuration()
    {

        attackFinished = true;
        //Vector3 placeForFireCraterDM = fireCraterDM.transform.position;
        yield return new WaitForSeconds(1f);
        slamAttackRange.SetActive(false);
        slamAttackRange2.SetActive(false);
        bigShockWave.SetActive(false);
        //DMShockWave.SetActive(false);
        motionBlurObject.SetActive(false);
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
        GameObject newDMCrater = fireCraterDM;
        craterScript = newDMCrater.GetComponent<Projectile>();
        craterScript.SetDamage(8);
        //craterScript.IsMoving(false);
        //craterScript.SetLifeTime(3);
        //craterScript.IsDestroyable(false);
        Instantiate(newDMCrater, fireCraterDM.transform.position, fireCraterDM.transform.rotation);
        enemyScript.UnsetPlayerDodged();
    }
    IEnumerator Cutscene()
    {
        animation.Play("Sitting Idle");
        yield return new WaitForSeconds(1);
        //idle = true;
        //animator.SetTrigger("Stand Up");
        animation.Play("Stand Up");
        StartCoroutine(StartIdle());
    }
    IEnumerator StartIdle()
    {
        yield return new WaitForSeconds(1f);
        idle = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        //I don't think this is going to work because I think the thing it's touching has to be a trig
        if (other.CompareTag("Ground") && desperationMoveOn == true)
        {
            DMShockWave.SetActive(true);
            motionBlurObject.SetActive(true);
            slamComing = false;
            warningLightSmall.SetActive(false);
            camScript.ScreenShakeMethod();
        }
    }
}
