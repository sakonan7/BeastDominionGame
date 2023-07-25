using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey : MonoBehaviour
{
    private new Animation animation;
    public GameObject HPBar;
    private GameObject cameraRef;
    private GameObject player;
    private GameObject tiger;
    private GameObject bird;
    private Rigidbody tigerRB;
    private Rigidbody birdRB;
    private PlayerController playerScript;
    private Enemy enemyScript;
    private float speed = 52;
    private Rigidbody monkeyRb;
    private Rigidbody playerRb;
    private Collider monkeyAttackReach;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    private float jumpForce = 4; //Slight jump before attack
    private float attackForce = 1; //May remove attackForce because Monkey doesn't knock chaarcter back a
    private bool attack = false;
    private bool cooldown = false;
    private bool playerStunned = false; //For if the Tiger is hit by the first claw. Tiger will always get hit twice
    private int damage = 1;
    private bool hitThrown = false;

    public GameObject firstClawSlash;
    public GameObject secondClawSlash;
    public GameObject attackRange;
    public ParticleSystem attackEffect;
    public bool isOnGround = false;
    private float distance;
    
    private bool stunned = false; //Freeze Monkey when i don't want it to move and when the Monkey is being stunlocked by att

    private GameManager gameManager;
    public int HP = 5;
    private bool testingStun = false;
    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animation>();
        enemyScript = GetComponent<Enemy>();

        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody>();
        playerScript = player.GetComponent<PlayerController>();

        tiger = playerScript.tiger;
        bird = playerScript.bird;
        tigerRB = tiger.GetComponent<Rigidbody>();
        birdRB = bird.GetComponent<Rigidbody>();
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

        cameraRef = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        HPBar.transform.position = new Vector3(transform.position.x, transform.position.y + 1.9f, transform.position.z + 0.1f);
        //Monkey will only do it's chase while it's on the ground to avoid antigravity business
        if (attack == false && cooldown == false && stunned == false && isOnGround == true && testingStun == false)
        {
            animation.Play("Run");

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
            if (distance <= 6)
            {
                StartCoroutine(FirstClaw());
            }
        }
            HPBar.transform.LookAt(HPBar.transform.position - (cameraRef.transform.position - HPBar.transform.position));
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
        enemyScript.SetAttackEffect(attackEffect); //Doing this for practice for when I have enemies with multiple attacks
        //Necessary because there's enough time for the Monkey to repeat an attack on the bird
        //May not be necessary after my edit to the collider
        enemyScript.SetAttackDirection(followDirection);
        enemyScript.SetForce(6);
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
                isOnGround = false;
            }
            //If that doesn't work, put an if (dodge == false

            animation.Play("Attack");
        //}
        yield return new WaitForSeconds(1.5f);
        attack = false;
        firstClawSlash.SetActive(false);
        attackRange.SetActive(false);
        //For simplicity, second claw attack will only happen if player was hit by the first
        if (playerScript.tigerActive == true && enemyScript.hitLanded == true)
        {
            //hitCount = 1;
            StartCoroutine(SecondClaw());
            enemyScript.SetComboAttack();
        }
        else
        {
            StartCoroutine(StartCoolDown());
            //playerStunned = false; //Because a second atack will not be made on the bird
        }
        enemyScript.ResetHitLanded();
        Debug.Log("First Hit");
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
        followDirection = (tiger.transform.position - transform.position).normalized;
        enemyScript.SetAttackEffect(attackEffect);
        enemyScript.SetAttackDirection(followDirection);
        enemyScript.SetForce(12);
        monkeyRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);
        monkeyRb.AddForce(Vector3.up * 5, ForceMode.Impulse); //For jumping, may need to modify gravity
        animation.Play("Attack");
        secondClawSlash.SetActive(true);
        
        enemyScript.SetComboFinisher();

        yield return new WaitForSeconds(1f);
        StartCoroutine(StartCoolDown());
        attack = false;
        secondClawSlash.SetActive(false);
        attackRange.SetActive(false);
        //Debug.Log("Start Cool");
        enemyScript.ResetHitLanded();
        enemyScript.ResetHitNumber();
        enemyScript.SetComboAttack();
        enemyScript.SetComboFinisher();

        //hitLanded = false;
        Debug.Log("Second Hit");
        //Debug.Log("Combo Finisher is " + enemyScript.comboFinisher);
    }

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
    IEnumerator StartCoolDown()
    {
        cooldown = true;
        animation.Play("Idle");
        //For the timebeing, turn off the player's monkey range
        //playerScript.monkeyRange.SetActive(false);
        if (playerScript.tigerActive == true)
        {
            yield return new WaitForSeconds(3);
        }
        else if (playerScript.birdActive == true)
        {
            //monkeyRb.AddForce(Vector3.down * 2, ForceMode.Impulse);
            yield return new WaitForSeconds(7);
        }
        cooldown = false;
        //playerScript.monkeyRange.SetActive(true);
        Debug.Log("Cooldown finished");
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

    }
    public void TakeDamage()
    {
        if (playerScript.tigerActive == true)
        {
            
            //HPBar.transform.localScale = new Vector3(HPBar.transform.localScale.x - 0.008f, 0, 0);
            if (playerScript.specialInvincibility == false)
            {
                HP -= 2;
            }
            else if (playerScript.specialInvincibility == true)
            {
                HP -= 2;
            }
        }
        else if (playerScript.birdActive == true)
        {
            HP -= 1;
            //HPBar.transform.localScale = new Vector3(HPBar.transform.localScale.x - 0.004f, 0, 0);
        }
        if (HP <= 0)
        {
            Destroy(gameObject);
            //Need to put an if case because there are chances I can kill a foe that isn't locked on
            //I think I will put a locked on boolean on foes
            playerScript.LockOff();
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
        yield return new WaitForSeconds(1.5f);
        stunned = false;
    }
}
