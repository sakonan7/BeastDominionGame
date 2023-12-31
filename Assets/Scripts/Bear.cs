using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{
    private Animator animator;
    private SkinnedMeshRenderer skin;
    public Material regularSkin;
    public Material rageSkin;
    private GameObject cameraRef;
    private GameObject player;
    private PlayerController playerScript;
    private Enemy enemyScript;
    private float speed = 2;
    private float flurrySpeed = 5;
    private Rigidbody bearRb;
    private Rigidbody playerRb;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    private float jumpForce = 70; //Slight jump before attack
    private float attackForce = 1; //May remove attackForce because Monkey doesn't knock chaarcter back a
    private bool attack = false;
    public bool beginningIdle = false;
    private bool idle = true;
    private bool chase = false;
    private bool flurryChase = false;
    private bool revengeMode = false;
    private bool playerStunned = false; //For if the Tiger is hit by the first claw. Tiger will always get hit twice
    private int damage = 1;
    private bool hitThrown = false;
    //This will account for multiple attacks and multiple effects. Might use an array instead
    //I will need to feed this into Enemy, possibly with a method like WhichAttack, so I can feed it into AttackLanded(
    public int attack1 = 0;
    public int attack2 = 1;
    public int attack3 = 2;

    public GameObject attackRange1;
    public GameObject attackRange2;
    public GameObject guardRange;
    public GameObject revengeRange;
    public ParticleSystem attackEffect;
    private AudioSource audio;
    public AudioClip bearAttack;
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

    private bool stunned = false; //Freeze Monkey when i don't want it to move and when the Monkey is being stunlocked by att
    private float idleTime;
    private float usualIdleTime = 9;
    private float flurryIdleTime = 12;
    private float damageIdleTime = 6;

    private GameManager gameManager;
    private int HP = 35; //7
    public bool testingStun = false;
    private bool testingBehaviors = false;
    private bool moveLeft = false;
    private bool moveRight = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        skin = GetComponentInChildren<SkinnedMeshRenderer>();
        enemyScript = GetComponent<Enemy>();
        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody>();
        playerScript = player.GetComponent<PlayerController>();

        bearRb = GetComponent<Rigidbody>();

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
        enemyScript.enemySounds[0] = bearAttack;
        enemyScript.SetHP(HP);
        enemyScript.IsGiantEnemy();

        cameraRef = GameObject.Find("Main Camera");
        StartCoroutine(IdleAnimation());
        //animator.SetBool("Idle", true);
        whichAttack = attackTwo;
    }

    // Update is called once per frame
    //Make Bear idlefor 2 seconds, then make it perform its fury attack. Rinse Rep.
    //Wait 2 seconds, chase player and perform claw. Wait, then do it again. Then after 2 seconds, roar and perform a powerful flurry
    //For Bird, it will not do any of this and simply wait and keep turning towards the bird. If the bird comes close to its front, it performs its claw
    //attack. Then wait before doing it again. It glows red when it is going to dothis
    void Update()
    {
        //transform.rotation = new Quaternion(0, transform.rotation.y, transform.rotation.z,0);

        if (testingStun == false)
        {
            //I'm gonna take out stunned == false because each time a foe is in attack mode, it can't be flinched and
            //they will be set back into IdleAnimation and only have IdleAnimation happen if the foe is not stunned
            if (idle== true)
            {
                animator.SetBool("Idle", true);
            }
           if (chase==true)
            {
                followDirection = (transform.position - player.transform.position).normalized;
                distance = Vector3.Distance(player.transform.position, transform.position);
                lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);//ForgotThatI didn't even havethis
            }
            if (idle == false && chase == true)
            {
                transform.Translate(followDirection * speed * Time.deltaTime);
                //Need to ensure these conditionals don't keep playing over and ov
                if (distance <= 6 && playerScript.birdActive==true &&revengeMode==false)
                {
                    animator.SetBool("Chase", false);
                    
                    chase = false;
                    revengeMode = true;
                    StartCoroutine(Revenge());
                    enemyScript.SetDamage(2);
                    enemyScript.SetForce(20);
                    enemyScript.BackKnockBack();
                    enemyScript.HurtFlying();
                    enemyScript.SetComboFinisher();
                }
                if (distance <= 6 && whichAttack ==attackOne && playerScript.birdActive == false)
                {
                    chase = false;
                    if (attackFinished == false)
                    {
                        animator.SetBool("Chase", false);
                        animator.SetTrigger("Attack1");
                        attackRange1.SetActive(true);
                        enemyScript.SetDamage(2);
                        enemyScript.SetForce(20);
                        enemyScript.BackKnockBack();
                        StartCoroutine(AttackDuration());
                    }
                }
                else if (distance <= 6 && whichAttack == attackTwo && playerScript.birdActive == false)
                {
                    chase = false;
                    animator.SetBool("Chase", false);
                    if (attackFinished == false)
                    {

                        StartCoroutine(Roar());
                        audio.PlayOneShot(bearAttack, 0.4f);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
                        //animator.SetTrigger("Buff");
                        attackFinished = true;
                    }
                }         
            }
            //Accidentally put this in the idle == false && chase == false con
            if (flurryChase == true)
            {
                transform.Translate(followDirection * flurrySpeed * Time.deltaTime);
            }
            if (revengeMode == true)
            {
                animator.SetBool("Idle", true);
                skin.material = rageSkin;
                guardRange.SetActive(true);
                //For some reason, IEnumerators can't do regular addforce, translate ot somethinglikethis
                if (enemyScript.attacked == true && enemyScript.guard == true)
                {
                    animator.SetBool("Idle", false);
                    revengeRange.SetActive(true);
                    animator.SetTrigger("Attack1");
                    //skin.material = regularSkin;
                    revengeMode = false;
                    StartCoroutine(AttackDuration());
                    enemyScript.GuardUntriggered();
                }
            }
            else if (revengeMode == false)
            {
                skin.material = regularSkin;
                guardRange.SetActive(false);
                
            }
            //if (attackFinished == true)
            //{
            //attackFinished = false;
            //idleTime = usualIdleTime;
            //StartCoroutine(IdleAnimation());
            //}
        }
    }
    //I thought I wouldn't need an AttackDuration, but I need to deactivate the attackrange
    IEnumerator AttackDuration()
    {
        //attackRange1.SetActive(true);
        attackFinished = true;
        yield return new WaitForSeconds(1.5f);
        attackRange1.SetActive(false);
        attackFinished = false;
        enemyScript.ResetKnockbacks();
        StartCoroutine(IdleAnimation());
        if (attackRange1.activeSelf == true)
        {
            attackRange1.SetActive(false);
        }
        if (revengeRange.activeSelf == true)
        {
            revengeRange.SetActive(false);
            enemyScript.SetComboFinisher();
        }
        if (enemyScript.canHurtFlying==true)
        {
            enemyScript.HurtFlying(); 
        }
        enemyScript.UnsetPlayerDodged();
    }
    IEnumerator Roar()
    {
        animator.SetBool("Idle", true);
        yield return new WaitForSeconds(3f);
        followDirection = (transform.position - player.transform.position).normalized;
        lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Before I added the above,
        //I didn't even calculate lookRotation, probably because I had the bear calculate it throughout the game
        enemyScript.SetDamage(1);
        enemyScript.SetForce(0);
        animator.SetBool("Idle", false);
        animator.SetBool("Fury", true);
        StartCoroutine(FlurryAttack());
        flurryChase = true;
        StartCoroutine(DamageIncreaser());
    }
    IEnumerator FlurryAttack() {
        attackRange1.SetActive(true);
        attackRange2.SetActive(true);
        attackFinished = true;
        if (enemyScript.hitLanded == true)
        {
            flurryChase = false;
            bearRb.velocity = Vector3.zero;
            bearRb.AddForce(Vector3.back * 100, ForceMode.Impulse);
            Debug.Log("Flurry ChaseOv");
        }
        
        yield return new WaitForSeconds(5f);
        flurryChase = false;
        attackRange1.SetActive(false);
        attackRange2.SetActive(false);
        //armadilloCollide.isTrigger = false;
        attackFinished = false;
        animator.SetBool("Fury", false);
        Debug.Log("Flurry Attack Done");
        idleTime = flurryIdleTime;
        StartCoroutine(IdleAnimation());
        enemyScript.UnsetPlayerDodged();
    }
    IEnumerator DamageIncreaser()
    {
        yield return new WaitForSeconds(2.5f);
        enemyScript.SetDamage(2);
    }
    IEnumerator Revenge()
    {

        yield return new WaitForSeconds(4);
        revengeMode = false;
        chase = true;
        animator.SetBool("Idle", false);
        animator.SetBool("Chase", true);
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
        chase = true;

        animator.SetBool("Idle", false);
        if (whichAttack == attackOne)
        {
            whichAttack = attackTwo;
        }
        else if (whichAttack == attackTwo)
        {
            whichAttack = attackOne;
        }
        
        animator.SetBool("Chase", true);

    }

}

