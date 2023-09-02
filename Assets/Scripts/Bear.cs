using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{
    private Animator animator;
    private GameObject cameraRef;
    private GameObject player;
    private PlayerController playerScript;
    private Enemy enemyScript;
    private float speed = 220;
    private float flurrySpeed = 1000;
    private Rigidbody bearRb;
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

    public GameObject attackRange1;
    public GameObject attackRange2;
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
    private bool isTunneled = false;

    private bool stunned = false; //Freeze Monkey when i don't want it to move and when the Monkey is being stunlocked by att
    private float idleTime;
    private float usualIdleTime = 9;
    private float damageIdleTime = 6;

    private GameManager gameManager;
    private int HP = 25; //7
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
        if (testingStun == false)
        {
            //I'm gonna take out stunned == false because each time a foe is in attack mode, it can't be flinched and
            //they will be set back into IdleAnimation and only have IdleAnimation happen if the foe is not stunned
            if (idle == false && whichAttack == attackTwo)
            {
                
                    if (attackFinished == false)
                    {
                        followDirection = (player.transform.position - transform.position).normalized;
                        distance = Vector3.Distance(player.transform.position, transform.position);
                        lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                        
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                                    //StartCoroutine(AttackCountdown());
                        enemyScript.SetDamage(2);
                        enemyScript.SetForce(0);
                        animator.SetBool("Fury", true);
                        StartCoroutine(FlurryAttack());
                    }
            }

            if (attackFinished == true)
            {
                attackFinished = false;
                idleTime = usualIdleTime;
                StartCoroutine(IdleAnimation());
            }
        }
    }
    //I thought I wouldn't need an AttackDuration, but I need to deactivate the attackrange
    IEnumerator AttackDuration()
    {
        attackRange1.SetActive(true);
        attackRange2.SetActive(true);
        attackFinished = true;
        yield return new WaitForSeconds(0.5f);
        attackRange1.SetActive(false);
        //armadilloCollide.isTrigger = false;
        attackFinished = false;
        attack = false;

        if (whichAttack == attackOne)
        {
            attackRange1.transform.localScale -= new Vector3(0.2f, 0, 0.2f);
        }
        else if (whichAttack == attackTwo)
        {
            enemyScript.SetComboFinisher();
        }
    }
    IEnumerator FlurryAttack() {
        attackRange1.SetActive(true);
        attackRange2.SetActive(true);
        attackFinished = true;
        bearRb.AddForce(followDirection * flurrySpeed);
        yield return new WaitForSeconds(5f);
        attackRange1.SetActive(false);
        attackRange2.SetActive(true);
        //armadilloCollide.isTrigger = false;
        attackFinished = false;
        animator.SetBool("Fury", false);
        Debug.Log("Flurry Attack Done");
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

        animator.SetBool("Idle", false);
        //animator.SetBool("Chase", true);
        
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

