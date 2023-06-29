using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class NewWolf : MonoBehaviour
{
    //Wolf code isn't working, so I'm making a new class where the Wolf walks differently and to test to see if Jump Attack will work now
    //Also, get rid of Chase method. I get the feeling it might be messing with the jump attack animation, even though logically, it should
    //work based on the tutorials

        /// <summary>
        /// This is challenging because I need so many booleans to decide what the enemy does
        /// </summary>
    private new Animation animation;
    private Animator animator;

    //private new Animator animation;
    //public GameObject focalPoint;
    public GameObject attackAura;
    public GameObject attackRange;
    //public GameObject enemyTarget;
    private Rigidbody wolfRb;
    public Vector3 followDirection;
    private Vector3 attackRecoil; //direction
    private float walkSpeed = 60; //Changed from 24 because it's running to fast to the
    private int walkDirection = 0;
    private int walkUpDownDirection = 0;
    private bool directionChosen = false;
    private float speed = 150;
    private float jumpForce = 5; //Originally 8
    private float attackForce = 12; //Originally 10 Changed from 15
    private float jumpAttackForce = 2;
    public int damage = 1;
    //I think I need another bool for battleStart idle time, but I think I just need to do if battleStart == true, use startIdleTime
    //and then use battleStart = false in every attack start. But writing this, I think I just need to do battleStart == false
    //in the same code as if (battleStart == true)
    private bool battleStart = true;
    private float startIdleTime;
    public float idleTime = 3;
    private Quaternion lookRotation;
    public int HP = 6;
    private bool idle = true;
    private bool chase = false;
    //private bool attack = false;
    private bool attack = false; //Keeping this because atm, I don't want foe to be interrupted when the player attacks them while
    //they're attack
    private bool launchAttack = false;
    public bool attackLanded = false;
    private int preAttackRecoil = 0;
    public int attackCounter = 0;
    //private bool attack = false; //Accidentally set this to true, causing Wolf to attack twice //But it still does for some method
    //attempts, so I don't know
    private bool cooldown = false;

    private bool stunned = false;

    private GameObject player;
    private PlayerController playerScript;
    private Vector3 playerPosition;
    private GameObject tiger;
    private GameObject bird;
    private Rigidbody tigerRB;
    private Rigidbody birdRB;

    public bool isOnGround = true;

    //public GameObject attackRange;
    //private bool attackRangeActive = false;

    private float distance;

    public ParticleSystem attackEffect;
    public ParticleSystem dyingEffect;
    private AudioSource wolfAudio;
    public AudioClip wolfAttack;

    //Gonna place the player's hit effects on the foes because for some reason, It gets destroyed on the Plyer's gameObject
    //Codes

    //Miscellaneous
    private GameManager gameManager;
    private bool testingStun = false;
    // Start is called before the first frame update
    void Start()
    {
        //idleAnim = focalPoint.GetComponent<WolfIdleAnim>();
        animation = GetComponent<Animation>();
        animator = GetComponent<Animator>();

        //animation = GetComponent<Animator>();
        wolfRb = GetComponent<Rigidbody>();

        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        tiger = playerScript.tiger;
        tigerRB = tiger.GetComponent<Rigidbody>();
        bird = playerScript.bird;
        birdRB = bird.GetComponent<Rigidbody>();

        Physics.gravity *= 0.5f;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (gameManager.difficulty == "Normal")
        {
            damage = 1; //For now, makeit 0 for testing rea
        }
        else if (gameManager.difficulty == "Hard")
        {
            damage = 2;
        }
        else if (gameManager.difficulty == "Very Hard")
        {
            damage = 3;
        }
        //dyingEffect.Play();
        ChooseDirection();
        ChooseUpDownDirection();
        wolfAudio = GetComponent<AudioSource>();
        startIdleTime = Random.Range(2, 7);
        //animator.speed = 1;
        animator.SetBool("Run", true);
    }

    // Update is called once per frame
    //Want Wolf to go back to chasing the player after getting stunned by an attack, but there are benefits to
    //The Wolf having to restart its pace, rather than attacking right af
    void Update()
    {

        //I'm just going to try measuring distance instead
        if (playerScript.tigerActive == true)
        {
            playerPosition = tiger.transform.position;
            //attackRange.transform.position = tiger.transform.position;
            distance = Vector3.Distance(tiger.transform.position, transform.position);
            lookRotation = Quaternion.LookRotation(tiger.transform.position - transform.position);
        }
        else if (playerScript.birdActive == true)
        {
            playerPosition = bird.transform.position;
            //attackRange.transform.position = bird.transform.position;
        }

        if (cooldown == true)
        {
            //animation.Play("Wolf New Idle");
            animator.SetBool("Idle", true);
        }


        //Need this because I don't want to keep evoking ChooseDirect
        //I will try invoking ChooseDirection in Start() and after each cool

        //Got rid of isOnGround == true
        //I need both idle and directionChosen because basically, there are two idles, one where the Wolf stands still, and one
        //where the Wolf runs to the
        if (idle == true && directionChosen == true && chase == false && stunned == false && testingStun == false)
        {
            StartCoroutine(IdleWalk());
            
            if (walkDirection == 0)
            {
                wolfRb.AddForce(Vector3.left * walkSpeed);
            }
            else if (walkDirection == 1)
            {
                wolfRb.AddForce(Vector3.right * walkSpeed);
            }
            //I think walking diagonally is good because I think dogs walk diagonally, not side
            if (walkUpDownDirection == 0)
            {
                //Debug.Log("Left Walk");
                wolfRb.AddForce(Vector3.fwd * walkSpeed);
            }
            else if (walkUpDownDirection == 1)
            {
                //Debug.Log("Right Walk");
                wolfRb.AddForce(Vector3.back * walkSpeed);
            }
        }
        //So actions are only performed on the ground
        //Got rid of //cooldown == false, then  && attack == false && isOnGround == true
        //I need to edit this, this is moving to close to the tiger. Also, for some reason I can't use distance in the if paramet
        if (chase == true)
        {
            //Run();
            //animation.Play("Jump_Attack");
            //StartCoroutine(GroundAttack());
            //animation.Play("Run Wolf");
            //May want to get rid of this code because it may not be necessary
            //But it may be necessary if I transform during a chase
            //animation.Play("Wolf Dash");
            //animator.speed = 2;
            animator.SetBool("Dash", true);
            followDirection = (playerPosition - transform.position).normalized;

            wolfRb.AddForce(followDirection * speed);
            //attackRange.SetActive(true);
            //attackRangeActive = true;

            //Debug.Log(distance);
            //I checked out the debug, wolf will always be at least 2.8 away from iger probably because of Tiger's size
            //Fucking finally
            //I'm going to get rid of chase because it doesn't make sense and this is already in a chase
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3); //Turned from 5 to 3 for smooth
            if (distance < 4f)
            {
                chase = false;
                animator.SetBool("Dash", false);
                launchAttack = true;
                //Debug.Log("This should only play");
                //I'm thinking of removing recoils because the attacks will be done by triggers, not collisions
                //I need this to stop the wolf's movement, because otherwise, the momentum of its addforce
                //will keep pushing the Wolf even if chase is over
                //Need to make the backward force not too noticeable, that's all
                //-Ugh, now it's not really stopping the wolf, corkscrew animation isn't working and wolf keeps moving
                //After the attack, so now it looks like the recoil doesn't work for the actual attack
                //The repeated recoils might have been a good thing
                //Debug.Log("Recoil");
                //if (preAttackRecoil == 0)
                //{
                    //attackRecoil = (transform.position - playerPosition).normalized;
                    //wolfRb.AddForce(attackRecoil * 12, ForceMode.Impulse); //Originally * 10 Changed from 15
                    //preAttackRecoil = 1;
                //}

            }
        }
        //chase == false && distance <= 6f
        //Need to insure this definitely plays after the above
        //Removed attackCounter. I think I need preAttackRecoil
        //Let's See what happen when I change else if to if
        //if (attack == true && preAttackRecoil == 1)
        //A lot more variables have to be used because this is an AI and AIs don't use command prompts
        //I don't need the attack bool because the damage calculation is done in tiger in
        if (launchAttack == true) 
        {
            //Maybe need a countdown before the attack
            //animation.Play("Wolf Corkscrew");
            //TempGroundAttack();
            //chase = false;


            launchAttack = false;
            StartCoroutine(PreAttackPause()); //PreAttackPause keeps playing, that's why the attack repeats, corkscrew is in there
            //PreAttackPause is not supposed to repeat. It's repeating because attack continues to equal
            //The weird thing is that this keeps repeating even though I turn off this boolean right a
        }
        //if (cooldown == true)
        //{
            //animation.Play("Idle Wolf");
        //}
        //attackAura.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }
    public void ChooseDirection()
    {
        //0 == left walk, 1 == right walk
        walkDirection = Random.Range(0, 2);
        directionChosen = true;
        //Debug.Log(walkDirection);
        //Debug.Log("Direction Chosen");
    }
    public void ChooseUpDownDirection()
    {
        //0 == left walk, 1 == right walk
        walkUpDownDirection = Random.Range(0, 2);
        //directionChosen = true;
        //Debug.Log(walkDirection);
        //Debug.Log("Direction Chosen");
    }
    IEnumerator IdleWalk()
    {
        if (battleStart == true)
        {
            idleTime = startIdleTime;
            battleStart = false;
        }
        idle = true;
        yield return new WaitForSeconds(idleTime);
        idle = false;
        chase = true;
        //Debug.Log("Idle Over, To See If This Plays Multiple");
        animator.SetBool("Run", false);
    }

    //IEnumerator is needed to keep the attack bool active because GroundAttack() will end it right away
    //All methods acts in the same moment, so a method for making attack false will active right a
    //I don't fully remember, so try a method and or maybe try an IEnumerator just for the attack bool
    //-I need an IEnumerator to start the cooldown

    IEnumerator PreAttackPause()
    {
        //animation.Play("Wolf New Idle");
        yield return new WaitForSeconds(0.3f);
        //Debug.Log("Pause done");
        //animation.Stop();
        CorkScrew();
        StartCoroutine(AttackDuration());
    }

    IEnumerator AttackDuration()
    {


        attack = true;

        //attackCounter = 1; //Putting it here because regardless, the wolf will not repeat an attack
        yield return new WaitForSeconds(0.4f);
        animator.SetBool("Ground Attack", false);
        attack = false;
        attackAura.SetActive(false);
        attackRange.SetActive(false);
        attackLanded = false;
        Debug.Log("Attack over");
        StartCoroutine(StartCoolDown());
        
    }
    //I think I will have the attack method AND the AttackDuration
    //That way, I don't need to use AttackCounter
    public void CorkScrew()
    {
        //animation.Play("Wolf Corkscrew");
        animator.SetBool("Ground Attack", true);
        //animator.speed = 3;
        wolfRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);
        attackRecoil = (transform.position - playerPosition).normalized;
        wolfRb.AddForce(attackRecoil, ForceMode.Impulse);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5); //In case the the player runs around the Wolf
        //right before the att
        Debug.Log("Corkscrew");
        //I don't think this is going to make much of a difference, but attack aura keeps spazzing 
        attackAura.SetActive(true);
        attackRange.SetActive(true);
    }
    public void PlayAttackEffect()
    {
        attackEffect.Play();
        wolfAudio.PlayOneShot(wolfAttack, 0.1f);
    }
    //To ensure that two hits don't come from the same attack
    public void SetAttackLanded()
    {
        attackLanded = true;
        Debug.Log("Attack Landed" + attackLanded);
    }


    IEnumerator StartCoolDown()
    {
        cooldown = true;
        //attack = false;
        //Debug.Log("Cool down");
        //animator.speed = 0;
        if (playerScript.tigerActive == true)
        {
            yield return new WaitForSeconds(2);
        }
        else if (playerScript.birdActive == true)
        {
            //wolfRb.AddForce(Vector3.down * 2, ForceMode.Impulse);
            yield return new WaitForSeconds(8);
        }
        cooldown = false;
        //idleAnim.ResetIdle();
        idle = true;
        //Forgot about this part
        directionChosen = false;
        attackCounter = 0;
        preAttackRecoil = 0;
        idleTime = 3;
        ChooseDirection();
        ChooseUpDownDirection();
        animator.SetBool("Idle", false);
        animator.SetBool("Run", true);
        //animator.speed = 1;
    }
    public void TakeDamage()
    {
        //ATM, I'm going to make it so that attacking Wolf while they're attacking doesn't stun Wolf
        if(attack == false)
        {
            Stunned();
        }
        
        if (playerScript.tigerActive == true && HP > 0)
        {
            HP -= 2;
            //HPBar.transform.localScale = new Vector3(HPBar.transform.localScale.x - 0.008f, 0, 0);
        }
        else if (playerScript.birdActive == true && HP > 0)
        {
            HP -= 1;
            //HPBar.transform.localScale = new Vector3(HPBar.transform.localScale.x - 0.004f, 0, 0);
        }
        //Debug.Log(HP);
        //Was originally an else if case, but moved it into the above because the wolf doesn't die the minute its HP falls under 0
        //Was originally going to place this under each conditional, but it looks like this conditional works on its own
        if (HP <= 0)
        {
            dyingEffect.Play();
            Destroy(gameObject);
            playerScript.LockOff();
            Debug.Log("Wolf Dies");
            gameManager.EnemyDefeated();
        }
    }
    public void Stunned()
    {
        StartCoroutine(StunnedDuration());
    }
    IEnumerator StunnedDuration()
    {
        stunned = true;
        yield return new WaitForSeconds(1.5f);
        
        ///Basically need to cancel everything and go back to Idle Walk after an attack from the play
        stunned = false;
        //attack = false;
        //launchAttack = false;
        chase = false;
        cooldown = false;
        //idleAnim.ResetIdle();
        idle = true;
        //Forgot about this part
        directionChosen = false; //Have to do this because Wolf isn't getting out of damage 2. Doing this for now because 
        //I need to reload the Wolf so it can try to attack the Player again after a while. Temporary
        ///ATM I will make it false, because it's false in CoolDown
        attackCounter = 0;
        preAttackRecoil = 0;
        idleTime = 2;
        ChooseDirection();
        ChooseUpDownDirection();
        animator.SetBool("Run", true);
        //Debug.Log("Let's See If There's A Walk");
        //animation.Play("Wolf New Idle");
    }
    //public Vector3 GiveTargetPosition()
    //{
        //return enemyTarget.transform.position;
    //}
    public void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Player") && attack == true && playerScript.dodge == false)
        //{

        //}
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }
    public void OnTriggerEnter(Collider other)
    {

        //Going to do this because I need to trigger the take damage methods of each foe
        if (other.CompareTag("Tiger Attack Regular"))
        {
            //For now, just trigger stun. I will use both of their directions to perform the knockback
            TakeDamage();
            playerScript.PlayTigerRegularStrike(transform.position);
            //Vector3 knockbackDirection = (transform.position - tiger.transform.position).normalized;
            //knockback force is inconsistent. Sometimes it doesn't knockback at all. Sometimes it knocks back too much
            //It doesn't matter what the value is.
            //It may not matter because I will have the attack lag minimized
            //But I don't want the player to whiff attacks, so I think I will make sure the tiger is the right distance from the wolf
            //Unless I can make a force play until a certain distance is reached
            //I can't use forcemode.impulse then
            wolfRb.AddForce(playerScript.attackDirection * 12, ForceMode.Impulse);
            playerScript.AttackLandedTrue();
        }
    }
}
