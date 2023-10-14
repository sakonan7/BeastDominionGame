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
    private float speed = 80;
    private Rigidbody gorillaRb;
    private Rigidbody playerRb;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    private bool attack = false;
    //private bool beginningIdle = true;
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

    public GameObject regularAttackRange;
    public GameObject secondAttackRange;
    public GameObject slamAttackRange;
    public GameObject slamAttackRange2;
    public GameObject regularShockWave;
    public GameObject bigShockWave;
    public GameObject DMShockWave;
    public ParticleSystem DMShockWaveEffect;
    private SkinnedMeshRenderer skin;
    public Material DMSkin;
    public Material regularSkin;
    public ParticleSystem flameAura;
    public ParticleSystem attackEffect;
    public GameObject fireCrater;
    private Vector3 placeForFireCrater;
    public GameObject fireCraterDM;
    private Projectile craterScript;
    public GameObject warningLightSmall;
    public GameObject warningLightDM;
    private bool slamComing = false;
    private bool desperationMoveOn = false;
    public GameObject motionBlurObject;
    private AudioSource audio;
    public AudioClip monkeyAttack;
    public AudioClip startingSound;
    public AudioClip fieryAura;
    public AudioClip DMSmash;
    private float attackVol;
    private float firstAttackVol = 0.1f;
    private float secondAttackVol = 0.3f;
    private bool playOnce = true;
    public bool isOnGround = false;
    private bool attackString1Part1 = true;
    private bool attackString1Part2 = false;
    private bool attackString2Part1 = false;
    private bool attackString2Part2 = false;
    private bool attackFinished = false;
    private bool slapString = false;
    private bool useSlamAttack = false;
    private float distance;
    private int attackString = 0;

    public GameObject[] rage = new GameObject[13];

    private bool stunned = false; //Freeze Monkey when i don't want it to move and when the Monkey is being stunlocked by att
    private float idleTime;
    private float stringPartTime = 5;
    private float betweenStringsTime = 7;
    private float DMIdleTime = 12;

    private GameManager gameManager;
    private int HP = 80; //7
    public bool testingStun = false;
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
        skin = GetComponentInChildren<SkinnedMeshRenderer>();
        //enemyScript.SetDamage(damage);
        enemyScript.attackEffect[0] = attackEffect;
        audio = GetComponent<AudioSource>();
        enemyScript.enemySounds[0] = monkeyAttack;
        enemyScript.SetHP(HP);
        enemyScript.IsGiantEnemy();
        enemyScript.IsGiantBoss();
        enemyScript.HurtFlying();

        cameraRef = GameObject.Find("Main Camera");
        camScript = cameraRef.GetComponent<ThirdPersonCamera>();

        //StartCoroutine(IdleAnimation());
        StartCoroutine(Cutscene());
        
        originalPosition = transform.position;
        idleTime = stringPartTime;
    }

    // Update is called once per frame
    void Update()
    {
if (testingStun == false)
        {
            //3 Ways to make Gorilla initiate an attack. Cancel out it's closeTheDistance with 3
            //Either it hits a boundaryorit is <= 8 or 16distance away from the player. I think I don't need to have OnCollision or OnCollisionStay
            //becausethat counts as being < 8 distance
            followDirection = (transform.position - player.transform.position).normalized;
            distance = Vector3.Distance(player.transform.position, transform.position);
            lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
            if (idle == true)
            {
                //animator.SetBool("Idle", true);
                animation.Play("Idle");
            }
            if (chase==true)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
                transform.Translate(followDirection * speed * Time.deltaTime);
                if (distance <= 20)
                {
                    chase = false;
                    Debug.Log("Distance Reached");
                }
            }
            //Need to ensure that the gorilla attacks a little bit slow
            //Intention is that even though he fights slow, do not expect him to be that predictable. 
            //Especiallybecause not all his attacks are thesamelevel of slow. (The slow slam vs the fast ) Also, do not recklesslydodge
            //The fast slam comes out faster but has the disadvantage of not allowing him to getrightin from of the player
            //for it
                //if (attackString1Part1 == true)
                //{

           if (slapString == true && chase == false)
           {
              Slap();
           }
                //}
                //if (attackString1Part2 == true)
                //{
                    //if (slapString == true && chase == false)
                    //{
                        //Slap();
                    //}
                //}
            if (useSlamAttack == true && chase ==false)
            {
                idle = false;
                //chase = true;
                slamAttackRange.SetActive(true);
                


                //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                            //StartCoroutine(AttackCountdown());
                Debug.Log(distance);
                //if (chase == true) {
                //if (distance == 21)
                //{
                //chase = false;
                //gorillaRb.velocity = Vector3.zero;



                //}
                //}
                useSlamAttack = false;
            }
            if (slamComing == true)
            {
                StartCoroutine(WarningLightRegular());

                warningLightSmall.transform.position = new Vector3(player.transform.position.x, warningLightSmall.transform.position.y, player.transform.position.z);
            }
            //I think this will be triggered by booldesperationMoveOn
            //Somethingis making the code not animation not cooperate. Not the lights or the way the animation is built. Try
            //getting rid of as many IEnumerators as possib
            //Something is cursed with this code, I tried turning off several IEnumeratorsand justdoing the animation and nothing happ

            if (desperationMoveOn == true)
            {
                StartCoroutine(WarningLightDM());
                lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
                //new code
                //Maybe don't need to play this as audio fullout because the audio clip is long and the code will play out the wholeclip
                //audio.clip = fieryAura;//Just needtocut fire sound to about 3 seconds, before a pause and the louder fire
                audio.PlayOneShot(fieryAura, 0.2f);

            }

        }
    }
    IEnumerator IdleAnimation()
    {
        idle = true;
            yield return new WaitForSeconds(idleTime);
        idle = false;
            chase = true;
        slapString = true;
    }
    IEnumerator Glow()
    {
        yield return new WaitForSeconds(1f);
        regularAttackRange.SetActive(false);
        secondAttackRange.SetActive(true);
        enemyScript.UnsetPlayerDodged();
        enemyScript.BackKnockBack();
    }
    public void Slap()
    {
        regularAttackRange.SetActive(true);
        animation.Play("Hit");
        StartCoroutine(Glow());
        StartCoroutine(SecondHit());
        slapString = false;
        enemyScript.SetDamage(0);
        enemyScript.SetForce(15);
        enemyScript.BackKnockBack();
        idleTime = betweenStringsTime; //Thisisfor ifgorilla used D
    }
    IEnumerator SecondHit()
    {
        enemyScript.SetDamage(0);
        enemyScript.SetForce(15);
        enemyScript.BackKnockBack();
        yield return new WaitForSeconds(2f);

        animation.Play("Hit1");
        //secondAttackRange.SetActive(false);
        //
        StartCoroutine(UnGlow());
        enemyScript.UnsetPlayerDodged();
        enemyScript.BackKnockBack();
    }
    IEnumerator UnGlow()
    {
        yield return new WaitForSeconds(1.5f);
        secondAttackRange.SetActive(false);
        //StartCoroutine(IdleAnimation());
        
        //StartCoroutine(CloseTheDistance());
        if (attackString1Part1 == true)
        {
            StartCoroutine(PauseSlam());
        }
        else if (attackString1Part2 == true)
        {
            //I'm very damn sure I don't close the distance for all slam att
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
            StartCoroutine(SlamDown());
            enemyScript.SetDamage(0);
            enemyScript.SetForce(30);
            enemyScript.SetComboFinisher();
            slamComing = true;
            useSlamAttack = true;
        }
        else if (attackString2Part1 == true)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
            StartCoroutine(SlamDown());
            enemyScript.SetDamage(0);
            enemyScript.SetForce(30);
            enemyScript.SetComboFinisher();
            slamComing = true;
            useSlamAttack = true;
        }
        else if (attackString2Part2 == true)
        {
            StartCoroutine(PauseSlam());
        }
    }
    IEnumerator PauseSlam()
    {
        transform.position = originalPosition;
        animation.Play("Idle");
        yield return new WaitForSeconds(2f);
        useSlamAttack = true;
        chase = true;
        StartCoroutine(CloseTheDistance());
    }
    //I think this is just the extra charge
    IEnumerator CloseTheDistance()
    {
        //transform.Translate(followDirection * speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
        yield return new WaitForSeconds(1);
        StartCoroutine(SlamDown());
        enemyScript.SetDamage(0);
        enemyScript.SetForce(30);
        enemyScript.SetComboFinisher();
        slamComing = true;
    }
    IEnumerator WarningLightRegular()
    {
        warningLightSmall.SetActive(true);
        yield return new WaitForSeconds(0.05f);
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
        yield return new WaitForSeconds(0.1f);
        regularShockWave.SetActive(true);
        motionBlurObject.SetActive(true);
        slamComing = false;
        warningLightSmall.SetActive(false);
        camScript.ScreenShakeMethod();
        placeForFireCrater = regularShockWave.transform.position;
    }
    IEnumerator SlamAttackDuration()
    {
        
        attackFinished = true;
        
        yield return new WaitForSeconds(1f);
        slamAttackRange.SetActive(false);
        regularShockWave.SetActive(false);
        motionBlurObject.SetActive(false);
        //armadilloCollide.isTrigger = false;
        attackFinished = false;
        attack = false;
    
            enemyScript.SetComboFinisher();
        enemyScript.ResetKnockbacks();
        //}
        
        transform.position = originalPosition;
        attackFinished = false;
        StartCoroutine(Crater());

        slapString = false;
        //This will determine what the next attackstring
        if (attackString1Part1 == true)
        {
            attackString1Part1 = false;
            attackString1Part2 = true;
            StartCoroutine(IdleAnimation());
        }
        else if (attackString1Part2 == true)
        {
            attackString1Part2 = false;
            attackString2Part1 = true;
            StartCoroutine(IdleAnimation());
            idleTime = betweenStringsTime;
        }
        else if (attackString2Part1 == true)
        {
            attackString2Part1 = false;
            attackString2Part2 = true;
            StartCoroutine(IdleAnimation());
            idleTime = stringPartTime;
        }
        else if (attackString2Part2 == true)
        {
            attackString2Part2 = false;
            StartCoroutine(NewDMCode());
            enemyScript.HurtFlying();
        }
        enemyScript.UnsetPlayerDodged();
    }
    IEnumerator Crater()
    {
        yield return new WaitForSeconds(0.1f);

        //craterScript = newCrater.GetComponent<Projectile>();
        //craterScript.SetDamage(0);
        //craterScript.IsMoving(false);
        //craterScript.SetLifeTime(3);
        //craterScript.IsDestroyable(false);
        Instantiate(fireCrater, new Vector3(placeForFireCrater.x, fireCrater.transform.position.y, placeForFireCrater.z), fireCrater.transform.rotation);
       
    }
    public void LightsOn()
    {
        for (int i = 0; i < rage.Length; i++)
        {
            rage[i].SetActive(true);
        }
    }
    public void LightsOff()
    {
        for (int i = 0; i < rage.Length; i++)
        {
            rage[i].SetActive(false);
        }
    }

    //Rewrite of DM
    IEnumerator NewDMCode()
    {
        skin.material = DMSkin;
        flameAura.Play();
                    LightsOn();
            //DMStart();
            desperationMoveOn = true;
            enemyScript.BackKnockBack();

        animation.Play("Idle");
        yield return new WaitForSeconds(6);
        desperationMoveOn = false;
        //Instantiate(DMShockWave, DMShockWave.transform.position, DMShockWave.transform.rotation);
        //motionBlurObject.SetActive(true);
        //slamComing = false;
        //camScript.ScreenShakeMethod();
        DMSlamDown();
        skin.material = regularSkin;
        LightsOff();
        flameAura.Stop();
        audio.Stop();
        //idle = false;
    }

    //I turned this into a method so there isn't a wa it time for 
    public void DMSlamDown()
    {

        //enemyScript.RightKnockBack();
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
        yield return new WaitForSeconds(0.2f);
        //bigShockWave.SetActive(true);
        DMShockWave.SetActive(true);
        DMShockWaveEffect.Play();
        //Instantiate(DMShockWave, DMShockWave.transform.position, DMShockWave.transform.rotation);
        motionBlurObject.SetActive(true);
        //slamComing = false;
        //warningLightSmall.SetActive(false);
        camScript.ScreenShakeMethod();
        audio.PlayOneShot(DMSmash, 0.4f);
    }
    IEnumerator DMSlamAttackDuration()
    {

        attackFinished = true;
        //Vector3 placeForFireCraterDM = fireCraterDM.transform.position;
        yield return new WaitForSeconds(1.5f);//Madeit 1.5f instead of1so that the player can't dodge through it as a tig
        //slamAttackRange.SetActive(false);
        //slamAttackRange2.SetActive(false);
        //bigShockWave.SetActive(false);
        DMShockWave.SetActive(false);
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
        enemyScript.HurtFlying();
        //}
        idle = true;
        transform.position = originalPosition;
        attackFinished = false;
        
        StartCoroutine(DMCrater());
        enemyScript.UnsetPlayerDodged();
        StartCoroutine(IdleAnimation());
        audio.Stop();
        attackString1Part1 = true;
        idleTime = DMIdleTime;
    }
    IEnumerator DMCrater()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject newDMCrater = fireCraterDM;
        craterScript = newDMCrater.GetComponent<Projectile>();
        craterScript.SetDamage(0);
        //craterScript.IsMoving(false);
        //craterScript.SetLifeTime(3);
        //craterScript.IsDestroyable(false);
        Instantiate(newDMCrater, fireCraterDM.transform.position, fireCraterDM.transform.rotation);
    }
    IEnumerator Cutscene()
    {
        animation.Play("Sitting Idle");
        yield return new WaitForSeconds(1);
        //idle = true;
        //animator.SetTrigger("Stand Up");
        animation.Play("Stand Up");
        StartCoroutine(StartIdle());
        //audio.PlayOneShot(startingSound, 0.2f);
    }
    IEnumerator StartIdle()
    {
        yield return new WaitForSeconds(1f);
        //idle = true;
        StartCoroutine(IdleAnimation());
    }
    private void OnTriggerEnter(Collider other)
    {
        //I don't think this is going to work because I think the thing it's touching has to be a trig
        //if (other.CompareTag("Ground") && desperationMoveOn == true)
        //{
            //DMShockWave.SetActive(true);
            //motionBlurObject.SetActive(true);
            //slamComing = false;
            //warningLightSmall.SetActive(false);
            //camScript.ScreenShakeMethod();
        //}
        if(other.CompareTag("CantCross") && chase == true)
        {
            chase = false;
            Debug.Log("BoundaryReached");
        }
    }
}
