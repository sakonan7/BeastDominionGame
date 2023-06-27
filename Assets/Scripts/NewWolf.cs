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

    //private new Animator animation;
    //public GameObject focalPoint;
    public GameObject attackAura;
    public GameObject attackRange;
    //public GameObject enemyTarget;
    private Rigidbody wolfRb;
    public Vector3 followDirection;
    private Vector3 attackRecoil; //direction
    private float walkSpeed = 18; //Changed from 24 because it's running to fast to the
    private int walkDirection = 0;
    private bool directionChosen = false;
    private float speed = 50;
    private float jumpForce = 1; //Originally 8
    private float attackForce = 12; //Originally 10 Changed from 15
    private float jumpAttackForce = 2;
    public int damage = 1;
    //I think I need another bool for battleStart idle time, but I think I just need to do if battleStart == true, use startIdleTime
    //and then use battleStart = false in every attack start. But writing this, I think I just need to do battleStart == false
    //in the same code as if (battleStart == true)
    private bool battleStart = true;
    private float startIdleTime;
    public float idleTime = 3;
    public int HP = 5;
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
        wolfAudio = GetComponent<AudioSource>();
        startIdleTime = Random.Range(2, 7);
    }

    // Update is called once per frame
    //Want Wolf to go back to chasing the player after getting stunned by an attack, but there are benefits to
    //The Wolf having to restart its pace, rather than attacking right af
    void Update()
    {
        //Originally had this in both the idle walk code and the chase code probably because they were methods instead of all
        //being in the update

        //Gonna rewrite the code. Now foes will put a sensor on the player
        //But I may be worried that other foes will react to the sensor
        //If worst comes, I will just use an interval of 1-2 seconds and just have the wolves do a jumping attack
        //-It's not a bad alternative because it shows dyanmicness
        //--I got, I will use a boolean attackActive that must be on for the monster to be able to att
        //attackActive will be based on attackRange.SetActive

        //I'm just going to try measuring distance instead
        if (playerScript.tigerActive == true)
        {
            playerPosition = tiger.transform.position;
            //attackRange.transform.position = tiger.transform.position;
            distance = Vector3.Distance(tiger.transform.position, transform.position);
        }
        else if (playerScript.birdActive == true)
        {
            playerPosition = bird.transform.position;
            //attackRange.transform.position = bird.transform.position;
        }

        if (cooldown == true)
        { 
            animation.Play("Wolf New Idle");
        }


        //Need this because I don't want to keep evoking ChooseDirect
        //I will try invoking ChooseDirection in Start() and after each cool

        //Got rid of isOnGround == true
        //I need both idle and directionChosen because basically, there are two idles, one where the Wolf stands still, and one
        //where the Wolf runs to the
        if (idle == true && directionChosen == true && chase == false && stunned == false && testingStun == false)
        {
            StartCoroutine(IdleWalk());
            animation.Play("Wolf Run");
            //animation.Play("Walk 1");
            //if (playerScript.tigerActive == true)
            //{
            //playerPosition = tiger.transform.position;
            //}
            //else if (playerScript.birdActive == true)
            //{
            //playerPosition = bird.transform.position;
            //}

            //For some reason, these parts are not playing after the first att
            if (walkDirection == 0)
            {
                //Debug.Log("Left Walk");
                wolfRb.AddForce(Vector3.left * walkSpeed);
            }
            else if (walkDirection == 1)
            {
                //Debug.Log("Right Walk");
                wolfRb.AddForce(Vector3.right * walkSpeed);
            }
            //More important for the wolf to be able to strafe left and right more
            //followDirection = (playerPosition - transform.position).normalized;
            //wolfRb.AddForce(followDirection * walkSpeed);
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
            animation.Play("Wolf Dash");
            followDirection = (playerPosition - transform.position).normalized;

            wolfRb.AddForce(followDirection * speed);
            //attackRange.SetActive(true);
            //attackRangeActive = true;
            
            //Debug.Log(distance);
            //I checked out the debug, wolf will always be at least 2.8 away from iger probably because of Tiger's size
            //Fucking finally
            if (chase == true && distance < 4f)
            {
                chase = false;
                launchAttack = true;
                Debug.Log("This should only play");
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
    }

    //IEnumerator is needed to keep the attack bool active because GroundAttack() will end it right away
    //All methods acts in the same moment, so a method for making attack false will active right a
    //I don't fully remember, so try a method and or maybe try an IEnumerator just for the attack bool
    //-I need an IEnumerator to start the cooldown

    IEnumerator PreAttackPause()
    {
        animation.Play("Wolf New Idle");
        yield return new WaitForSeconds(0.3f);
        //Debug.Log("Pause done");
        //animation.Stop();
        CorkScrew();
        StartCoroutine(AttackDuration());
    }

    IEnumerator AttackDuration()
    {


        attack = true;

        //I should not need the attack and chase boole
        //I want the wolf to stop dead in its tracks, try applying a small backwards force
        //-Looks like I already did months ago
        //Debug.Log("special Invincibility: " + playerScript.specialInvincibility); //Need to check why my invincibility frame isn't work
        //Invincibility didn't work because I used || instead of &&. I think I need to use && because both invincibilities have to be false
        //It worked. It makes so much sense, because the only way for the dodge to work with || is if both invincibilities are true
        //, but that could never happ
        //Oops, accidentally did playerScript.stunnedI
        //if (attackCounter == 0)
        //{
            
            
        //}
        //attackCounter = 1; //Putting it here because regardless, the wolf will not repeat an attack
        yield return new WaitForSeconds(0.8f);
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
        animation.Play("Wolf Corkscrew");
        wolfRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);
        attackRecoil = (transform.position - playerPosition).normalized;
        wolfRb.AddForce(attackRecoil, ForceMode.Impulse);
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
        Debug.Log(attackLanded);
    }

    //public void AirAttack()
    //{
    //attack = true;
    //animation.Play("Jump_Attack");
    //wolfRb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    //wolfRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);
    //Debug.Log("Jump Attack");
    //}

    IEnumerator StartCoolDown()
    {
        cooldown = true;
        //attack = false;
        Debug.Log("Cool down");
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
            //HP -= 2;
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
        animation.Play("Wolf Damage 2");
        //Debug.Log("Wolf Damage");
        yield return new WaitForSeconds(1.5f);

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
