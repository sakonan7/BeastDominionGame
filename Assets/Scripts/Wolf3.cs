using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf3 : MonoBehaviour
{
    //private new Animation animation;
    private Animator animator;
    //public GameObject HPBar;
    private GameObject cameraRef;
    private GameObject player;
    private PlayerController playerScript;
    private Enemy enemyScript;
    private float walkSpeed = 100;
    private int walkDirection = 0;
    private bool directionChosen = false;
    private float speed = 220;
    private Rigidbody wolfRb;
    private Rigidbody playerRb;
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
    
    public GameObject attackRange;
    public ParticleSystem attackEffect;
    private AudioSource audio;
    public AudioClip wolfAttack;
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
    private int HP = 11; //7
    private bool testingStun = false;
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

        wolfRb = GetComponent<Rigidbody>();
        
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
        enemyScript.enemySounds[0] = wolfAttack;
        enemyScript.SetHP(HP);

        cameraRef = GameObject.Find("Main Camera");

        ChooseDirection();
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
            if (moveLeft == true)
            {
                wolfRb.AddForce(Vector3.left * speed / 2);
            }
            else if (moveRight == true)
            {
                wolfRb.AddForce(Vector3.right * speed / 2);
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
                        wolfRb.AddForce(Vector3.left * walkSpeed);
                    }
                    else if (walkDirection == 1)
                    {
                        wolfRb.AddForce(Vector3.right * walkSpeed);
                    }
                    //I think walking diagonally is good because I think dogs walk diagonally, not side
                    //if (walkUpDownDirection == 0)
                    //{
                    //Debug.Log("Left Walk");
                    //wolfRb.AddForce(Vector3.fwd * walkSpeed);
                    //}
                    //Debug.Log("Right Walk");
                    wolfRb.AddForce(Vector3.back * walkSpeed);
                }

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
                    wolfRb.AddForce(followDirection * speed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
                                                                                                //StartCoroutine(AttackCountdown());
                    if (distance <= 4)
                    {
                        Debug.Log("Reached");
                        animator.SetBool("Dash", false);
                        chase = false;
                        //jumpForce = 3;
                        CorkScrew();
                        //StartCoroutine(AttackDuration());
                        if (attackFinished == false)
                        {
                            attackFinished = true;
                        }
                    }
                }
            }
            // && isOnGround == true
            if (attackFinished == true)
            {
                attackFinished = false;
                idleTime = usualIdleTime;
                ChooseDirection();
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
            audio.PlayOneShot(wolfAttack, attackVol);
        }
    }
    public void PlayAttackEffect()
    {
        attackEffect.Play();
        //wolfAudio.PlayOneShot(wolfAttack, 0.1f);
    }
    IEnumerator AttackDuration()
    {
        //animation.Play("Wolf Corkscrew");
        animator.SetBool("Ground Attack", true);
        //animator.speed = 3;
        //wolfRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);
        //attackRecoil = (transform.position - playerPosition).normalized;
        //wolfRb.AddForce(attackRecoil, ForceMode.Impulse);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5); //In case the the player runs around the Wolf
                                                                                    //right before the att
                                                                                    //Debug.Log("Corkscrew");
                                                                                    //I don't think this is going to make much of a difference, but attack aura keeps spazzing 
        //attackAura.SetActive(true);
        //attackRange.SetActive(true);
        //StartCoroutine(AttackDuration());
        //attackCounter = 1;

        attack = true;
        //wolfRb.constraints = RigidbodyConstraints.FreezeRotation;
        //attackCounter = 1; //Putting it here because regardless, the wolf will not repeat an attack
        if (enemyScript.hitLanded == true)
        {
            enemyScript.SetPlayerDodged();//So player can't be hit multiple times by this att
        }
        enemyScript.BackKnockBack();
        yield return new WaitForSeconds(0.4f);
        animator.SetBool("Ground Attack", false);
        attack = false;
        //attackCounter = 0;
        //attackAura.SetActive(false);
        attackRange.SetActive(false);
        //attackLanded = false;
        Debug.Log("Attack over");
        //JumpBack();
        attackFinished = true;
        enemyScript.UnsetPlayerDodged();
        enemyScript.ResetKnockbacks();
    }
    public void CorkScrew()
    {
            //animation.Play("Wolf Corkscrew");
            //animator.SetBool("Ground Attack", true);
            //animator.speed = 3;
            wolfRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);
            //attackRecoil = (transform.position - playerPosition).normalized;
            //wolfRb.AddForce(attackRecoil, ForceMode.Impulse);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5); //In case the the player runs around the Wolf
                                                                                        //right before the att
                                                                                        //Debug.Log("Corkscrew");
                                                                                        //I don't think this is going to make much of a difference, but attack aura keeps spazzing 
            //attackAura.SetActive(true);
            attackRange.SetActive(true);
            StartCoroutine(AttackDuration());
            //attackCounter = 1;
            //Debug.Log("Attack repeated for some rea");
            //animator.SetBool("Ground Attack", false);
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
        //monkeyRb.AddForce(Vector3.up * 9, ForceMode.Impulse); //For jumping, may need to modify gravity
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
        animator.SetBool("Dash", true);
        //playerScript.monkeyRange.SetActive(true);
        Debug.Log("Cooldown finished");
        directionChosen = false;
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

