using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey : MonoBehaviour
{
    //private new Animation animation;
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
    private float jumpForce = 70; //Slight jump before attack
    private float attackForce = 1; //May remove attackForce because Monkey doesn't knock chaarcter back a
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
    public GameObject attackRange;
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
    private int HP = 10; //7
    public bool testingStun = false;
    private bool testingBehaviors = false;
    private bool moveLeft = false;
    private bool moveRight = true;
    // Start is called before the first frame update
    void Start()
    {
        //animation = GetComponent<Animation>();
        animator = GetComponent<Animator>();
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

        StartCoroutine(IdleAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        //HPBar.transform.position = new Vector3(transform.position.x, transform.position.y + 1.9f, transform.position.z + 0.1f);
        //Monkey will only do it's chase while it's on the ground to avoid antigravity business
        if (testingBehaviors == true)
        {
            //Maybe Use A Button Press To Make Monkey Change Movement Directions And See If Tiger Keeps FollowingIt.
            if (moveLeft ==  true)
            {
                monkeyRb.AddForce(Vector3.left * speed / 2);
            }
            else if (moveRight == true)
            {
                monkeyRb.AddForce(Vector3.right * speed / 2);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                moveLeft = !moveLeft;
                moveRight = !moveRight;
            }
        }
        if (testingStun == false)
        {
//Less necessary because Monkey technically onlyhas one attack,but doing this for consistenc
if (stunned == false)
            {
                if (idle == false && chase == true)
                {
                    //animation.Play("Run");

                    //I think i should do the direction following outside of this
                    //Either way, it looks like the Monkey only does it at the start and never rotates to face the tiger again
                    //Vector3 newDirection;
                    //if (playerScript.tigerActive == true)
                    //{
                    followDirection = (player.transform.position - transform.position).normalized;
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
                    lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                    monkeyRb.AddForce(followDirection * speed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                                //StartCoroutine(AttackCountdown());
                    if (distance <= 1.8)
                    {
                        animator.SetBool("Chase", false);
                        chase = false;
                        //jumpForce = 60; //Went from 50 to 60 because I want some knockback force from the first att
                        //StartCoroutine(FirstClaw());
                        //}
                        //else if (distance <= 3)
                        //{
                        //animator.SetBool("Chase", false);
                        //chase = false;
                        jumpForce = 3;
                        StartCoroutine(FirstClaw());
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

            //HPBar.transform.LookAt(HPBar.transform.position - (cameraRef.transform.position - HPBar.transform.position));

    }
    private void LateUpdate()
    {
        if (enemyScript.hitLanded == true && playOnce == true)
        {
            playOnce = false;
            attackEffect.Play();
            audio.PlayOneShot(monkeyAttack, attackVol);
        }
    }
    //Change code. Monkey will start by running at the character and then within range, attack. Afterwards, the monkey will wait 4 seconds
    //Before attacking again
    IEnumerator FirstClaw()
    {
        //Want to expand the Monkey's collider slightly
        //StartCoroutine(Attack());
        //Debug.Log("First Claw");

        //May want to do a counter so that an attack doesn't re
        attack = true;
        firstClawSlash.SetActive(true);
        attackRange.SetActive(true);
        //enemyScript.SetAttackEffect(attackEffect); //Doing this for practice for when I have enemies with multiple attacks
        //Necessary because there's enough time for the Monkey to repeat an attack on the bird
        //May not be necessary after my edit to the collider
        enemyScript.SetAttackDirection(followDirection);
        enemyScript.SetForce(0);
        //lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        //monkeyRb.AddForce(followDirection * speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5); //For extra right target turning
        //if (stunned == false && playerStunned == false && playerScript.specialInvincibility == false)
        //{
        //if (hitOnce == false)
        //{
        if (playerScript.tigerActive == true)
            {
                //followDirection = (tiger.transform.position - transform.position).normalized;
                monkeyRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);
                monkeyRb.AddForce(Vector3.up * 2, ForceMode.Impulse); //For jumping, may need to modify gravity
                                                                      //attackCount++;
            }
            else if (playerScript.birdActive == true)
            {
                //followDirection = (bird.transform.position - transform.position).normalized;
                //monkeyRb.AddForce(followDirection, ForceMode.Impulse);
                monkeyRb.AddForce(Vector3.up * 5, ForceMode.Impulse); //For jumping, may need to modify gravity
                
            }
        //If that doesn't work, put an if (dodge == false

        //animation.Play("Attack");
        animator.SetTrigger("Slash 1");
        isOnGround = false; //Will always have this happen, because both attacks make the Monkey jump
        //}
        //if (enemyScript.hitLanded == true)
        //{
        //PlayAttackEffect();
        //}
        monkeyRb.constraints = RigidbodyConstraints.FreezeRotation;
        attackVol = firstAttackVol;
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("Attack 1", false);
        attack = false;
        firstClawSlash.SetActive(false);
        attackRange.SetActive(false);
        //For simplicity, second claw attack will only happen if player was hit by the first
        if (playerScript.tigerActive == true && enemyScript.hitLanded == true)
        {
            //hitCount = 1;
            StartCoroutine(SecondClaw());
            enemyScript.SetComboAttack(); //I put this here because if the IEnumerator goes on too long, it'll set the comboAttack repeatedly
        }
        else
        {
            attackFinished = true; //IdleAnimation() is not played here because it plays in Update when the Monkey has returned to the 
            //playerStunned = false; //Because a second atack will not be made on the bird
        }

        enemyScript.ResetHitLanded();
        //Debug.Log("First Hit");
        playOnce = true;
        enemyScript.UnsetPlayerDodged();
    }

    //Combos will be complicated because I need the combo finisher to trigger stunInvincibility, but also, it needs to land
    //This will be a handwave because this combo only happens if the first hit of the combo
    IEnumerator SecondClaw()
    {
        //Need a cooldown period before this that is only a second
        //Debug.Log("Second claw");
        attack = true;
        attackRange.SetActive(true);
        //StartCoroutine(Attack());
        followDirection = (player.transform.position - transform.position).normalized;
        //enemyScript.SetAttackEffect(attackEffect);
        enemyScript.SetAttackDirection(followDirection);
        enemyScript.BackKnockBack();
        enemyScript.SetForce(6);
        monkeyRb.AddForce(followDirection * (jumpForce/2), ForceMode.Impulse);
        monkeyRb.AddForce(Vector3.up * 5, ForceMode.Impulse); //For jumping, may need to modify gravity
        //animation.Play("Attack");
        animator.SetTrigger("Slash 2");
        secondClawSlash.SetActive(true);
        
        enemyScript.SetComboFinisher();

        monkeyRb.constraints = RigidbodyConstraints.FreezeRotation;
        attackVol = secondAttackVol;
        yield return new WaitForSeconds(1f);
        animator.SetBool("Attack 2", false);
        //StartCoroutine(StartCoolDown());
        attackFinished = true;
        attack = false;
        secondClawSlash.SetActive(false);
        attackRange.SetActive(false);
        //Debug.Log("Start Cool");
        enemyScript.ResetHitLanded();
        enemyScript.ResetHitNumber();
        enemyScript.SetComboAttack();
        enemyScript.SetComboFinisher();

        //hitLanded = false;
        //Debug.Log("Second Hit");
        //Debug.Log("Combo Finisher is " + enemyScript.comboFinisher);
        playOnce = true;
        enemyScript.UnsetPlayerDodged();
        enemyScript.ResetKnockbacks();
    }
    //public void PlayAttackEffect()
    //{
        //attackEffect.Play();
        //wolfAudio.PlayOneShot(wolfAttack, 0.1f);
    //}
    //Needed because the forward movement from the force causes the monkey to jump in an arc instead of upwards as intended
    IEnumerator PauseBeforeJump()
    {
        yield return new WaitForSeconds(0.5f);
        Jump();
    }
    public void Jump()
    {
        monkeyRb.AddForce(Vector3.up * 9, ForceMode.Impulse); //For jumping, may need to modify gravity
        isOnGround = false;
        StartCoroutine(LagBeforeAttack());
        //Will rely on colliders for isOnGround = true;
    }
    //Test to see if the lag time I'm looking for is here
    IEnumerator LagBeforeAttack()
    {
        yield return new WaitForSeconds(1f);
        //StartCoroutine(AirSlash());
        attack = false;
        Debug.Log("Start Air Attack");
    }
    IEnumerator IdleAnimation()
    {
        idle = true;
        //animation.Play("Idle");
        animator.SetBool("Idle", true);
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
    //IEnumerator Fall()
   // {
        //fallingDown = true;
        //animation.Play();
    //}



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
        yield return new WaitForSeconds(3f);
        animator.SetBool("Damaged", false);
        stunned = false;
        idleTime = damageIdleTime;
        StartCoroutine(IdleAnimation());
    }
}
