using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armadillo : MonoBehaviour
{
    private Animator animator;
    private SkinnedMeshRenderer skin;
    //public GameObject HPBar;
    private GameObject cameraRef;
    private GameObject player;
    private PlayerController playerScript;
    private Enemy enemyScript;
    private float speed = 220;
    private Rigidbody armadilloRb;
    private Rigidbody playerRb;
    private Collider armadilloCollide;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    private float jumpForce = 70; //Slight jump before attack
    private float attackForce = 1; //May remove attackForce because Monkey doesn't knock chaarcter back a
    private bool attack = false;
    public bool beginningIdle = true;
    private bool idle = true;
    private bool tunnelChase = false;
    private Vector3 originalSize;
    private bool playerStunned = false; //For if the Tiger is hit by the first claw. Tiger will always get hit twice
    private int damage = 1;
    private bool hitThrown = false;
    //This will account for multiple attacks and multiple effects. Might use an array instead
    //I will need to feed this into Enemy, possibly with a method like WhichAttack, so I can feed it into AttackLanded(
    public int attack1 = 0;
    public int attack2 = 1;
    public int attack3 = 2;

    public GameObject attackRange;
    public ParticleSystem tunnelingAttackEffect;
    public ParticleSystem tunneling;
    public GameObject separater;
    public ParticleSystem attackEffect;
    private AudioSource audio;
    public AudioClip armadilloAttack;
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
    private bool stunLocked = true;
    private float idleTime;
    private float usualIdleTime = 9;
    private float damageIdleTime = 6;

    private GameManager gameManager;
    private int HP = 10; //7
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
        armadilloCollide = GetComponent<Collider>();
        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody>();
        playerScript = player.GetComponent<PlayerController>();

        armadilloRb = GetComponent<Rigidbody>();
        
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
        enemyScript.enemySounds[0] = armadilloAttack;
        enemyScript.SetHP(HP);

        cameraRef = GameObject.Find("Main Camera");
        StartCoroutine(IdleAnimation());
        //animator.SetBool("Idle", true);
        whichAttack = attackOne;
        originalSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (testingStun == false)
        {
            //I'm gonna take out stunned == false because each time a foe is in attack mode, it can't be flinched and
            //they will be set back into IdleAnimation and only have IdleAnimation happen if the foe is not stunned
if (stunLocked == false)
            {
                if (idle == false && whichAttack == attackOne)
                {
                    attack = true;

                    followDirection = (player.transform.position - transform.position).normalized;
                    distance = Vector3.Distance(player.transform.position, transform.position);
                    lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                    armadilloRb.AddForce(followDirection * speed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                                //StartCoroutine(AttackCountdown());
                    enemyScript.SetDamage(1);
                    enemyScript.SetForce(12);
                    if (distance <= 2.5)
                    {
                        animator.SetBool("Chase", false);
                        if (attackFinished == false)
                        {
                            SpinAttack();
                            StartCoroutine(AttackDuration());
                        }
                    }
                }
                if (idle == false && whichAttack == attackTwo && tunnelChase == true)
                {
                    attack = true;
                    tunneling.Play();
                    skin.enabled = false;

                    if (isTunneled == false)
                    {
                        //Instead, make Armadillo invisible and shrinkhim so that the target still appears on Armadillo but is lower on the ground
                        //Inspired by Vanitas from Kingdom Hearts
                        transform.localScale += new Vector3(0, -3, 0);

                        //transform.Translate(0, 2, 0);
                        isTunneled = true;
                        enemyScript.SetCantBeHit();
                    }
                    followDirection = (player.transform.position - transform.position).normalized;
                    distance = Vector3.Distance(player.transform.position, transform.position);
                    lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                    armadilloRb.AddForce(followDirection * speed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                                //StartCoroutine(AttackCountdown());
                    enemyScript.SetDamage(2);
                    enemyScript.SetForce(6);
                    if (distance <= 3)
                    {
                        animator.SetBool("Chase", false);
                        tunnelChase = false;
                        tunneling.Stop();
                        if (attackFinished == false)
                        {
                            PopUp();
                            StartCoroutine(AttackDuration());
                            enemyScript.SetCantBeHit();
                        }
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
    //AfterPopUp(), if the Armadillo is attacked while in the air, it's automatically thrown back to grown level, with
    //its separater goneifI used it
    public void PopUp()
    {
        armadilloRb.velocity = Vector3.zero;
        //Use velocity for jumping instead. Or use translate and separater. Separater isgood for keeping the player away and not toppling the
        //Armadillo if the player dodgesit
        //armadilloRb.AddForce(Vector3.up * 10, ForceMode.Impulse);
        transform.localScale = originalSize;
        skin.enabled = true;
        isOnGround = false;
        enemyScript.SetComboFinisher();
        enemyScript.BackKnockBack();
        //StartCoroutine(AfterPopUp());
    }
    IEnumerator AfterPopUp()
    {
        yield return new WaitForSeconds(0.5f);
        separater.SetActive(true); //This is for making sure Armadillo doesn't unbalance, maybe use an IEnumerator for this instead
        //Have the IEnumerator make it appear AFTerA thecount
    }
    public void SpinAttack()
    {
        animator.SetTrigger("Spin Attack");
        attackEffect.Play();
        armadilloRb.velocity = Vector3.zero;
        attackRange.transform.localScale += new Vector3(0.2f, 0, 0.2f);
        armadilloRb.angularVelocity = new Vector3(0, 3.14f, 0);
        enemyScript.BackKnockBack();
    }
    //I thought I wouldn't need an AttackDuration, but I need to deactivate the attackrange
    IEnumerator AttackDuration()
    {
        attackRange.SetActive(true);
        attackFinished = true;
        yield return new WaitForSeconds(0.5f);
        attackRange.SetActive(false);
        //armadilloCollide.isTrigger = false;
        //attackFinished = false;
        attack = false;
        
        if (whichAttack == attackOne)
        {
            attackRange.transform.localScale -= new Vector3(0.2f, 0, 0.2f);
            whichAttack = attackTwo;
            attackFinished = true;//Attack One automatically has attackFinished because it stays on the ground for the att
            attackEffect.Stop();
        }
        else if (whichAttack == attackTwo)
        {
            enemyScript.SetComboFinisher();
            whichAttack = attackOne;
            separater.SetActive(false);
        }
        enemyScript.UnsetPlayerDodged();
        enemyScript.ResetKnockbacks();
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
        animator.SetBool("Idle", false);
        animator.SetBool("Chase", true);
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
                if (other.CompareTag("Bird Attack Range"))
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
            //I need to think more about how to keep triggering a stun bool and how to untrigger it
            //if the player stops attacking for a second
            //Stunned();
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
            animator.SetTrigger("Damaged");
        }
    }


    IEnumerator StunLock()
    {
        yield return new WaitForSeconds(3);
        stunLocked = false;//Almostforgot 
        idleTime = damageIdleTime;
        StartCoroutine(IdleAnimation());
    }
}
