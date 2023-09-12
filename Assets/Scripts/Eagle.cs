using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : MonoBehaviour
{
    private new Animation animation;
    //private Animator animator;
    //public GameObject HPBar;
    private GameObject cameraRef;
    private GameObject player;
    private PlayerController playerScript;
    private Enemy enemyScript;
    private float speed = 25;
    private float distanceCloserSpeed = 30;
    private int walkDirection = 0;
    private bool directionChosen = false;
    private Rigidbody eagleRb;
    private Rigidbody playerRb;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    private float jumpForce = 70; //Slight jump before attack
    private bool attack = false;
    private bool beginningIdle = true;
    private bool idle = true;
    private bool chase = false;
    private bool distanceCloser;
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
    public AudioClip eagleCry;
    public AudioClip eagleAttack;
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
      animation = GetComponent<Animation>();
        enemyScript = GetComponent<Enemy>();

        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody>();
        playerScript = player.GetComponent<PlayerController>();

        eagleRb = GetComponent<Rigidbody>();
        
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
        enemyScript.enemySounds[0] = eagleAttack;
        enemyScript.SetHP(HP);
        enemyScript.SetFlying();
        enemyScript.HasRevengeValue();

        cameraRef = GameObject.Find("Main Camera");

        StartCoroutine(IdleAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        if (testingBehaviors == true)
        {
            if (moveLeft == true)
            {
                eagleRb.AddForce(Vector3.left * speed / 2);
            }
            else if (moveRight == true)
            {
                eagleRb.AddForce(Vector3.right * speed / 2);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                moveLeft = !moveLeft;
                moveRight = !moveRight;
            }
        }
        if (testingStun == false)
        {
            if (stunned == false)
            {
                if (idle == true)
                {
                    if (walkDirection == 0)
                    {
                        eagleRb.AddForce(Vector3.left * speed);
                    }
                    else if (walkDirection == 1)
                    {
                        eagleRb.AddForce(Vector3.right * speed);
                    }
                }
                if (idle == false && chase == true)
                {
                    //animation.Play("Run");
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
                    eagleRb.AddForce(followDirection * speed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                                //StartCoroutine(AttackCountdown());
                    if (distance <= 8)
                    {
                        //Switched from tigerActive to isFlying because the eaglewill use the swooping attacking when
                        //the bird is low
                        chase = false;
                        if (playerScript.isFlying == true)
                        {
                            //flyingHit, this bool will tell attackDuration to put the bird back
                        }
                        if (playerScript.isFlying == false)
                        {
                            //flyingHit, this bool will tell attackDuration to put the bird back
                            transform.Translate(0, -1.5f, 0);
                            enemyScript.SetFlying();
                        }
                        distanceCloser = true;
                    }
                }
                if (distanceCloser == true)
                {
                    eagleRb.AddForce(followDirection * distanceCloserSpeed);
                    StartCoroutine(AttackDuration());
                    if (enemyScript.hitLanded == true)
                    {
                        StartCoroutine(Lag());
                        enemyScript.ResetHitLanded();
                        eagleRb.velocity = Vector3.zero;
                    }
                }
            }
            //if (attackFinished == true && isOnGround == true)
            //{
                //attackFinished = false;
                //idleTime = usualIdleTime;
                //StartCoroutine(IdleAnimation());
            //}
        }

        //HPBar.transform.LookAt(HPBar.transform.position - (cameraRef.transform.position - HPBar.transform.position));

    }
    private void LateUpdate()
    {
        if (enemyScript.hitLanded == true && playOnce == true)
        {
            playOnce = false;
            attackEffect.Play();
            audio.PlayOneShot(eagleAttack, attackVol);
        }
    }
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
            //monkeyRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);
            //monkeyRb.AddForce(Vector3.up * 2, ForceMode.Impulse); //For jumping, may need to modify gravity
                                                                  //attackCount++;
        }
        else if (playerScript.birdActive == true)
        {
            //followDirection = (bird.transform.position - transform.position).normalized;
            //monkeyRb.AddForce(followDirection, ForceMode.Impulse);
            //monkeyRb.AddForce(Vector3.up * 5, ForceMode.Impulse); //For jumping, may need to modify gravity

        }
        //If that doesn't work, put an if (dodge == false

        //animation.Play("Attack");
        //animator.SetTrigger("Slash 1");
        isOnGround = false; //Will always have this happen, because both attacks make the Monkey jump
        //}
        //if (enemyScript.hitLanded == true)
        //{
        //PlayAttackEffect();
        //}
        //monkeyRb.constraints = RigidbodyConstraints.FreezeRotation;
        attackVol = firstAttackVol;
        yield return new WaitForSeconds(1.5f);
        //animator.SetBool("Attack 1", false);
        attack = false;
        firstClawSlash.SetActive(false);
        attackRange.SetActive(false);
        //For simplicity, second claw attack will only happen if player was hit by the first
        if (playerScript.tigerActive == true && enemyScript.hitLanded == true)
        {
            //hitCount = 1;
            //StartCoroutine(SecondClaw());
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
    //AttackDuration And an attacklag. The AttackDuration is short and the attack lag is around 2 seconds long. 
    //Forthe ground to air att. The attacklag is shorter for when the player is abird


    IEnumerator PauseBeforeJump()
    {
        yield return new WaitForSeconds(0.5f);
        //Jump();
    }
    //Test to see if the lag time I'm looking for is here
    IEnumerator LagBeforeAttack()
    {
        yield return new WaitForSeconds(1f);
        //StartCoroutine(AirSlash());
        attack = false;
        Debug.Log("Start Air Attack");
    }
    IEnumerator AttackDuration()
    {
        attack = true;
        attackRange.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        attack = false;
        eagleRb.velocity = Vector3.zero;
        attackRange.SetActive(false);
    }
    IEnumerator Lag()
    {
        yield return new WaitForSeconds(2);
        if (enemyScript.isFlying == false)
        {
            transform.Translate(0, 1.5f, 0);
        }
        StartCoroutine(IdleAnimation());
    }
    public void ChooseDirection()
    {
        //0 == left walk, 1 == right walk
        walkDirection = Random.Range(0, 2);
        directionChosen = true;
        //Debug.Log(walkDirection);
        Debug.Log("Direction Chosen");
    }
    IEnumerator IdleAnimation()
    {
        idle = true;
        animation.Play("Idle");
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
        directionChosen = false;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && distanceCloser ==true)
        {
            distanceCloser = false;
            StartCoroutine(Lag());
            enemyScript.ResetHitLanded();
            eagleRb.velocity = Vector3.zero;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        //Attacks will reset its revenge value, but for the bird, nothing will happen when it reaches
        if (other.CompareTag("Tiger Attack Regular"))
        {
            Damaged();
        }
        if (other.CompareTag("Tiger Special"))
        {
            Damaged();
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
    //I need a code that takes into account if the player stops hitting the bird before its revenge value is
    public void Damaged()
    {
        if (attack == false)
        {
            Stunned();
            idle = false; //Putting this here to make sure its idle animation is cancelled and restarted
            //each time the player stops attack
        }
        if (enemyScript.currentRevengeValue == enemyScript.revengeValueCount)
        {
            enemyScript.ResetRevengeValue();
            if (enemyScript.isFlying ==false)
            {
                transform.Translate(0, 1.5f, 0);
                playerRb.velocity = Vector3.back * 10;
                //StartCoroutine(SolidForSeconds());
                StartCoroutine(IdleAnimation());
            }
        }
    }
    public void Stunned()
    {
        StartCoroutine(StunnedDuration());
    }
    IEnumerator StunnedDuration()
    {
        stunned = true;
        animation.Play("Damage Monkey");
        yield return new WaitForSeconds(3f);
        stunned = false;
        idleTime = damageIdleTime;
        transform.Translate(0, 1.5f, 0);
        StartCoroutine(IdleAnimation());
    }
}
