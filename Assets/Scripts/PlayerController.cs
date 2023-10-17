using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    //public Transform cam;
    public GameObject cameraRef;
    private ThirdPersonCamera camScript;
    public GameObject orientationObject;
    public Transform orientation;


    private new Animation animation;
    private Animator tigerAnimator;
    private new Animation birdAnimation;
    public bool tigerActive = true;
    public bool birdActive = false;

    public GameObject tiger;
    public GameObject bird;
    public GameObject birdSeparater;
    private Collider birdCollider;
    public GameObject tigerFollow;
    public GameObject birdFollow;
    public GameObject camFollow;
    public GameObject camRotater;
    private Vector3 followPosition;
    private Quaternion followRotation;
    public bool isFlying = false;
    private bool swoopedDown = false;
    [Header("Ground")]
    public float playerHeight = 0;
    public LayerMask whatIsGround;
    private bool grounded = true;

    [Header("Combo Meter")]
    public int hitNumber = 0;
    private bool rackingUpCombo = false;
    private static bool tigerSpecialUnlocked = false;
    private static bool birdSpecialUnlocked = false;
    private bool specialUsed = false;

    public bool specialInvincibility = false;
    [Header("Special Attack Objects")]
    public GameObject bladeOfLight;
    public GameObject tigerSpecialAOE;
    private bool charging = false;
    private Light staffLight;
    public GameObject birdAura;

    [Header("Place tiger and bird sound and effects here")]
    public AudioClip tigerRegularStrike;
    public ParticleSystem regularHitEffect;
    public ParticleSystem birdHitEffect;
    public GameObject tigerAttackEffect;
    public GameObject birdAttackEffect;
    public Transform transformEffect;
    public Light blackoutLight;
    private Color originalColor;
    public AudioClip tigerSpecial;
    public AudioClip tigerSpecialStrike;
    public ParticleSystem specialHitEffect;

    [Header("Healing")]
    public ParticleSystem pickUpHealEffect;
    public ParticleSystem healingEffect;

    //Either make the Rigidbody bigger or make a sensor
    //*Rigidybody might not be better because the tiger will spin. I can stop the sensor from rotating, but not the
    //Rigidbody. Rigidbody would just give a more realistic anima

    private float speed = 36; //Changed from 30 because now I'm using charactercontroller
    private float tigerSpeed = 36;
    private float birdSpeed = 54;
    private float dodgeForce = 25; //90 barely jumps
    public GameObject dodgeEffect;
    private float attackForce = 20;
    private float forwardInput;
    private Vector3 direction;
    private float sideInput;
    private Vector3 moveDirection;
    //This stuff is from the 3rd Person Camera Vide
    private float targetAngle;
    private float angle;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private Vector3 moveDir;
    private bool closeTheDistance = false;
    private bool specialCloseTheDistance = false;
    private float attackTimeLength;
    private float normalTigerAttacklength = 0.5f;
    private float distanceCloserTigerAttackLength = 0.5f;
    private float birdAttackLength = 0.2f;
    private Quaternion attackRotation;

    //Testing Out Tiger's Rotating Towards Monkey
    //Having LockOn( On should be making Tiger's rotation always towards Monkey though..
    
    private float angleBetween;
    //private float distance;

    private Rigidbody playerRb;
    //private Rigidbody tigerRB;
    private bool running = false;

    private Rigidbody birdRB;
    private Vector3 resetY;

    //These bools are necessary because they are important outside of making the player unable to 
    public bool dodge = false;
    private float dodgeTime;
    private float tigerDodge = 0.8f;
    private float birdDodge = 1.2f;
    //public bool successfulDodge = true;
    //public bool lag = false;
    public bool attack = false;
    public bool swoopLag = false;
    public bool attackLanded = false;
    private bool canCombo = false;
    //I can either make the distance closers stop movement or use stuned/damaged to do the 
    public bool cantMove = false;

    private bool transforming = false;
    public bool cantTransform = false;
    public bool stunnedInvincibility;
    private float invincibleFrame;
    private bool noMoreTurn = false;
    private Quaternion originalRotation;

    public GameObject target;
    private TargetTracking targetScript;
    private GameObject targetedEnemy;
    private Vector3 currentEnemyPosition;
    private Enemy enemyScript;
    private Enemy guardingEnemy;
    private Enemy attackingEnemy;
    private Vector3 enemyTargetPosition;
    private Rigidbody foeRB;
    //public bool canLockOn = false;
    public bool lockedOn = false;
    public GameObject focalPoint; //Turn camera towards targetted foe
    //New lock on code
    private float distance = 0;
    private bool lockOnTurnedOn = false;
    private static bool noHealingItems = true;

    public Vector3 attackDirection;
    Vector3 dodgeDirection;
    private Transform lockedOnLocation;
    public Vector3 transformPosition;

    //Taking damage
    private bool tigerFlinch = false;
    private bool tigerFlinch2 = false;
    private bool tigerKnockedBack = false;
    private bool birdFlinch = false;
    private bool birdKnockedBack = false;
    public bool invincible = false;
    private bool damageStun = false;

    //Sound
    private AudioSource playerAudio;
    public AudioClip tigerRoar;
    public AudioClip damaged;
    public AudioClip knockBack;
    public AudioClip birdCry;
    public AudioClip bladeOfLightChargeUp;
    public AudioClip tigerSwing;

    //Display
    [Header ("Display")]
    public TextMeshProUGUI HPText;
    //public Image playerMugshot;
    public static int HP = 10;
    private int originalHP = 10;
    public static bool death = false;
    //public bool gameOver = false;
    public RawImage playerMugshot;
    public Texture tigerMugshot;
    public Texture birdMugshot;
    public Image HPBar;
    private float maxHPBarFill = 1; //References the max amount of physical space the HP fills
    public TextMesh damageDisplay;
    private int damageForDisplay; //Proxy variable just for displaying damage taken
    public GameObject healingItemDisplay;
    public TextMeshProUGUI healingItemNumber;
    public static int numberOfItems = 0;
    //RectTransform newTargetRect;
    //public GameObject newTarget;
    public GameObject comboCounterHolder;
    public TextMeshProUGUI comboCounter;
    public RawImage tigerComboIcon;
    public RawImage tigerSpecialCommand;
    public RawImage tigerSpecialLightUp;
    public RawImage tigerComboLightUp;
    public RawImage birdComboIcon;
    public RawImage birdSpecialCommand;
    public RawImage birdSpecialLightUp;
    public RawImage birdComboLightUp;
    public TextMeshProUGUI pauseMessage;
    public TextMeshProUGUI gameOverMessage;

    //Miscellaneous
    private GameManager gameManagerScript;

    //Boundaries

    //Pause Game
    private bool paused = false;

    //Practice for Lightning King Boss
    //public GameObject ball;
    //private Rigidbody ballRB;

    public GameObject MotionBlurObject;
    private bool attackEffect = false;
    private void Awake()
    {
        SetHP();
    }
    void Start()
    {
        //cam = cameraRef.transform;
        camScript = cameraRef.GetComponent<ThirdPersonCamera>();
        Cutscenes();
        playerRb = GetComponent<Rigidbody>();

        //HPBar = Image.Find("Player HP Bar Background");
        
        animation = tiger.GetComponent<Animation>();
        tigerAnimator = tiger.GetComponent<Animator>();
        

        birdRB = bird.GetComponent<Rigidbody>();
        birdAnimation = bird.GetComponent<Animation>();
        //birdSensor = GameObject.Find("Bird Sensor");
        

        tigerActive = true;
        tiger.SetActive(true);
        //tigerSensor.SetActive(true);
        birdActive = false;
        bird.SetActive(false);

        dodgeTime = tigerDodge;

        playerAudio = GetComponent<AudioSource>();
        //playerUI = GameObject.Find("Canvas");
        //DisplayHP(HP);
        //maxHPBarFill = 1;//Changed this from HPBar.FillAmount because that is always going to equal 1

        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //ballRB = ball.GetComponent<Rigidbody>();
        staffLight = GameObject.Find("NewStaff").GetComponent<Light>();
        damageDisplay.color = new Color(1, 1, 1, 1);
        comboCounter.text = "x " + hitNumber;
        healingItemNumber.text = "X " + numberOfItems;
        originalColor = blackoutLight.color;
        SpecialOn();

        targetScript = target.GetComponent<TargetTracking>();
        target.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && paused == false)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.F) && paused == true)
        {
            UnpauseGame();
        }
        if(tigerActive == true)
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        }
        else if (birdActive == true)
        {
            grounded = false;
        }
    }
    //I tried putting everything in regular Update(), but it makes everything a lot slower
    void FixedUpdate()
    {

        //Code that applies only when the respective form is active
        //This code is for anything that needs to follow the player
        //transform.rotation = new Quaternion(0, transform.rotation.y, transform.rotation.z, 0);
        if (birdActive == true)
        {
            //monkeyRange.transform.position = new Vector3(bird.transform.position.x, 0.45f, bird.transform.position.z - 0.1f);
            //wolfRange.transform.position = new Vector3(bird.transform.position.x, 0.45f, bird.transform.position.z - 0.1f);
            //sensor.transform.position = new Vector3(bird.transform.position.x, 0.45f, bird.transform.position.z - 0.1f);
            //dodgeEffect.transform.position = new Vector3(bird.transform.position.x, 0.45f, bird.transform.position.z - 0.1f);
            tiger.transform.rotation = bird.transform.rotation;
            //transform.rotation = bird.transform.rotation;
        }
        else if (tigerActive == true)
        {
            //monkeyRange.transform.position = new Vector3(tiger.transform.position.x, 0.45f, tiger.transform.position.z - 0.1f);
            //wolfRange.transform.position = new Vector3(tiger.transform.position.x, 0.45f, tiger.transform.position.z - 0.1f);
            //sensor.transform.position = new Vector3(tiger.transform.position.x, 0.45f, tiger.transform.position.z - 0.1f);
            //dodgeEffect.transform.position = new Vector3(tiger.transform.position.x, 0.45f, tiger.transform.position.z - 0.1f);
            //orientationObject.transform.position = new Vector3(tiger.transform.position.x, tiger.transform.position.y + 0.77f, tiger.transform.position.z + 0.82f);
            //orientationObject.transform.rotation = tiger.transform.rotation;
            bird.transform.rotation = tiger.transform.rotation;
            //tiger.transform.position = new Vector3(tiger.transform.position.x,transform.position.y - 0.098f,tiger.transform.position.z);
            //transform.rotation = tiger.transform.rotation;
        }

        if (gameManagerScript.stageCleared == true)
        {
            birdSeparater.SetActive(false);
        }
        else if (gameManagerScript.stageCleared == false)
        {
            birdSeparater.SetActive(true);
        }

        if(death == true)
        {
            //ATM, each level always starts out with the player being a tiger. This will not be the case for 
            //gameOver = death;
            animation.Play("Current Death");
            cantMove = true;
            //gameManagerScript.StartLevelMethod();
        }

        //movement
        forwardInput = Input.GetAxisRaw("Vertical");
        sideInput = Input.GetAxisRaw("Horizontal");
        //direction = new Vector3(forwardInput, 0, sideInput).normalized;



        //Wrap all the code in the instances where the Player can't do anything, like stunlocking and transforming
        //if (gameManagerScript.startGame == true)
        //{
        //Idle animat
        ///Kept attack == false for closeTheDist
        if (dodge == false && attack == false && tigerActive == true && running == false && cantMove == false)
            {
                animation.Play("Idle Tweak");
            //tigerAnimator.SetBool("Idle", true);
            }
            if (dodge == false &&attack ==false&& birdActive == true && cantMove == false)
            {
                birdAnimation.Play("Player Idle");
            }

        //Movement
        ///Gonna try to make a general case for all controls where you can't do anything if you're lagged or stunned or using a special
        ///Or if you're using a dodge or if you're using an attack. Just put every case where the player shouldn't be mov
        ///This is so Players can't repeat an action while the action is happen
        if (cantMove == false && closeTheDistance == false && specialCloseTheDistance == false)
        {
            if (attack == false && dodge == false)
            {
                //Going to add another code where tiger can only move if grounded ==
                //Or make it so player can'tdo anything unlesstiger is grounded or bird is active 
                moveDirection = orientation.forward * forwardInput + orientation.right * sideInput;

                playerRb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
                playerRb.drag = 5;
                //speed control
                Vector3 flatVel = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);


                if (flatVel.magnitude > speed)
                {
                    Vector3 limitedVel = flatVel.normalized * speed;
                    playerRb.velocity = new Vector3(limitedVel.x, 0, limitedVel.z);
                }
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    running = true;
                }
                else
                {
                    running = false;
                }
            }

            //if (birdActive == true && attack == false && dodge == false)
            //{

                //birdRB.AddForce(Vector3.forward * speed * forwardInput);
                //birdRB.AddForce(Vector3.right * speed * sideInput);

            //}
            //Tiger has different code for movement because ineed to trigger the running animat
            //else if (tigerActive == true && attack == false && dodge == false && gameManagerScript.startingCutscene == false)
            //{


                //if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                //{
                    //running = true;
                //}
                //else
                //{
                    //running = false;
                //}
            //}




        }

        //These are for things outside of controls such as attack effects and things that will not be affected by anything that are related
        //to stuns such as locking on
        //For some reason, || makes more sense than && to me, but I think logically, it should be 
        if (running == true && (damageStun == false && stunnedInvincibility == false))
        {
            animation.Play("Run Tweak");
            //tigerAnimator.SetTrigger("Run");
        }
        //else
        //{
            //tigerAnimator.SetTrigger("Run");
        //}
        //Putting it on here atm because I want this to be interupted by stunning
        //Also, players can't do anything while the character is closing the distance
        //Put it out of the conditional because it's not an action
        ///Keeping this here even though I moved the code from this to the IEnumerator to play the animation
        ///


        if (closeTheDistance == true && cantMove == false)
        {
            //float time = 0;
            animation.Play("Distance Closer");
            //Need to keep updatingtheenemypositionandrota
            //This is important, because I only receive the enemy position once, and this doesn't take into account enemies moving. Especially not fast moving
            currentEnemyPosition = new Vector3(targetedEnemy.transform.position.x, 0, targetedEnemy.transform.position.z);
            attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z).normalized;
            attackRotation = Quaternion.LookRotation(currentEnemyPosition - tiger.transform.position);
            tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 5 * Time.deltaTime);

            //I would prefer to use nonImpulse, but it is too slow and using Impulse is unexpectedly cool
            playerRb.AddForce(attackDirection * 10, ForceMode.Impulse); //attack force wasn't enough //Also, it isn't enough here //Try impulse
                //ForceMode Impulse is amazing. Needed to go from speed to 5 becaue of how fast and far it went
                
            //playerRb.velocity = attackDirection * 10;
            //speed control
            Vector3 dashVel = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);


            if (dashVel.magnitude > speed)
            {
                Vector3 limitedDashVel = dashVel.normalized * speed;
                playerRb.velocity = new Vector3(limitedDashVel.x, 0, limitedDashVel.z);
            }
            //time += Time.deltaTime * turnSpeed;
            if (tigerActive == true)
            {
                tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 3);
            }
            if (birdActive == true)
            {
                bird.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 3);
            }
            
            //Just cancel the distanceClosersif the flying enemy just happens to start flying before the player can reach
            //them.Shouldn't happen becauseThedistance clloserattack is very quick
            if (enemyScript.isFlying == true)
            {
                closeTheDistance = false; // Almostforgot thispart
                //Just cancel into att
                if (tigerActive==true)
                {
                    animation.Play("Attack 1 & 2");
                    //playerRb.constraints = RigidbodyConstraints.FreezeRotation;
                    playerAudio.PlayOneShot(tigerSwing, 0.05f);
                }
            }

        }
        if (specialCloseTheDistance == true)
        {
            //For some reason I commented out the code for the distance closing lo
            if (tigerActive == true)
            {
                animation.Play("Distance Closer");
            }
            currentEnemyPosition = new Vector3(targetedEnemy.transform.position.x, 0, targetedEnemy.transform.position.z);
            attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z).normalized;
            attackRotation = Quaternion.LookRotation(currentEnemyPosition - tiger.transform.position);
            tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 5 * Time.deltaTime);

            //I would prefer to use nonImpulse, but it is too slow and using Impulse is unexpectedly cool
            ///For some reason, I made specialdistancecloserslower than regular distance clos
            playerRb.AddForce(attackDirection * 5, ForceMode.Impulse); //attack force wasn't enough //Also, it isn't enough here //Try impulse
                                                                       //ForceMode Impulse is amazing. Needed to go from speed to 5 becaue of how fast and far it went
            //transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 5 * Time.deltaTime);
            //Put the closeTheDistance code in here
            Vector3 specVel = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);


            if (specVel.magnitude > speed)
            {
                Vector3 limitedSpecVel = specVel.normalized * speed;
                playerRb.velocity = new Vector3(limitedSpecVel.x, 0, limitedSpecVel.z);
            }
            if (tigerActive == true)
            {
                tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 3);
            }
            if (birdActive == true)
            {
                bird.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 3);
            }

            if (enemyScript.isFlying == true)
            {
                specialCloseTheDistance = false;
                if (tigerActive == true)
                {
                    animation.Play("Attack 1 & 2");
                    TigerSpecial();
                    StartCoroutine(TigerSpecialDuration());
                }
                else if (birdActive == true)
                {
                    StartCoroutine(BirdSpecialDuration());
                }
            }
        }



            //Motion Blur
            //I'm thinking I don't want a motion blur for special attacks
        //Almost forgot to add tigerActive because I don't want the blur for the bird
        if (attack == true && tigerActive == true)
        {
            //attackEffect = true;
        }
        else
        {
            attackEffect = false;
        }
        if (attackEffect == true)
        {
            MotionBlurObject.SetActive(true);
        }
        else if (attackEffect == false)
        {
            MotionBlurObject.SetActive(false);
        }


        if (charging == true)
        {
            animation.Play("Charge Up");
        }

        //Non Input
        //temrporary lockdown code while loop in LockOn() froze the game because it was an Infinite loop. Better for update because Update
        //updates per frame
        if (lockedOn == true)
        {
            //Put this in a conditional and changed from enemyScript.HP > 0 so I don't get an error when an enemy is defeat
            if (targetedEnemy != null)
                {
                //Too complicated to place the target on a unique place on a foe atm because I would need to access the enemy's class.
                //Unless I use tags

                //Weirdly fixed by using position instead of Translate(
                //enemyTargetPosition = targetedEnemy.GiveTargetPosition();
                //target.transform.position = targetedEnemy.transform.position; //I think I should at most just adjust target's loca
                                                                              //target.transform.position = enemyTargetPosition;
                                                                              //newTarget.transform.position = new Vector3(targetedEnemy.transform.position.x, targetedEnemy.transform.position.y, 0);

                //Original plan was to keep the original target but inactive to give the player a target to hit
                //And to have the new target appear over the targeted foe
                //newTargetRect = newTarget.GetComponent<RectTransform>();
                //newTargetRect.localPosition = new Vector2(target.transform.position.x, target.transform.position.y);
                
                //Code to make lockedOn symbol face camera
                //The original simple LookAt(cameraRef.transform) didn't work because it showed the clear backside of the plane/quad instead
                //target.transform.LookAt(target.transform.position - (cameraRef.transform.position - target.transform.position));
                
                

                //Code to turn camera towards target
                //Test to see if using a method will only place the method once. I don't think so, so try it in the lockedon meth
                distance = Vector3.Distance(targetedEnemy.transform.position, transform.position);
                //Calculating attackdirection AND attackrotation here tosee if it solves the first attack prob
                followPosition = new Vector3(targetedEnemy.transform.position.x, 0, targetedEnemy.transform.position.z);
                followRotation = Quaternion.LookRotation(followPosition - transform.position);
                
                targetScript.Target(lockedOnLocation.position);

                camRotater.transform.rotation = Quaternion.Slerp(transform.rotation, followRotation, 3);

            }
            //I may want to change this because I can trigger an error by trying to access enemyScript when targetEnemy has been killed
            if (enemyScript.GetHP() <= 0)
            {
                LockOff();
                targetScript.TargetOff();
            }
        }
        //else if (lockedOn == false)
        //{
            //target.SetActive(false);
            //gameManagerScript.LockOff();
        //}
        if (transforming == true)
        {
            animation.Play("Transform");
            //If birdActive == true, have it tilt all the way up and tilt it back down after the IEnumerator is
        }
        //}
        

        //Cutscenes
        if (gameManagerScript.startingCutscene == true)
        {
            if (death ==false)
            {
                OpeningRun();
            }
        }
        //For some reason this only works here, in FixedUpdate, everywhereelse rerotates right a
        if (Input.GetKeyDown(KeyCode.N))
        {
            //transform.Rotate(0, transform.rotation.y + 180, 0, 0);
            playerRb.angularVelocity = new Vector3(0, 3.14f, 0);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            LoseHP(1, 1);
            StartCoroutine(DamageDisplayed());
        }
    }
    public void LateUpdate()
    {
        //Lock On
        //Changing code for now because lock on doesn't work on foes who have been generated into the scene
        //and not already
        //Removed canLockOn == true
        if (Input.GetMouseButtonDown(1))
        {
            //sensor.SetActive(true);
            LockOn();
        }
        if (Input.GetKeyDown(KeyCode.X) && lockedOn == true)
        {
            LockOff();
        }

        if (closeTheDistance == true && cantMove == true)
        {
            closeTheDistance = false;
            //Debug.Log("I don't want to do it, but, closeTheDistance = " + closeTheDistance);
        }

        //Setting this here because I want to make more space in Update and because this will happen after certainn actions
        //like getting damaged or performing a spec
        if (rackingUpCombo == false)
        {
            comboCounterHolder.gameObject.SetActive(false);
            hitNumber = 0;
            comboCounter.color = new Color(1, 1, 1, 1);
        }
        if (tigerSpecialUnlocked == true)
        {
            if (tigerActive == true)
            {
                tigerSpecialCommand.gameObject.SetActive(true);
            }
            else
            {
                tigerSpecialCommand.gameObject.SetActive(false);
            }
        }
        if (birdSpecialUnlocked == true)
        {
            if (birdActive == true)
            {

                birdSpecialCommand.gameObject.SetActive(true);
            }
            else
            {
                birdSpecialCommand.gameObject.SetActive(false);
            }
        }
        if (numberOfItems > 0)
        {
            healingItemDisplay.SetActive(true);
        }
        else if (numberOfItems <= 0)
        {
            healingItemDisplay.SetActive(false);
        }
        //Attacking
        ///Small note, I accidentally used & instead of && for lockedOn. It seems like it didn't affect the code
        ///Doing AttackDuration in attack methods instead
        ///            //Tiger isn't turning at all...This worked for rabbit completely fine with and without input from me
            //When I put this in Update(), the attacks are slower than if I were to put in LateUpdate()

        ///I feel like this doesn't work, because I can spam bird attack while it is still lowered. I added swoopedDown for this 
        if (cantMove == false)
        {
            //For Attacking, I may want to modify Quaternion.Slerp tomake the tigerand maybe the bird too not turn out of X-ax
            if (Input.GetMouseButtonDown(0) && lockedOn == true && swoopedDown == false)
            {

                //attackDirection isn't changing directions because it's using the empty Player object as the reference and the Player
                //object doesn't move
                if (birdActive == true)
                {
                    Swoop();
                    //Moved attackDirection here because the player object gets rotated now
                    attackDirection = (targetedEnemy.transform.position - tiger.transform.position).normalized;

                    //This only works for grounded ene
                    //attackRotation = Quaternion.LookRotation(targetedEnemy.transform.position - tiger.transform.position);

                    currentEnemyPosition = new Vector3(targetedEnemy.transform.position.x, tiger.transform.position.y, targetedEnemy.transform.position.z);
                    attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z).normalized;
                    attackRotation = Quaternion.LookRotation(currentEnemyPosition - tiger.transform.position);

                    bird.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 3); //Moved this from Strike() to
                                                                                                             //see if I can immediately turn my character towards an ene

                    //bird.transform.rotation = new Quaternion(0, bird.transform.rotation.y, bird.transform.rotation.z, 0);
                    //bird.transform.rotation = new Quaternion(0, attackRotation.y, attackRotation.z, 0);
                    birdHitEffect.Play();
                    if (enemyScript.isFlying==true)
                    {
                       // bird.transform.rotation = Quaternion.Slerp(bird.transform.rotation, attackRotation, 3); //Notsurewhatthepurpose of thisis
                    }
                }
                else if (tigerActive == true)
                {
                    //StartCoroutine(Turning());
                    //Strike();
                    //if (canCombo == false)
                    //{
                    //StartCoroutine(Turning());
                    //}
                    //else
                    //{
                    Strike();

                    attackDirection = (targetedEnemy.transform.position - tiger.transform.position).normalized;
                    //Only rotate when foe is notfly
                    //My method (not literal)worked here. It's because of attackRotation that tiger rotates upwards for att
                    //if (enemyScript.isFlying == false)
                    //{
                    //Moved attackDirection here because the player object gets rotated now
                        //attackDirection = (targetedEnemy.transform.position - tiger.transform.position).normalized;
                    //if (enemyScript.isFlying == true)
                    //{//To avoid tiger from roating from att
                        //attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z);
                    //}

                        //attackRotation = Quaternion.LookRotation(targetedEnemy.transform.position - tiger.transform.position);

                        currentEnemyPosition = new Vector3(targetedEnemy.transform.position.x, 0, targetedEnemy.transform.position.z);
                        attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z).normalized;
                        attackRotation = Quaternion.LookRotation(currentEnemyPosition - tiger.transform.position);


                        tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 3); //Moved this from Strike() to
                                                                                                              //see if I can immediately turn my character towards an ene

                    //}
                    //else if (enemyScript.isFlying == true)
                    //{

                    //}
                }
                if (running == true)
                {
                    running = false;
                }
            }
            if (Input.GetMouseButtonDown(0) && lockedOn == false &&swoopedDown ==false)
            {


                //Temporary fix atm because while the below is what I want, I can't fix the non moving attack at
                if (running == true)
                {
                    running = false;

                }
                //if (running == false)
                //{
                //attackDirection = Vector3.fwd;
                //Strike();
                //}
                if (birdActive == true)
                {
                    //attackDirection = (target.transform.position - bird.transform.position).normalized;
                    attackDirection = (birdFollow.transform.position - tiger.transform.position).normalized;
                    Swoop();
                    
                }
                else if (tigerActive == true)
                {
                    //attackDirection = (target.transform.position - tiger.transform.position).normalized;
                    attackDirection = (tigerFollow.transform.position - tiger.transform.position).normalized;
                    Strike();
                    //Vector3 tigerPosition = tiger.transform.position
                    //attackDirection = tigerPosition.forward;
                    
                }
            }
            if (Input.GetKeyDown(KeyCode.Q) && numberOfItems > 0)
            {
                Heal();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {

                //Make an else case for the current code for when the player isn't pressing a direction
                //Placed animations here instead so that the full animation plays
                if (running == true)
                {
                    running = false;

                    playerRb.AddForce(moveDirection.normalized * dodgeForce, ForceMode.Impulse);
                }
                //else if (running == false)
                //{
                //playerRb.AddForce(Vector3.fwd * dodgeForce, ForceMode.Impulse);

                //}
                if (tigerActive == true)
                {
                    animation.Play("Jump Tweak");
                    dodgeDirection = (tigerFollow.transform.position - tiger.transform.position).normalized;

                }
                else if (birdActive == true)
                {
                    //birdAnimation.Play("Player Attack");
                    birdAnimation.Stop();
                    dodgeDirection = (birdFollow.transform.position - tiger.transform.position).normalized;
                }

                playerRb.AddForce(dodgeDirection * dodgeForce, ForceMode.Impulse);
                StartCoroutine(Dodge());
            }
            if (Input.GetKeyDown(KeyCode.E) && cantTransform == false)
            {
                //Transform();
                //Debug.Log("Transform");
                StartCoroutine(TransformCountdown());
                if (running == true)
                {
                    running = false;
                }
            }
            //Special Attack
            //Changed it so that ChargeUp will determine what direction Tiger will go
            // 
            if (Input.GetKeyDown(KeyCode.Z) && tigerSpecialUnlocked == true && tigerActive == true)
            {
                //StartCoroutine(TigerSpecialDuration());
                StartCoroutine(ChargeUp());

                //TigerSpecial();
                //tigerRB.AddTorque(Vector3.up * 100, ForceMode.VelocityChange);
                //Either use Fight Idle animation
                if (running == true)
                {
                    running = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.Z) && birdSpecialUnlocked == true && birdActive == true)
            {



                //Moved this from Strike() to
                //see if I can immediately turn my character towards an ene


                //attackDirection isn't changing directions because it's using the empty Player object as the reference and the Player
                //object doesn't move
                BirdSpecial();
                if (running == true)
                {
                    running = false;
                }
            }
        }

    }

    public void SetHP()
    {
        if (HP < 10 && HP > 0)
        {
            //HPBar.fillAmount = maxHPBarFill * (HP / originalHP);
            HPBar.fillAmount = (maxHPBarFill / originalHP) * HP;
        }
        else if (HP <= 0)
        {
            HPBar.fillAmount = maxHPBarFill;
            HP = originalHP;
            death = false;
        }
    }
    IEnumerator TellDistance ()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Distance between player and enemy is " + distance);
    }

    //Turn into an IEnumerator
    IEnumerator FreezeRotations()
    {
        yield return new WaitForSeconds(0.5f);
        playerRb.constraints = RigidbodyConstraints.FreezeRotationY;
    }
    public void UnfreezeRotations()
    {
        //playerRb.constraints = RigidbodyConstraints.None;
        //playerRb.constraints = RigidbodyConstraints.FreezePositionY;
        //playerRb.constraints = RigidbodyConstraints.FreezeRotationX;
        //playerRb.constraints = RigidbodyConstraints.FreezeRotationZ;
        playerRb.constraints &= RigidbodyConstraints.FreezeRotationY;
        playerRb.constraints = RigidbodyConstraints.FreezeRotationZ;
        playerRb.constraints = RigidbodyConstraints.FreezeRotationX;
    }
    IEnumerator AttackDuration()
    {
        attack = true;
        cantMove = true;
        if (tigerActive == true)
        {
            //tigerCollider.size = new Vector3(tigerCollider.size.x + 2.5f, tigerCollider.size.y, tigerCollider.size.z + 2.2f);
            //tigerCollider.center = new Vector3(tigerCollider.center.x, tigerCollider.center.y, tigerCollider.center.z + 1.25f);
            tigerAttackEffect.SetActive(true);
        }
        if (birdActive == true)
        {
            birdAttackEffect.SetActive(true);

        }
        if (damageStun == true || stunnedInvincibility) {
            attack = false;
            tigerAttackEffect.SetActive(false);
            birdAttackEffect.SetActive(false);
        }
        //playerRb.constraints = RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(attackTimeLength);
        attack = false;
        cantMove = false;
        //UnfreezeRotations();
        //Debug.Log("Attack");
        if (birdActive == true)
        {
                birdAttackEffect.SetActive(false);
            if (swoopedDown == true)
            {
                swoopedDown = false; bird.transform.Translate(0, 1.5f, 0);
                //bird.transform.Translate(0, -1.5f, 0);
                birdSeparater.transform.Translate(0, -1.5f, 0);
                //birdSeparater.SetActive(true);
                birdCollider.isTrigger = false;
            }

            isFlying = true;
            //transform.rotation = bird.transform.rotation;
        }
        if (tigerActive == true)
        {
            //tigerCollider.size = new Vector3(tigerCollider.size.x - 2.5f, tigerCollider.size.y, tigerCollider.size.z - 2.2f);
            //tigerCollider.center = new Vector3(tigerCollider.center.x, tigerCollider.center.y, tigerCollider.center.z - 1.25f);
            tigerAttackEffect.SetActive(false);
            //transform.rotation = tiger.transform.rotation;
            //tiger.transform.rotation = new Quaternion(originalRotation.x, 0, 0, 0);
        }
        //transform.rotation = new Quaternion(0, transform.rotation.y, transform.rotation.z, 0);
        if (attackLanded == false)
        {
            StartCoroutine(StrikeLag());
        }
        else
        {
            attackLanded = false;
            //Debug.Log("Combo Attack");
            StartCoroutine(ComboAttack());
        }
    }
    //This one is going to be tough, because I need attackLanded to be false eventually.
    //Test it only being true for 0.1 seconds
    //Also make a case where the method will only activate if attackLanded == false so it isn't triggered multiple times
    //Was originally going to place this in game manager, but I think this will be easier
    public void AttackLandedTrue()
    {
        if (attackLanded == false)
        {
            attackLanded = true;
            //Debug.Log("Can Continue To Attack");
            //StartCoroutine(NoAttackLag());
            playerRb.velocity = Vector3.zero;
        }
        if (hitNumber == 0 && (specialUsed == false))
        {
            StartCombo();
        }
        hitNumber++;
        comboCounter.text = "x " + hitNumber;
        
        if (hitNumber > 2 && hitNumber < 5)
        {
            //comboCounter.color = new Color(255, 191, 76, 255);
            //comboCounter.color = Color.yellow;
            comboCounter.color = new Color(1, 0.749f, 0.2962f, 1);
        }
        if (hitNumber >= 5)
        {
            //new Color(179, 255, 253, 255)
            //comboCounter.color = Color.cyan;
            comboCounter.color = new Color(0.5603f, 1, 0.965f, 1);
            if (tigerActive == true)
            {
                tigerSpecialUnlocked = true;
            }
            if (birdActive == true)
            {
                birdSpecialUnlocked = true;
            }
        }
        StartCoroutine(ComboMeterAnimation());
    }
    //Make it slightly increase in size and become yellow which each. When it reaches 3, make it
    //yellow orange, then a yellower orange for hits after 3, then light blue for 6 hits and the special attack pops
    //Then a tiger roar
    IEnumerator ComboMeterAnimation()
    {
        comboCounter.fontSize = 32;
        yield return new WaitForSeconds(0.5f);
        comboCounter.fontSize = 28.8f;
    }
    public void StartCombo()
    {
            rackingUpCombo = true;
            comboCounterHolder.gameObject.SetActive(true);
            hitNumber = 0; //Was initially 1, but I decided to put hitNumber++ in AttackLandedTrue(
    }
    IEnumerator ComboAttack()
    {
        canCombo = true;
        yield return new WaitForSeconds(3f); //1.3f instead of 0.1f, so lag can't take effect. Temporary?
        canCombo = false;
        //Debug.Log("May have Lost The Combo Opportunity"); //Can test for this by checking if canCombo is false after this IEnumerator finish
    }
    //IEnumerator for charging up Blade of light
    //Blade of light also increases into its full size
    IEnumerator ChargeUp()
    {
        charging = true;
        specialInvincibility = true;
        cantMove = true; //The placement of this stuff is necessary because specialCloseTheDistance is not affected by stuns, hun
        tigerSpecialLightUp.gameObject.SetActive(true);
        tigerComboLightUp.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        charging = false;
        
        bladeOfLight.SetActive(true);
        staffLight.intensity = 2;

        playerAudio.PlayOneShot(bladeOfLightChargeUp, 0.2f);
        //Maywant to watch out for this for flying enemies, but maybe not because the eaglewill be in its lag after the charge up
        //is done
        if (lockedOn == true && enemyScript.isFlying == false)
        {
            attackDirection = (targetedEnemy.transform.position - tiger.transform.position).normalized;
            //attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z);
            currentEnemyPosition = new Vector3(targetedEnemy.transform.position.x, 0, targetedEnemy.transform.position.z);
            attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z).normalized;
            attackRotation = Quaternion.LookRotation(currentEnemyPosition - tiger.transform.position);
            tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 5 * Time.deltaTime);
            //attackRotation = Quaternion.LookRotation(targetedEnemy.transform.position - tiger.transform.position);
            //I think the above is a mistake because I was supposed to calculate attackRotation before using
            //tiger.transform.rotation. But it also doesn't seem like a mistake because it didn't mess with the move's functionality at  the moment
            //I think it just messed up the tiger turning towards a target right after charging 
                specialCloseTheDistance = true;
            
        }
        else if (lockedOn == false)
        {
            attackDirection = (tigerFollow.transform.position - tiger.transform.position).normalized;
            playerRb.AddForce(attackDirection * (attackForce + 154), ForceMode.Impulse); //+ 8 normally, but try + 12 for blade of
            TigerSpecial();
            StartCoroutine(TigerSpecialDuration());
        }
        else if (lockedOn == true && enemyScript.isFlying == true)
        {
            //attackDirection = (tigerFollow.transform.position - tiger.transform.position).normalized;
            //Will attack in the flying enemy's direction, but not 
            currentEnemyPosition = new Vector3(targetedEnemy.transform.position.x, 0, targetedEnemy.transform.position.z);
            attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z).normalized;
            attackRotation = Quaternion.LookRotation(currentEnemyPosition - tiger.transform.position);
            playerRb.AddForce(attackDirection * (attackForce + 154), ForceMode.Impulse); //+ 8 normally, but try + 12 for blade of
            TigerSpecial();
            StartCoroutine(TigerSpecialDuration());
        }
    }

    IEnumerator TigerSpecialDuration()
    {
        //attack = true;
        //specialInvincibility = true;
        playerAudio.PlayOneShot(tigerSpecial, 0.1f);
        //bladeOfLight.SetActive(true);
        //tigerCollider.isTrigger = true;
        specialUsed = true;
        yield return new WaitForSeconds(2f);
        specialUsed = false;
        //tigerCollider.isTrigger = false;
        attack = false;
        cantMove = false;
        specialInvincibility = false;
        bladeOfLight.SetActive(false);
        tigerSpecialAOE.SetActive(false);
        StartCoroutine(StrikeLag());
        staffLight.intensity = 0;
        rackingUpCombo = false;
        tigerSpecialUnlocked = false;
        tigerSpecialCommand.gameObject.SetActive(false);
        tigerSpecialLightUp.gameObject.SetActive(false);
        tigerComboLightUp.gameObject.SetActive(false);
    }
    IEnumerator BirdSpecialDuration()
    {
        //attack = true;
        //specialInvincibility = true;
        //playerAudio.PlayOneShot(tigerSpecial, 0.1f);
        //bladeOfLight.SetActive(true);
        specialUsed = true;
        //isFlying = false;
        yield return new WaitForSeconds(3f);
        if (swoopedDown == true)
        {
                bird.transform.Translate(0, 1.5f, 0);
            birdSeparater.transform.Translate(0, -1.5f, 0);
            //birdSeparater.SetActive(true);
            swoopedDown = false; //Almost forgot
            birdCollider.isTrigger = false;
        }
        specialUsed = false;
        isFlying = true;
        attack = false;
        cantMove = false;
        specialInvincibility = false;
        birdAura.SetActive(false);
        StartCoroutine(StrikeLag());
        birdSpecialUnlocked = false;
        birdSpecialCommand.gameObject.SetActive(false);
        birdSpecialLightUp.gameObject.SetActive(false);
        birdComboLightUp.gameObject.SetActive(false);
        

        Debug.Log("Bird Special Ov");
    }
    public void Swoop()
    {
        //The intention is to have the bird forc and then return to its starting position when it started its att
        //attackDirection = (target.transform.position - transform.position).normalized;
        //Try using downward and upward forces instead
        //bird.transform.Translate(bird.transform.position.x, 0.8f, bird.transform.position.z);
        //birdRB.AddForce(Vector3.down * 5, ForceMode.Impulse);
        //birdRB.AddForce(attackDirection * attackForce, ForceMode.Impulse);
        //birdAnimation.Play("Attack");
        //resetY = new Vector3(bird.transform.position.x, 0.6f, bird.transform.position.z);
        //StartCoroutine(SwoopLag());
        //StartCoroutine(AttackDuration());

        canCombo = false;
        attackTimeLength = birdAttackLength;
        //birdAnimation.Play("Player Attack");
        birdAnimation.Stop();
        if (lockedOn == false)
        {
            
            StartCoroutine(AttackDuration());
            playerRb.AddForce(attackDirection * (attackForce + 10), ForceMode.Impulse);
            //StartCoroutine(FreezeRotations());
        }
        //I guestimated from gameplay that the distance needs to be at least 15
        //I think I may want to rewrite this because I don't think this works
        //if (lockedOn == true)
        //{
            //Debug.Log(distance);
        //}
        //Placingthis over here instead of in AttackDuration because IEnumerators are continuous (although I think I tried a movement code
        //In another script and it didn't work in an IEnumera)
        if (lockedOn == true && enemyScript.isFlying== false) //Almost accidentally wrotethis as ==true
        {
            //StartCoroutine(TellAngle());
            bird.transform.Translate(0, -1.5f, 0);
            birdSeparater.transform.Translate(0, 1.5f, 0);
            //birdSeparater.SetActive(false);
            isFlying = false;
            swoopedDown = true;
            birdCollider.isTrigger = true;
        }
        if (distance >= 15 && lockedOn)
        {
            Debug.Log("Non Distance Closer,Bird");
            StartCoroutine(AttackDuration());
            playerRb.AddForce(attackDirection * (attackForce + 20), ForceMode.Impulse);
            //StartCoroutine(FreezeRotations());
        }
        //Want DistanceCloser only to play when the tiger isn't close enough. Was originally going to have a distance > 10 || distance <=3
        //above,but I realized that the below will cover it. Maybe, let's keep testing it out
        else if (((distance < 15 && distance > 4) || canCombo == true) && lockedOn)
        {
            //I need some way to stop this
            //Maybe like the wolf, once the tiger reaches the necessary distance, just perform the regular attack
            //I could either have a non impulse movement to close the distance, or an impulse that will definitely get me close
            //enough to the target. I partially want to do the latter, but I think the former is better because the impulse is not consistent
            //Gonna need a method like DistanceCloser
            ///At first, I was wondering if the attack duration plays long enough for distance closer, but it looks like it does
            //Debug.Log("Distance Closer");
            //transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 10 * Time.deltaTime);
            closeTheDistance = true;
            //StartCoroutine(DistanceCloser());
            //tigerRB.AddForce(attackDirection * (attackForce + 16), ForceMode.Impulse);
            //canCombo = false;
        }
        else if (distance < 4 && lockedOn)
        {
            //transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 10 * Time.deltaTime);
            StartCoroutine(AttackDuration());
            playerRb.AddForce(attackDirection * (attackForce + 20), ForceMode.Impulse);//Changed from 8 to 12
            //StartCoroutine(FreezeRotations());
        }
    }
    //IEnumerator BirdTriggerOff()
    //{
        //yield return new WaitForSeconds();
    //}
    public void Strike()
    {
        //playerAudio.PlayOneShot(tigerRoar, 0.2f);
        //Why did I think of lockedOn == false, I think I thought of it in the case you aren't locked
        //I think I will create a case where lockedOn == false and for now, I want to make the distance smaller for
        //Debug.Log(playerRb.velocity);
        canCombo = false;
        //originalRotation = tiger.transform.rotation;

        //I envision the tiger not being able to reach the eagle at all and won't be able to use its distance clos
        if (lockedOn == false)
        {
            attackTimeLength = normalTigerAttacklength;
            StartCoroutine(AttackDuration());
            playerRb.AddForce(attackDirection * (attackForce + 10), ForceMode.Impulse);//Changed from 8 to 12
            //playerRb.velocity = attackDirection * (attackForce + 10);
            animation.Play("Attack 1 & 2");
            playerAudio.PlayOneShot(tigerSwing, 0.05f);
            //StartCoroutine(FreezeRotations());
        }
        //I guestimated from gameplay that the distance needs to be at least 15
        //I think I may want to rewrite this because I don't think this works
        if (lockedOn == true && enemyScript.isFlying == true)
        {
            attackTimeLength = normalTigerAttacklength;
            StartCoroutine(AttackDuration());
            
            playerRb.AddForce(attackDirection * (attackForce + 14), ForceMode.Impulse);//Changed from 8 to 12
            //playerRb.velocity = attackDirection * (attackForce + 10);
            animation.Play("Attack 1 & 2");
            playerAudio.PlayOneShot(tigerSwing, 0.05f);
        }
        else if (lockedOn == true && enemyScript.isFlying == false)
        {
            if (distance >= 15)
            {
                Debug.Log("Non distance closer, tig");
                //transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 10 * Time.deltaTime); //For some reason,
                //I don't have this for locked On att
                //transform.rotation = Quaternion.LookRotation(targetedEnemy.transform.position- transform.position);
                attackTimeLength = normalTigerAttacklength;
                StartCoroutine(AttackDuration());
                playerRb.AddForce(attackDirection * (attackForce + 14), ForceMode.Impulse);//Changed from 8 to 12
                                                                                           //playerRb.velocity = attackDirection * (attackForce + 14);
                animation.Play("Attack 1 & 2");
                playerAudio.PlayOneShot(tigerSwing, 0.05f);
                //StartCoroutine(FreezeRotations());
            }
            //Want DistanceCloser only to play when the tiger isn't close enough. Was originally going to have a distance > 10 || distance <=3
            //above,but I realized that the below will cover it. Maybe, let's keep testing it out
            else if (((distance < 15 && distance > 4) || canCombo == true))
            {
                //I need some way to stop this
                //Maybe like the wolf, once the tiger reaches the necessary distance, just perform the regular attack
                //I could either have a non impulse movement to close the distance, or an impulse that will definitely get me close
                //enough to the target. I partially want to do the latter, but I think the former is better because the impulse is not consistent
                //Gonna need a method like DistanceCloser
                ///At first, I was wondering if the attack duration plays long enough for distance closer, but it looks like it does
                //Debug.Log("Distance Closer");
                //transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 10 * Time.deltaTime);
                //transform.rotation = Quaternion.LookRotation(targetedEnemy.transform.position - transform.position);
                closeTheDistance = true;
                //StartCoroutine(DistanceCloser());
                //tigerRB.AddForce(attackDirection * (attackForce + 16), ForceMode.Impulse);
                //canCombo = false;
            }
            else if (distance < 4)
            {
                //transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 10 * Time.deltaTime);
                attackTimeLength = normalTigerAttacklength;
                StartCoroutine(AttackDuration());
                playerRb.AddForce(attackDirection * (attackForce + 14), ForceMode.Impulse);//Changed from 8 to 12
                animation.Play("Attack 1 & 2");
                playerAudio.PlayOneShot(tigerSwing, 0.05f);
                //StartCoroutine(FreezeRotations());
            }
        }

    }


    //The Vector3 strikeArea is for making the hit effect play directly on the
    public void PlayTigerRegularStrike(Vector3 strikeArea)
    {
        playerAudio.PlayOneShot(tigerRegularStrike, 0.5f);
        //regularHitEffect.transform.position = new Vector3(strikeArea.x, strikeArea.y + 1, strikeArea.z);
        //regularHitEffect.Play();
    }
    public void PlayBirdRegularStrike(Vector3 strikeArea)
    {
        playerAudio.PlayOneShot(tigerRegularStrike, 0.2f);
        //regularHitEffect.transform.position = new Vector3(strikeArea.x, strikeArea.y - 2, strikeArea.z);
        birdHitEffect.Play();
    }
    IEnumerator StrikeLag()
    {
        //Atm after all attacks. The player can't do anything, even walk or dodge
        cantMove = true;
        //Debug.Log("Attack Lag");
        yield return new WaitForSeconds(0.8f);
        cantMove = false;
    }
    public void SpecialOn()
    {
        tigerSpecialUnlocked = true;
        birdSpecialUnlocked = true;
    }
    public void TigerSpecial()
    {
        
        playerAudio.PlayOneShot(tigerSwing, 0.05f);

        if (lockedOn == true)
        {
            currentEnemyPosition = new Vector3(targetedEnemy.transform.position.x, 0, targetedEnemy.transform.position.z);
        }
        attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z).normalized;
        attackRotation = Quaternion.LookRotation(currentEnemyPosition - tiger.transform.position);

        //attackRotation = Quaternion.LookRotation(targetedEnemy.transform.position - tiger.transform.position);
        tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 10 * Time.deltaTime); //Am using all the attack rotations
        //here because there is a charge up before Tiger Special Attack
        //playerRb.AddRelativeTorque(Vector3.down * 5, ForceMode.Impulse);
        //transform.rotation = new Quaternion(0, 360, 0, 0);
        StartCoroutine(Spin());
        tigerSpecialAOE.SetActive(true);
        //TigerSpecialSecondStrike();
        //StartCoroutine(UseTigerSpecialSecond());
        
    }
    IEnumerator Spin()
    {
        //playerRb.AddRelativeTorque(Vector3.down * 10, ForceMode.Impulse);
        tiger.transform.rotation = new Quaternion(0, 180, 0, 0);
        //transform.Rotate(Vector3.down, 60 * Time.deltaTime);
        yield return new WaitForSeconds(2);
        //Debug.Log(Spin());
    }
    IEnumerator UseTigerSpecialSecond()
    {
        yield return new WaitForSeconds(1f);
        TigerSpecialSecondStrike();
    }
    //Test for combo attacks
    public void TigerSpecialSecondStrike()
    {
        playerAudio.PlayOneShot(tigerSwing, 0.05f);
        playerRb.AddRelativeTorque(Vector3.down * 5, ForceMode.Impulse);
        animation.Play("Attack 1 & 2");


    }
    public void PlayTigerSpecialStrike(Vector3 strikeArea)
    {
        playerAudio.PlayOneShot(tigerSpecialStrike, 0.5f);
        //specialHitEffect.transform.position = new Vector3(strikeArea.x, strikeArea.y + 1, strikeArea.z);
        //specialHitEffect.Play();
    }
    public void BirdSpecial()
    {
        birdAura.SetActive(true);
        //bird.transform.Translate(0, -1.5f, 0);
        //birdSeparater.SetActive(false);
        cantMove = true;
        rackingUpCombo = false;
        birdAnimation.Stop();
        if (lockedOn == false)
        {
            attackDirection = (birdFollow.transform.position - tiger.transform.position).normalized;
            StartCoroutine(BirdSpecialDuration());
            playerRb.AddForce(attackDirection * (attackForce + 25), ForceMode.Impulse);
            //StartCoroutine(FreezeRotations());
        }
        if (lockedOn == true && enemyScript.isFlying == false) //Almost accidentally wrotethis as ==true
        {
            //StartCoroutine(TellAngle());
            bird.transform.Translate(0, -1.5f, 0);
            birdSeparater.transform.Translate(0, 1.5f, 0);
            //birdSeparater.SetActive(false);
            isFlying = false;
            swoopedDown = true;
            //birdCollider.isTrigger = true;I didn't do this for nonlockon birdspecial for somereason
            currentEnemyPosition = new Vector3(targetedEnemy.transform.position.x, tiger.transform.position.y, targetedEnemy.transform.position.z);
            attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z).normalized;
            attackRotation = Quaternion.LookRotation(currentEnemyPosition - tiger.transform.position);
            bird.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 3);
            StartCoroutine(BirdSpecialDuration());
            playerRb.AddForce(attackDirection * (attackForce + 30), ForceMode.Impulse);
        }
        //I completely forgot a case for flyingene
        else if (lockedOn == true && enemyScript.isFlying == true) //Almost accidentally wrotethis as ==true
        {
            currentEnemyPosition = new Vector3(targetedEnemy.transform.position.x, tiger.transform.position.y, targetedEnemy.transform.position.z);
            attackDirection = new Vector3(attackDirection.x, 0, attackDirection.z).normalized;
            attackRotation = Quaternion.LookRotation(currentEnemyPosition - tiger.transform.position);
            bird.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 3);
            StartCoroutine(BirdSpecialDuration());
            playerRb.AddForce(attackDirection * (attackForce + 30), ForceMode.Impulse);
        }
    }
    IEnumerator Dodge()
    {
        dodge = true;
        cantMove = true;
        canCombo = false;
        //
        if (birdActive == true)
        {
            //bird.transform.rotation = new Quaternion(0, 0, 180, 0);
            dodgeEffect.SetActive(true);
        }
        
        yield return new WaitForSeconds(dodgeTime);
        if (birdActive == true)
        {
            //bird.transform.Rotate(0, bird.transform.rotation.y, 39.5f);
            //bird.transform.rotation = new Quaternion(0, 0, 300, 0);
            dodgeEffect.SetActive(false);
        }
        dodge = false;
        //dodgeEffect.SetActive(false);
        StartCoroutine(DodgeLag());
    }
    IEnumerator DodgeLag()
    {
        //Debug.Log("Dodge Lag");
        yield return new WaitForSeconds(0.2f);
        cantMove = false;
    }
    IEnumerator SwoopLag()
    {
        swoopLag = true;
        yield return new WaitForSeconds(0.2f);
        swoopLag = false;
    }
    //public void EnableLockOn()
    //{
        //canLockOn = true;
    //}
    public void LockOn()
    {
        //Create target
        //Make Target appear on foe
        //target.SetActive(true);
        //newTarget.SetActive(true);
        targetedEnemy = GameObject.FindGameObjectWithTag("Enemy");
        if (lockedOn == true)
        {
            //enemyScript.LockOff();
            //camScript.LockOff();
            LockOff();
            targetedEnemy = null;
            targetedEnemy = GameObject.FindGameObjectWithTag("Enemy");
            //gameManagerScript.LockOff();
        }
        

        if (targetedEnemy != null) //There's no immediate way to check if there's an object of tag something, I wished there
        {
            GameObject [] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float [] distanceList = new float[enemies.Length]; //Changed this from list to array
            float newMin = 0;
            //int newMinIndex = 0;
            bool smallestDistanceFound = false;
            int j = 0;
                for(int i = 0; i < enemies.Length; i++)
                {
                    distanceList[i] = Vector3.Distance(enemies[i].transform.position, transform.position);
                }
                newMin = Mathf.Min(distanceList);

                while (smallestDistanceFound == false)
                {
                    if (distanceList[j] == newMin)
                    {
                        smallestDistanceFound = true;
                        targetedEnemy = enemies[j];//This code will avoid problems of two foes having the same distance from
                                                   //the player
                                                   //Doing this in case there's already a foe that's been locked

                    enemyScript = targetedEnemy.GetComponent<Enemy>();
                    //if (enemyScript.lockedOn == false)
                    //{
                    target.SetActive(true);
                    enemyScript.LockOn();
                    //lockedOnLocation = new Vector3(targetedEnemy.transform.position.x, targetedEnemy.transform.position.y + 0.05f, targetedEnemy.transform.position.z);
                    lockedOnLocation = targetedEnemy.transform.Find("Camera Target").transform;
                    //gameManagerScript.LockOn(targetedEnemy.transform.position);
                    //}
                        
                    }
                    j++;
                }
            //if (targetedEnemy == null)
            //{
            //Debug.Log("Targeted Enemy is null");
            //}
            followPosition = new Vector3(targetedEnemy.transform.position.x, 0, targetedEnemy.transform.position.z);
            followRotation = Quaternion.LookRotation(followPosition - transform.position);


            camRotater.transform.rotation = Quaternion.Slerp(transform.rotation, followRotation, 3);
            camScript.TurnToTarget(camFollow.transform);
            StartCoroutine(TellDistance());
            
            targetScript.SetHP(enemyScript.originalHP, enemyScript.HP);
            lockedOn = true;
            //I was going to get rid of this because it looked like this code was for shifting the target
            //But it's actually if the lockOn function isn't even on
        }
        else
        {
            Debug.Log("Nothing to target");
        }
        //Rewrite from scratch, I think I accidentally killed my vibe

    }
    public void LockOff()
    {
        lockedOn = false;
        enemyScript.LockOff();
        camScript.LockOff();
        target.SetActive(false);
    }
    IEnumerator TransformCountdown()
    {
        transforming = true;
        cantMove = true;
        transformEffect.gameObject.SetActive(true);
        //GameObject dummyTransformObject = transformEffect.gameObject;
        //Instantiate(transformEffect, transform.position, Quaternion.identity);
        //transformEffect.gameObject.SetActive(true);
        blackoutLight.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(1f);
        Transform();
        transformEffect.gameObject.SetActive(false);
        //Destroy(dummyTransformObject);

        cantMove = false;
        blackoutLight.color = originalColor;
    }
    public void Transform()
    {
        //The problem was that for some reason E was triggering both if cases at the same time, so I made tiger transformation
        //into an else if
        //First is transforming into Tiger
        camScript.ChangeForms();
        if (birdActive == true)
        {
            //tiger.transform.Translate(bird.transform.position.x - 6.6f, 0, bird.transform.position.z + 14.5f);
            //tiger.transform.Translate(bird.transform.position.x - 14.18f, 0, bird.transform.position.z + 12.8f);
            tiger.SetActive(true);
            //transform.position = new Vector3(tiger.transform.position.x, transform.position.y, tiger.transform.position.z);

            //Instantiate(tiger);
            //tiger.transform.position = new Vector3(bird.transform.position.x, 3, bird.transform.position.z);

            //Destroy(bird);
            bird.SetActive(false);
            tigerActive = true;
            birdActive = false;
            //tigerSensor.SetActive(true);
            playerMugshot.texture = tigerMugshot;
            speed = tigerSpeed;
            dodgeTime = tigerDodge;
            //playerRb.constraints &= ~RigidbodyConstraints.FreezePositionY;
            //playerRb.constraints = RigidbodyConstraints.FreezeRotationX;
            //playerRb.constraints = RigidbodyConstraints.FreezeRotationZ;
            rackingUpCombo = false;
            tigerComboIcon.gameObject.SetActive(true);
            birdComboIcon.gameObject.SetActive(false);
            isFlying = false;
        }
        else if (tigerActive == true)
        {
            tiger.SetActive(false);
            //Instantiate(bird);
            //bird.transform.position = new Vector3(tiger.transform.position.x, 2.93f, tiger.transform.position.z);
            //bird.transform.Translate(tiger.transform.position.x - 6.6f, 0, tiger.transform.position.z + 14.5f);
            //bird.transform.Translate(tiger.transform.position.x - 14.8f, 0, tiger.transform.position.z + 12.8f);
            //Destroy(tiger);
            bird.SetActive(true);
            //transform.position = new Vector3(bird.transform.position.x, transform.position.y, bird.transform.position.z);
            tigerActive = false;
            birdActive = true;
            //birdSensor.SetActive(true);
            playerMugshot.texture = birdMugshot;
            speed = birdSpeed;
            dodgeTime = birdDodge;
            //playerRb.constraints = RigidbodyConstraints.FreezePositionY;
            rackingUpCombo = false;
            tigerComboIcon.gameObject.SetActive(false);
            birdComboIcon.gameObject.SetActive(true);
            isFlying = true;
            birdCollider = bird.GetComponent<Collider>();
        }
        transforming = false;
    }
    public void TransformLock()
    {
        cantTransform = !cantTransform;
    }
    public void Heal()
    {
        numberOfItems--;
        healingItemNumber.text = "X " + numberOfItems;
        HP += 3;
        if (HP < originalHP)
        {

            //Taken directly fromlosing HP.For some reason, it just works
            HPBar.fillAmount = (maxHPBarFill / originalHP) * HP;
        }
        if (HP >= originalHP)
        {
            HP = originalHP;
            HPBar.fillAmount = 1;
        }
        StartCoroutine(PickUpHeal());
        StartCoroutine(HealEffect());
        Debug.Log("Healed Up to " + HPBar.fillAmount);
    }
    IEnumerator PickUpHeal()
    {
        pickUpHealEffect.Play();
        yield return new WaitForSeconds(0.5f);
        pickUpHealEffect.Stop();
    }
    IEnumerator HealEffect()
    {
        healingEffect.Play();
        yield return new WaitForSeconds(0.5f);
        healingEffect.Stop();
    }

    public void TigerKnockBack()
    {
        tigerKnockedBack = true;
        StartCoroutine(StunDuration());
    }
    public void BirdKnockBack()
    {
        birdKnockedBack = true;
        StartCoroutine(StunDuration());
    }
    //The main difference between DamageStunStart() and StunDuration() is that StunDuration() provides stunnedInvincibility
    //while DamageStunStart() doesn't. I may make DamageStunStart take up less time than StunDuration(
    IEnumerator DamageStunStart()
    { //Was going to place this in LateUpdate, but I realized it isn't necessary
        damageStun = true;
        cantMove = true;
        //I thought it would be enough if I used running == true and damageStun and stunnedInvincibility in Update,
        //but I think it'snot enough
        if (running == true)
        {
            running = false;
        }
        yield return new WaitForSeconds(2f);
        damageStun = false;
        cantMove = false;
    }
    //Turn this into StunInvincibilityDuration() because that is the main purpose of
    IEnumerator StunDuration()
    {
        
        stunnedInvincibility = true;
        cantMove = true;


        //The display code works because of the camera. Because of the camera, the player character is always at the center of it, where
        //The text also is
        if (running == true)
        {
            running = false;
        }
        yield return new WaitForSeconds(2f);
        if (tigerKnockedBack == true)
        {
            tigerKnockedBack = false;

            //tigerRB.AddTorque(Vector3.right * 12, ForceMode.Impulse);
            tiger.transform.Rotate(-16.5f, -8f, -1.5f);
            //tigerRB.AddForce(Vector3.down * 5, ForceMode.Impulse);
            tiger.transform.Translate(0, -0.2f, 0);
            playerRb.constraints = RigidbodyConstraints.FreezePositionY;
            playerRb.constraints = RigidbodyConstraints.FreezeRotationZ;
        }
        else if (birdFlinch == true)
        {
            //birdRB.AddTorque(Vector3.right * 0.75f, ForceMode.Impulse); //tilt back
            //birdRB.AddTorque(Vector3.down * 0.25f, ForceMode.Impulse);
            //Have to rotatet the bird back because the bird doesn't have gravity
            //bird.transform.Rotate(109.1f, 60f, 78);
            birdFlinch = false;
            //Debug.Log("Flinch done");
        }
        else if (birdKnockedBack == true)
        {
            birdRB.AddTorque(Vector3.right * 1.1f, ForceMode.Impulse); //tilt back
            //bird.transform.Rotate(140.1f, 0, 0);
            birdKnockedBack = false;
            //Debug.Log("Flinch done");
        }
        stunnedInvincibility = false;
        cantMove = false;
    }
    //public void DisplayHP(int currentHP)
    //{
        //HPText.text = "" + HP;
    //}
    public void LoseHP(int damage, int stunType)
    {

        //float damageDone = damage / originalHP / maxHPBarFill;

        //HPBar.fillAmount = HPBar.fillAmount - damageDone;
        HPBar.fillAmount = (maxHPBarFill / originalHP) * (HP - damage);
        //HPBar.fillAmount -= damage / maxHPBarFill;
        HP -= damage;
        damageForDisplay = damage;
        damageDisplay.gameObject.SetActive(true);

        damageDisplay.text = "" + damageForDisplay;
        //Debug.Log(HPBar.fillAmount);
        Debug.Log(HP);
        playerAudio.PlayOneShot(damaged, 0.1f);
        //Putting stun animations here because I need to feed a method/IEnumerator with what stun type I'm going to
        if (tigerActive == true)
        {
            //Debug.Log(stunType % 2);
            if (stunType % 2 == 1)
            {
                animation.Play("Flinch 1");
            }
            else if (stunType % 2 == 0)
            {
                animation.Play("Flinch 2");
            }
        }
        if (birdActive == true && noMoreTurn == false)
        {
            originalRotation = bird.transform.rotation;
            noMoreTurn = true;
            //Debug.Log(stunType % 2);
            if (stunType % 2 == 1)
            {
                //animation.Play("Flinch 1");
                bird.transform.Rotate(-30, -25, 0);
                StartCoroutine(Reorient1());
            }
            else if (stunType % 2 == 0)
            {
                //animation.Play("Flinch 2");
                bird.transform.Rotate(-30, 25, 0);
                StartCoroutine(Reorient0());
            }
        }
        if (tigerKnockedBack == true)
        {
            animation.Play("Hit Back T");

        }
        else if (birdFlinch == true)
        {
            //Flinch, less tilt back and a tilt towards the bird's left
            //Knockback, more tilt and no left or
            //Back right z turn
            //Left tilt up x
            //Up tilt horizontal y
            birdRB.AddTorque(Vector3.left * 0.75f, ForceMode.Impulse); //tilt backwards
            birdRB.AddTorque(Vector3.up * 0.25f, ForceMode.Impulse);
        }
        else if (birdKnockedBack == true)
        {
            birdRB.AddTorque(Vector3.left * 1.1f, ForceMode.Impulse);
        }
        if (rackingUpCombo == true)
        {
            rackingUpCombo = false;
        }
        
        //This was for showing the damage done from each attack on the Player, but isn't necessary because the player and that
        //text is always at the center of the screen
        //damageDisplay.transform.position = new Vector3(tiger.transform.position.x, tiger.transform.position.y+ 5, tiger.transform.position.z);
        if (HP <= 0)
        {
            //Destroy(gameObject);
            //gameObject.SetActive(false);
            //Time.timeScale = 0;
            //Debug.Log("Game O");
            StartCoroutine(GameOverSlowDown());
            death = true;
            gameManagerScript.SetGameOver();
            SceneManager.LoadScene("Game Over Screen");
        }
    }
    IEnumerator Reorient1()
    {
        yield return new WaitForSeconds(0.5f);
        noMoreTurn = false;
        bird.transform.rotation = originalRotation;
        //Debug.Log("Return");
    }
        IEnumerator Reorient0()
    {
        yield return new WaitForSeconds(0.5f);
        noMoreTurn = false;
        bird.transform.rotation = originalRotation;
    }
    IEnumerator DamageDisplayed()
    {
        yield return new WaitForSeconds(0.5f);
        damageDisplay.gameObject.SetActive(false);
    }
    public void IncreaseHealingItems()
    {
        numberOfItems++;
        healingItemNumber.text = "X " + numberOfItems;
    }
    //Was originally going to place several of these variables in Tiger, but it would have been complicated for not very good reason
    public void DisplayNumberOfItems()
    {
        healingItemDisplay.SetActive(true);
        healingItemNumber.text = "X " + numberOfItems;
    }

    //This part will be dedicated to cutscenes
    //I think for this part, i will have the tiger run up to a sensor and destroy the sensor and start the game
    //That way the gameplay will always start when the tiger reaches a certain part. The same way I will have foes manifest in each aren
    /// <summary>
    /// For somereason, I need to set cantMove to private and to true from the very beginning, even though I am successfully changing
    /// cantMove to 
    /// </summary>
    public void Cutscenes()
    {
        cantMove = true;
        //Debug.Log("Can't Move Should Be " + cantMove);
    }
    public void CutsceneOff()
    {
        cantMove = false;
        //Debug.Log("Can't Move is equal to" + cantMove);
    }
    public void OpeningRun()
    {
        running = true;
        //tigerAnimator.SetBool("Idle", false);
        playerRb.AddForce(Vector3.forward * speed);
        //tigerControl.Move(Vector3.forward * speed * Time.deltaTime);
    }
    public void RunAnimationOff()
    {
        running = false;
    }
    //public void OpeningRunDone()
    //{
        //animation.Play("Idle Tweak");
    //}
    public void PauseGame()
    {
        pauseMessage.gameObject.SetActive(true);
        Time.timeScale = 0;
        paused = true;
    }
    public void UnpauseGame()
    {
        pauseMessage.gameObject.SetActive(false);
        Time.timeScale = 1;
        paused = false;
    }
    IEnumerator GameOverSlowDown()
    {
        Time.timeScale = 0.2f;
        //gameOverMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        //Time.timeScale = 0;
        //Time.timeScale = 1; //The interesting thingis that is that the timeScale doesn't set back to . gameOverMessage does setback, 
        
    }
    public void OnCollisionEnter(Collision collision)
    {

        //For some reason trigger doesn't work with meat, but it worked for attack range
        if (collision.gameObject.CompareTag("Heal"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(PickUpHeal());
            IncreaseHealingItems();
            //if (noHealingItems == true)
            //{
                //noHealingItems = false;
                //DisplayNumberOfItems();
            //i}
        }
        //May have to call anything that stops the player Wall
        //May need to put this in PlayerController
        //Worked so far because last time I did tigerSpecial near a wall, it caused Tiger to be rotated off the x-ax
        if (collision.gameObject.CompareTag("Wall") && (attack == true || specialInvincibility == true))
        {
            playerRb.velocity = Vector3.zero;
        }
        if (collision.gameObject.CompareTag("Targeted Enemy") && (closeTheDistance == true || specialCloseTheDistance == true))
        {
            if (closeTheDistance == true)
            {
                closeTheDistance = false;
                //StartCoroutine(FreezeRotations());
                attackTimeLength = distanceCloserTigerAttackLength;
                StartCoroutine(AttackDuration());
                if (tigerActive == true)
                {
                    animation.Play("Attack 1 & 2");
                    //playerRb.constraints = RigidbodyConstraints.FreezeRotation;
                    playerAudio.PlayOneShot(tigerSwing, 0.05f);
                }
                else if (birdActive == true)
                {

                }
            }
            else if (specialCloseTheDistance)
            {
                if (tigerActive == true)
                {
                    animation.Play("Attack 1 & 2");
                    TigerSpecial();
                    StartCoroutine(TigerSpecialDuration());
                }
                if (birdActive == true)
                {
                    StartCoroutine(BirdSpecialDuration());

                }

                specialCloseTheDistance = false;
            }
           }
        if (collision.gameObject.name == "Checkpoint")
        {
            //SceneManager.LoadScene("Armadillo Scene");
            SceneManager.LoadScene("Level 2");
        }
        else if (collision.gameObject.name == "Checkpoint 2")
        {
            SceneManager.LoadScene("Temp Level 3");
        }
        else if (collision.gameObject.name == "Checkpoint 3")
        {
            SceneManager.LoadScene("Boss Draft");
        }
        if (collision.gameObject.CompareTag("Wall") && (damageStun == true || stunnedInvincibility == true))
        {
            playerRb.velocity = Vector3.zero;
            //if (attackingEnemy.giantEnemy == false)
            //{
                //attackingEnemy.transform.Translate(Vector3.back * 10);
            //}
            //if (attackingEnemy.giantBoss == false)
            //{
                //attackingEnemy.GetComponent<Rigidbody>().AddForce(Vector3.back * 10, ForceMode.Impulse);
            //}
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Targeted Enemy") && (closeTheDistance == true || specialCloseTheDistance == true))
        {
            if (closeTheDistance == true)
            {
                closeTheDistance = false;
                //StartCoroutine(FreezeRotations());
                attackTimeLength = distanceCloserTigerAttackLength;
                StartCoroutine(AttackDuration());
                if (tigerActive == true)
                {
                    animation.Play("Attack 1 & 2");
                    //playerRb.constraints = RigidbodyConstraints.FreezeRotation;
                    playerAudio.PlayOneShot(tigerSwing, 0.05f);
                }
                else if (birdActive == true)
                {

                }
            }
            else if (specialCloseTheDistance)
            {
                if (tigerActive == true)
                {
                    animation.Play("Attack 1 & 2");
                    TigerSpecial();
                    StartCoroutine(TigerSpecialDuration());
                }
                if (birdActive == true)
                {
                    StartCoroutine(BirdSpecialDuration());

                }

                specialCloseTheDistance = false;
            }
        }

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Start Game Boundary")
        {
            gameManagerScript.StartLevelMethod();
            //For some reason, now the below methods don'twork in GameManager.Maybe GameManager isn't functioning fasten
            CutsceneOff();
            RunAnimationOff();
            Destroy(other);
            //RunAnimationOff(); //For some reason,now it doesn't work in Game Mana
            //Debug.Log("Game Start");
        }
//The thing that was triggering the colliders by accident was the motion blur object. I have removed its collider fornow
//The last part is so that only Bird can triggerthis trigger
//If I want to make the checkpoints triggers again, I could make it so that the gameObject's name has to be
//Tiger or Bird so that the motion blue object can't trigger
///.name == "Bird" didn'twork
        if (other.gameObject.name == "End Transformation"&&birdActive == true)
        {
            if (cantTransform == false)
            {
                StartCoroutine(TransformCountdown());
                //I think I will have tags designated for Walls to fly over
                //Surprisingly, bird.transform.y worked the same way as transform.position.y + 2.2f
                //transform.position = new Vector3(transform.position.x, bird.transform.position.y - 0.1f, transform.position.z);
            }
            cantTransform = true;
            //Debug.Log("Triggered?");
        }
        //else
        //{
            //cantTransform = false;
        //}


        //Play attack effect in Enemy and load the effect in the individual script. IE, if Xemnas is using his ethereal blades,
        //load the ethereal blade effect into the private variable in Enemy. If Xemnas is using his spark orbs, load the spark orbs
        //effect into the private variable in Enem
        ///Was going to do a successfuldodge bool, but that would make the player able to be invincibile to other foe's attacks
        ///after a successfuldodge fora time
        ///wasGoing to use an arbitrary IEnumerator of 0.5s, butIdecided to have the enemy individualscript reset the bools instead
        ///so it isn't as arbitrary and it make sense because the enemy's attackrange as disappeared at that 
        ///This is inspired by how Terra-Xehanort Datauses a slam attack and if the player dodged it, the lingering effect will not damage Sora
        ///but if he does get hitby it, it does have a lingering hitbox because he can getdamaged again by falling into the spikes
        ///I don't have to reset values for projectiles because projectiles will get destroyed after a while
        ///It's a good thing I thought of thisbecause I didn't account for making the player unable to dodgeinto the fire crater
        ///The main point of this is to make a player able to dodge an attack and not get hit just because they're stuck around the 
        ///hitbox. This does have the unintentional downside of making it so that players potentially can't be hit by say Sephiroth
        ///creating a circle of slashes around himself that hit players multiple times unless they dodge out of range or block
        ///but I think isLingingeraccounts for that
        ///I didn't think of this at first because usually attack ranges disappearreally quickly, but I think Gorilla attack got me think
        if(other.CompareTag("Enemy Attack Range") && (dodge == true))
        {
            Enemy currentEnemyScript = other.gameObject.GetComponentInParent<Enemy>();
            if (currentEnemyScript.isLingering == false)
            {
                currentEnemyScript.SetPlayerDodged();
            }
            
        }
        //Idon't think I need to check for enemyScript.playerDodgedeven though Iwanted to. I could always check inside the loop
        //after accessing enemyScript in the condition
        //Weeks later, i think I completelyforgotabout the above
        if (other.CompareTag("Enemy Attack Range") && (dodge == false && specialInvincibility == false && stunnedInvincibility == false))
        {
            Enemy currentEnemyScript = other.gameObject.GetComponentInParent<Enemy>();
            attackingEnemy = currentEnemyScript;

            //if (wolfScript.attackLanded == false)
            //{

            //LoseHP(wolfScript.damage);
            //TigerFlinching(); //Have evoke this one last because this one triggers the StunDuration, and the above
            //Sets the value of damage
            //This is going to be more challenging, because I need a specific Wolf's attack direc
            //I got it, draw a wolf script from the other.gameObject.
            //I hope this doesn't cause an issue when multiple attacks land on the player
            //Serendipity, I can use this to determine damage
            //
            //wolfScript.SetAttackLanded();
            //wolfScript.PlayAttackEffect();

            //Attack Force will have to be fed to Enemy

            //Need to put a boolean on a foe's individual script to tell if an attack has knockback and then use an if to apply
            //the knockbackforce



            if (currentEnemyScript.playerDodged == false)
            {
                //This is so that player can't be hit more than once by a nonlingering att
                if (currentEnemyScript.isLingering == false)
                {
                    currentEnemyScript.SetPlayerDodged();
                }
                //This is for tiger
                if (tigerActive == true)
                {
                    //playerRb.velocity = attackDirection * (attackForce + 20) exam
                    if (currentEnemyScript.leftAttack == true)
                    {
                        //playerRb.AddForce(Vector3.left * currentEnemyScript.attackForce, ForceMode.Impulse);
                        playerRb.velocity = Vector3.left * currentEnemyScript.attackForce;
                    }
                    if (currentEnemyScript.rightAttack == true)
                    {
                        //playerRb.AddForce(Vector3.right * currentEnemyScript.attackForce, ForceMode.Impulse);
                        playerRb.velocity = Vector3.right * currentEnemyScript.attackForce;
                    }
                    if (currentEnemyScript.backAttack == true)
                    {
                        //playerRb.AddForce(Vector3.back * currentEnemyScript.attackForce, ForceMode.Impulse);
                        playerRb.velocity = Vector3.back * currentEnemyScript.attackForce;
                    }
                }
                else if (birdActive == true && currentEnemyScript.canHurtFlying == true)
                {
                    if (isFlying == false)
                    {
                        isFlying = true;
                        bird.transform.Translate(0, 1.5f, 0);
                        birdSeparater.transform.Translate(0, -1.5f, 0);
                        //birdSeparater.SetActive(true);
                    }
                    if (currentEnemyScript.leftAttack == true)
                    {
                        //playerRb.AddForce(Vector3.left * currentEnemyScript.attackForce, ForceMode.Impulse);
                        playerRb.velocity = Vector3.left * currentEnemyScript.attackForce;
                    }
                    if (currentEnemyScript.rightAttack == true)
                    {
                        //playerRb.AddForce(Vector3.right * currentEnemyScript.attackForce, ForceMode.Impulse);
                        playerRb.velocity = Vector3.right * currentEnemyScript.attackForce;
                    }
                    if (currentEnemyScript.backAttack == true)
                    {
                        //playerRb.AddForce(Vector3.back * currentEnemyScript.attackForce, ForceMode.Impulse);
                        playerRb.velocity = Vector3.back * currentEnemyScript.attackForce;
                    }
                }
                currentEnemyScript.AttackLanded(0);
                //playerRb.AddForce(Vector3.back * 12, ForceMode.Impulse); //I don't know why I have this
                //playerScript.AttackLandedTrue();
                //}
                LoseHP(currentEnemyScript.damage, currentEnemyScript.hitNumber);
                StartCoroutine(DamageDisplayed());

                if (currentEnemyScript.comboFinisher == false)
                {
                    StartCoroutine(DamageStunStart());
                }
                //enemyScript.PlayAttackEffect();
                //Removed checking for comboAttack because the player only gets stunnedInvincibilityfrom combo finish
                //I probably didn't realize this when designing combo att
                if (currentEnemyScript.comboFinisher == true)
                {
                    StartCoroutine(StunDuration());
                    Debug.Log("Stun Duration is equal to " + stunnedInvincibility);
                    //playerRb.AddForce(-orientation.forward * enemyScript.attackForce, ForceMode.Impulse);
                }
            }
        }
        if (other.CompareTag("Projectile") && (dodge == true))
        {
            Projectile projectileScript = other.gameObject.GetComponent<Projectile>();
            if (projectileScript.isLingering == false)
            {
                projectileScript.SetPlayerDodged();
            }

        }
        if (other.CompareTag("Projectile") && (dodge == false && specialInvincibility == false && stunnedInvincibility == false))
        {
            Projectile projectileScript = other.gameObject.GetComponent<Projectile>();
            //playerRb.AddForce(-orientation.forward * projectileScript.attackForce, ForceMode.Impulse);
            //projectileScript.AttackLanded(0);

            //enemyScript.PlayAttackEffect();
            if (tigerActive == true)
            {
                if (projectileScript.leftAttack == true)
                {
                    //playerRb.AddForce(Vector3.left * projectileScript.attackForce, ForceMode.Impulse);
                    playerRb.velocity = Vector3.left * projectileScript.attackForce;
                }
                if (projectileScript.rightAttack == true)
                {
                    //playerRb.AddForce(Vector3.right * projectileScript.attackForce, ForceMode.Impulse);
                    playerRb.velocity = Vector3.right * projectileScript.attackForce;
                }
                if (projectileScript.backAttack == true)
                {
                    //playerRb.AddForce(Vector3.back * projectileScript.attackForce, ForceMode.Impulse);
                    playerRb.velocity = Vector3.left * projectileScript.attackForce;
                }
                LoseHP(projectileScript.damage, 1);
                StartCoroutine(DamageDisplayed());
                if (projectileScript.comboFinisher == false)
                {
                    StartCoroutine(DamageStunStart());
                }
                if (projectileScript.comboFinisher == true)
                {
                    StartCoroutine(StunDuration());
                    Debug.Log("Stun Duration is equal to " + stunnedInvincibility);
                    //playerRb.AddForce(-orientation.forward * enemyScript.attackForce, ForceMode.Impulse);
                }
            }
            else if (birdActive == true && projectileScript.canHurtFlying == true)
            {
                if (isFlying == false)
                {
                    isFlying = true;
                    bird.transform.Translate(0, 1.5f, 0);
                    birdSeparater.transform.Translate(0, -1.5f, 0);
                    //birdSeparater.SetActive(true);
                }
                if (projectileScript.leftAttack == true)
                {
                    //playerRb.AddForce(Vector3.left * projectileScript.attackForce, ForceMode.Impulse);
                    playerRb.velocity = Vector3.left * projectileScript.attackForce;
                }
                if (projectileScript.rightAttack == true)
                {
                    //playerRb.AddForce(Vector3.right * projectileScript.attackForce, ForceMode.Impulse);
                    playerRb.velocity = Vector3.right * projectileScript.attackForce;
                }
                if (projectileScript.backAttack == true)
                {
                    //playerRb.AddForce(Vector3.back * projectileScript.attackForce, ForceMode.Impulse);
                    playerRb.velocity = Vector3.left * projectileScript.attackForce;
                }
                LoseHP(projectileScript.damage * 2, 1);
                StartCoroutine(DamageDisplayed());
                //playerRb.AddForce(Vector3.back * projectileScript.attackForce, ForceMode.Impulse);
                if (projectileScript.comboFinisher == false)
                {
                    StartCoroutine(DamageStunStart());
                }
                if (projectileScript.comboFinisher == true)
                {
                    StartCoroutine(StunDuration());
                    Debug.Log("Stun Duration is equal to " + stunnedInvincibility);
                    //playerRb.AddForce(-orientation.forward * enemyScript.attackForce, ForceMode.Impulse);
                }
            }

        }
        if (other.CompareTag("Guard"))
        {
            guardingEnemy = other.GetComponentInParent<Enemy>();
            guardingEnemy.GuardTriggered();
        }

    }
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NoTransformation") && cantTransform == true)
        {
            cantTransform = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NoTransformation") && cantTransform == true)
        {
            cantTransform = false;
        }
        if (other.CompareTag("Guard"))
        {
            guardingEnemy.GuardUntriggered();
        }
    }
}

