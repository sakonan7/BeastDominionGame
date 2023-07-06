using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    //public Transform cam;
    public GameObject cameraRef;
    public GameObject orientationObject;
    public Transform orientation;

    //Character move
    private CharacterController tigerControl;
    private CharacterController birdControl;

    private new Animation animation;
    private new Animation birdAnimation;
    public bool tigerActive = true;
    public bool birdActive = false;

    public GameObject tiger;
    public GameObject bird;

    //private BoxCollider tigerCollider;
    //private BoxCollider birdCollider;

    //public GameObject tigerSensor;
    //public GameObject birdSensor;
    //public GameObject monkeyRange;
    //public GameObject wolfRange;
    //public GameObject sensor;

    //public GameObject dodgeEffect;
    


    public bool specialInvincibility = false;
    public GameObject bladeOfLight;
    private bool charging = false;

    [Header("Place tiger and bird sound and effects here")]
    public AudioClip tigerRegularStrike;
    public ParticleSystem regularHitEffect;
    public GameObject tigerAttackEffect;
    public Transform transformEffect;
    public AudioClip tigerSpecial;
    public AudioClip tigerSpecialStrike;
    public ParticleSystem specialHitEffect;

    //Either make the Rigidbody bigger or make a sensor
    //*Rigidybody might not be better because the tiger will spin. I can stop the sensor from rotating, but not the
    //Rigidbody. Rigidbody would just give a more realistic anima

    private float speed = 36; //Changed from 30 because now I'm using charactercontroller
    private float birdSpeed = 40;
    private float dodgeForce = 15; //90 barely jumps
    public float attackForce = 5;
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
    private float attackTimeLength;
    private float normalTigerAttacklength = 0.3f;
    private float distanceCloserTigerAttackLength = 0.1f;
    private Quaternion attackRotation;
    //private float distance;

    private Rigidbody tigerRB;
    private bool running = false;

    private Rigidbody birdRB;
    private Vector3 resetY;

    public bool dodge = false;
    public bool lag = false;
    public bool attack = false;
    public bool swoopLag = false;
    public bool attackLanded = false;

    private bool transforming = false;
    public bool stunnedInvincibility;
    private float invincibleFrame;

    public GameObject target;
    private GameObject targetedEnemy;
    private Vector3 enemyTargetPosition;
    private Rigidbody foeRB;
    public bool canLockOn = false;
    public bool lockedOn = false;
    public GameObject focalPoint; //Turn camera towards targetted foe
    //New lock on code
    private float distance = 0;
    private bool lockOnTurnedOn = false;

    public Vector3 attackDirection;
    public Vector3 transformPosition;

    //Taking damage
    private bool tigerFlinch = false;
    private bool tigerFlinch2 = false;
    private bool tigerKnockedBack = false;
    private bool birdFlinch = false;
    private bool birdKnockedBack = false;
    public bool stunned = false;
    public bool invincible = false;

    //Sound
    private AudioSource playerAudio;
    public AudioClip tigerRoar;
    public AudioClip damaged;
    public AudioClip knockBack;
    public AudioClip birdCry;
    public AudioClip bladeOfLightChargeUp;
    public AudioClip tigerSwing;

    //Display
    public TextMeshProUGUI HPText;
    //public Image playerMugshot;
    public int HP = 10;
    public RawImage playerMugshot;
    public Texture tigerMugshot;
    public Texture birdMugshot;
    public Image HPBar;
    float maxHPBarFill; //References the max amount of physical space the HP fills
    public TextMeshProUGUI damageDisplay;
    private int damageForDisplay; //Proxy variable just for displaying damage taken
    public GameObject healingItemDisplay;
    public TextMeshProUGUI healingItemNumber;
    public int numberOfItems = 0;
    //RectTransform newTargetRect;
    //public GameObject newTarget;
    public TextMeshProUGUI comboCounter; //Make it slightly increase in size and become yellow which each. When it reaches 3, make it
    //yellow orange, then a yellower orange for hits after 3, then light blue for 6 hits and the special attack pops
    //Then a tiger roar

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
    void Start()
    {
        tigerControl = tiger.GetComponent<CharacterController>();
        birdControl = bird.GetComponent<CharacterController>();
        //cam = cameraRef.transform;

        tigerRB = tiger.GetComponent<Rigidbody>();
        animation = tiger.GetComponent<Animation>();

        //tigerCollider = tiger.GetComponent<BoxCollider>();
        //birdCollider = bird.GetComponent<BoxCollider>();
        //tigerSensor = GameObject.Find("Tiger Sensor");

        birdRB = bird.GetComponent<Rigidbody>();
        birdAnimation = bird.GetComponent<Animation>();
        //birdSensor = GameObject.Find("Bird Sensor");

        tigerActive = true;
        //tiger.SetActive(true);
        //tigerSensor.SetActive(true);
        birdActive = false;
        bird.SetActive(false);

        playerAudio = GetComponent<AudioSource>();
        //playerUI = GameObject.Find("Canvas");
        //DisplayHP(HP);
        maxHPBarFill = HPBar.fillAmount;

        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //ballRB = ball.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(distance);
        if (Input.GetKeyDown(KeyCode.F) && paused == false)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.F) && paused == true)
        {
            UnpauseGame();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            //Instantiate(ball, new Vector3(tiger.transform.position.x - 1, tiger.transform.position.y + 3f, tiger.transform.position.z), ball.transform.rotation);
            //Instantiate(ball, new Vector3(tiger.transform.position.x - 2, tiger.transform.position.y + 2f, tiger.transform.position.z), ball.transform.rotation);
            //Instantiate(ball, new Vector3(tiger.transform.position.x - 3, tiger.transform.position.y + 1f, tiger.transform.position.z), ball.transform.rotation);
            //Instantiate(ball, new Vector3(tiger.transform.position.x + 1, tiger.transform.position.y + 2.5f, tiger.transform.position.z), ball.transform.rotation);
            //Instantiate(ball, new Vector3(tiger.transform.position.x + 2, tiger.transform.position.y + 1.5f, tiger.transform.position.z), ball.transform.rotation);
            //Instantiate(ball, new Vector3(tiger.transform.position.x + 1, tiger.transform.position.y + 3f, tiger.transform.position.z), ball.transform.rotation);
            //Instantiate(ball, new Vector3(tiger.transform.position.x + 2, tiger.transform.position.y + 2f, tiger.transform.position.z), ball.transform.rotation);
            //Instantiate(ball, new Vector3(tiger.transform.position.x + 3, tiger.transform.position.y + 1f, tiger.transform.position.z), ball.transform.rotation);
            //ballRB.AddForce(Vector3.fwd * 30, ForceMode.Impulse);
        }

        //The attack ranges will always be at y=-0.11, now-0.34f because I put back under parent, now 0 and the rotations will always be equal to 0
        //monkeyRange.transform.rotation = new Quaternion(0, 0, 0, 0);
        //wolfRange.transform.rotation = new Quaternion(0, 0, 0, 0);
        //sensor.transform.rotation = new Quaternion(0, 0, 0, 0);
        //dodgeEffect.transform.rotation = new Quaternion(0, 0, 0, 0);

        //This code is for anything that needs to follow the player
        if (birdActive == true)
        {
            //monkeyRange.transform.position = new Vector3(bird.transform.position.x, 0.45f, bird.transform.position.z - 0.1f);
            //wolfRange.transform.position = new Vector3(bird.transform.position.x, 0.45f, bird.transform.position.z - 0.1f);
            //sensor.transform.position = new Vector3(bird.transform.position.x, 0.45f, bird.transform.position.z - 0.1f);
            //dodgeEffect.transform.position = new Vector3(bird.transform.position.x, 0.45f, bird.transform.position.z - 0.1f);
        }
        else if (tigerActive == true)
        {
            //monkeyRange.transform.position = new Vector3(tiger.transform.position.x, 0.45f, tiger.transform.position.z - 0.1f);
            //wolfRange.transform.position = new Vector3(tiger.transform.position.x, 0.45f, tiger.transform.position.z - 0.1f);
            //sensor.transform.position = new Vector3(tiger.transform.position.x, 0.45f, tiger.transform.position.z - 0.1f);
            //dodgeEffect.transform.position = new Vector3(tiger.transform.position.x, 0.45f, tiger.transform.position.z - 0.1f);
            orientationObject.transform.position = new Vector3(tiger.transform.position.x, tiger.transform.position.y + 0.77f, tiger.transform.position.z + 0.82f);
            orientationObject.transform.rotation = tiger.transform.rotation;
        }

        //movement
        forwardInput = Input.GetAxis("Vertical");
        sideInput = Input.GetAxis("Horizontal");
        //direction = new Vector3(forwardInput, 0, sideInput).normalized;


        //Trying to keep Player empty gameObject around the same place as the characters
        //I don't think it works because wherever Player goes, the nested objects have to go too
        //if (birdActive == true)
        //{
        //transform.Translate(bird.transform.position.x, 0, bird.transform.position.z); //Causes the bird to soar right
        //transform.position = bird.transform.position; //Causes the bird to soar up
        //}
        //Literally causes the tiger to fly lmao
        //This code literally doesn't work for some reason lol
        //if (tigerActive == true)
        //{
        //transform.position = new Vector3(tiger.transform.position.x, transform.position.y, tiger.transform.position.z);
        //}

        //Wrap all the code in the instances where the Player can't do anything, like stunlocking and transforming
        //if (gameManagerScript.startGame == true)
        //{
        //Idle animat
        if (dodge == false && attack == false && tigerActive == true && lag == false && running == false && stunned == false)
            {
                animation.Play("Idle Tweak");
            }
            if (dodge == false && attack == false && birdActive == true && lag == false && stunned == false)
            {
                birdAnimation.Play("Idle");
            }

        //Movement
        ///Gonna try to make a general case for all controls where you can't do anything if you're lagged or stunned or using a special
        ///Or if you're using a dodge or if you're using an attack. Just put every case where the player shouldn't be mov
        ///This is so Players can't repeat an action while the action is happen
        if (lag == false && attack == false && dodge == false && stunned == false && transforming == false && closeTheDistance == false)
        {
            //I can still perform attack while running, so I will need to make sure I can't run while attacking. Just checked,
            //the same happens when I use a dodge
            //I already checked, I can't move while having an attack or dodge lag
            ///The current conditions I have put in haven't solved anything yet, so I will have to try something
            ///maybe try a running boolean
            ///this isn't a problem for attack, but it's a problem for dodging
            ///I think that the problem is that the animation keeps going, not that the action gets interrupt
            ///


            if (birdActive == true && attack == false && dodge == false)
            {
                //targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                //This is just for the rotating, not for the character's horizontal
                //or vertical move
                //angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //bird.transform.rotation = Quaternion.Euler(0, angle, 0);
                //moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                //birdRB.AddForce(moveDir.normalized * speed);
                birdRB.AddForce(Vector3.forward * speed * forwardInput);
                birdRB.AddForce(Vector3.right * speed * sideInput);
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    running = true;
                }
                else
                {
                    running = false;
                }
            }
            //Tiger has different code for movement because ineed to trigger the running animat
            else if (tigerActive == true && attack == false && dodge == false && gameManagerScript.startingCutscene == false)
            {
                moveDirection = orientation.forward * forwardInput + orientation.right * sideInput;

                //tigerRB.AddForce(moveDir.normalized * speed);
                //tigerRB.AddRelativeForce(Vector3.forward * speed * forwardInput);
                //tigerRB.AddRelativeForce(Vector3.right * speed * sideInput);
                tigerRB.AddForce(moveDirection * speed);

                //animation.Stop();
                //The button presses trigger running bool and running bool triggers animation. Surprisingly simple but not easy to think
                //Just doing the button inputs makes the animation only play one
                //This is different from dodging and attacking because those are single act

                //if (direction.magnitude >= 0.1f)
                //{
                //targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                //angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //tiger.transform.rotation = Quaternion.Euler(0, angle, 0);
                //moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                //tigerControl.Move(moveDir.normalized * speed * Time.deltaTime);
                //}

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    running = true;
                }
                else
                {
                    running = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Create countdown and boolean for dodge
                //transform.Translate(Vector3.forward * 50 * Time.deltaTime);
                //Quick shifting maybe paper like sound
                //att the moment, you have to press both butt
                //Because I don't know how to have the code sense when the player is moving around
                //Only want dodge to work while they are mov
                //if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                //{
                //tigerRB.AddForce(Vector3.forward * dodgeForce, ForceMode.Impulse);
                //}
                //if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                //{
                //playerRB.AddForce(Vector3.left * dodgeForce, ForceMode.Impulse);
                //Debug.Log("Left dodge");
                //StartCoroutine(Dodge());
                //}
                //if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                //{
                //playerRB.AddForce(Vector3.right * dodgeForce, ForceMode.Impulse);
                //Debug.Log("Right dodge");
                //StartCoroutine(Dodge());
                //}
                //if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                //{
                //playerRB.AddForce(Vector3.down * dodgeForce, ForceMode.Impulse);
                //Debug.Log("Backward dodge");
                //StartCoroutine(Dodge());
                //}

                //Make an else case for the current code for when the player isn't pressing a direction
                //Placed animations here instead so that the full animation plays
                if (birdActive == true)
                {
                    birdRB.AddForce(Vector3.forward * dodgeForce, ForceMode.Impulse);
                    birdAnimation.Play("Attack");
                    //Try using rotate degrees instead and Vectors and speed
                    //transform.Rotate(360, 0, 0);
                    //bird.transform.Rotate(Vector3.back * 300 * Time.deltaTime); //Doesn't work most likely because I need to use Time.deltaTime but
                    //something that counts milliseconds, so that bird can spin a full 360 in one second
                    //Use torque
                    birdRB.AddTorque(Vector3.back, ForceMode.Impulse);
                    //Quick fix, but it actually worked. Running animation 
                    //I tried putting a big case above all the controls, but it doesn't
                    //For some reason, this is necessary in the code for the actions
                    if (running == true)
                    {
                        running = false;
                    }
                }
                else if (tigerActive == true)
                {
                    //Tried tiger.transform.forward, but the transform of the tiger doesn't
                    tigerRB.AddForce(moveDirection * dodgeForce, ForceMode.Impulse);
                    animation.Play("Jump Tweak");
                    if (running == true)
                    {
                        running = false;
                    }
                }

                StartCoroutine(Dodge());
            }
            if (Input.GetKeyDown(KeyCode.E) && dodge == false && attack == false && stunned == false)
            {
                //Transform();
                //Debug.Log("Transform");
                StartCoroutine(TransformCountdown());
                if (running == true)
                {
                    running = false;
                }
            }
            //Attacking
            ///Small note, I accidentally used & instead of && for lockedOn. It seems like it didn't affect the code
            ///Doing AttackDuration in attack methods instead
            if (Input.GetMouseButtonDown(0) &&lockedOn == true)
            {
                

                //attackDirection isn't changing directions because it's using the empty Player object as the reference and the Player
                //object doesn't move
                if (birdActive == true)
                {
                    attackDirection = (target.transform.position - bird.transform.position).normalized;
                    Swoop();
                }
                else if (tigerActive == true)
                {
                    attackDirection = (target.transform.position - tiger.transform.position).normalized;
                    //A little wonky atm
                    //Definitely doesn't
                    //replaced targetedEnemy.transform.position
                    ///Fixed by using tiger.transform, while in two places, I used transform instead
                    ///This is still not working properly though, because tiger is only rotating towards the foe only when it is very
                    //Vector3 newDirection = Vector3.RotateTowards(tiger.transform.forward, targetedEnemy.transform.position, Time.deltaTime, 0.0f);
                    //tiger.transform.rotation = Quaternion.LookRotation(newDirection);

                    //Vector3 targetDirection = targetedEnemy.transform.position - tiger.transform.position;
                    //Vector3 newDirection = Vector3.RotateTowards(tiger.);

                    //Vector3 targetDirection = target.transform.position - tiger.transform.position;
                    //float singleStep = Mathf.PI * Time.deltaTime;
                    //Vector3 newDirection = Vector3.RotateTowards(tiger.transform.forward, targetDirection, singleStep, 0.0f);
                    //tiger.transform.rotation = Quaternion.LookRotation(newDirection);
                    attackRotation = Quaternion.LookRotation(target.transform.position - tiger.transform.position);
                    Strike();
                }
                if (running == true)
                {
                    running = false;
                }
            }
            if (Input.GetMouseButtonDown(0) && lockedOn == false)
            {
                
                attackDirection = Vector3.fwd; //changed from Vector3.fwd. Changed back from moveDirection
                if (birdActive == true)
                {
                    //attackDirection = (target.transform.position - bird.transform.position).normalized;
                    Swoop();
                }
                else if (tigerActive == true)
                {
                    //attackDirection = (target.transform.position - tiger.transform.position).normalized;
                    Strike();
                }
                if (running == true)
                {
                    running = false;
                }
            }
            //Special Attack
            if (Input.GetKeyDown(KeyCode.Z) && lockedOn == true)
            {
                //StartCoroutine(TigerSpecialDuration());
                StartCoroutine(ChargeUp());
                attackDirection = (target.transform.position - tiger.transform.position).normalized;
                //TigerSpecial();
                //tigerRB.AddTorque(Vector3.up * 100, ForceMode.VelocityChange);
                //Either use Fight Idle animation
                if (running == true)
                {
                    running = false;
                }
            }
            //wrote this first
            if (Input.GetKeyDown(KeyCode.Z) && lockedOn == false)
            {
                //Summons blade of light and then dashes and performs a 360 degree slash. Need to make sure it's 360
                //Maybe there is a charge up animation
                //A little bit further than a regular attack

                //Just do the animation for now. Need an IEnumerator to make the blade go inactive and for attack duration. Could
                //do it in the same IEnumera
                //StartCoroutine(TigerSpecialDuration());
                StartCoroutine(ChargeUp());
                attackDirection = Vector3.fwd;
                //TigerSpecial();
                //tigerRB.AddTorque(Vector3.up * 100, ForceMode.VelocityChange);
                //Either use Fight Idle animation
                                    if (running == true)
                    {
                        running = false;
                    }
            }

        }

        //These are for things outside of controls such as attack effects and things that will not be affected by anything that are related
        //to stuns such as locking on

        if (running == true)
        {
            animation.Play("Run Tweak");
        }
        //Putting it on here atm because I want this to be interupted by stunning
        //Also, players can't do anything while the character is closing the distance
        //Put it out of the conditional because it's not an action
        if (closeTheDistance == true && stunned == false)
        {
            animation.Play("Distance Closer");
                //I would prefer to use nonImpulse, but it is too slow and using Impulse is unexpectedly cool
                tigerRB.AddForce(attackDirection * 4, ForceMode.Impulse); //attack force wasn't enough //Also, it isn't enough here //Try impulse
                //ForceMode Impulse is amazing. Needed to go from speed to 5 becaue of how fast and far it went
                tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 5);
            //My plan for this is to have the player close the distance and once they're within 1 distance away from a foe, perform the
            //strike. I'm thinking that I don't want to use the regular AttackDuration. Use a reduced AttackDuration
            //Use a set of time limits. For Distance Closer, use 0.2 seconds. The funny thing is, the original attack duration is around
            //0.3 already. That means the duration is already really short
            //make DistanceCloser an IEnumerator
            //It will also perform the Strike animation it
            //closeTheDistance will be part of the upper conditionals
            //Actually, I need another method for the attack part. I'm pretty sure I can cancel closeTheDistance if I program
            //to reach 1. The attack part is DistanceCloserStrike. Otherwise, I may attempt to do it in AttackDuration instead
            //because I don't want the code to be too complicated. It may also work because DistanceCloser's attack duration is supposed
            //to be shorter
            //I think this is a good idea
            //After the distance is met or the duration of DistanceCloser is met, perform the attack
            //Unless player is stunned
            //After the time is reached in DistanceCloser, see if closeTheDistance is true, and if it is true, perform the attack
            //DistanceCloser is oddly complicated lol
            //Have stunned cancel closeTheDistance here
            if (stunned == true)
            {
                closeTheDistance = false;
                Debug.Log("I don't want to do it, but, closeTheDistance = " + closeTheDistance);
            }
            if (distance < 2)
            {
                Debug.Log("Distance met");
                closeTheDistance = false;
                attackTimeLength = distanceCloserTigerAttackLength;
                StartCoroutine(AttackDuration());
                animation.Play("Attack 1 & 2");
                playerAudio.PlayOneShot(tigerSwing, 0.05f);
            }
        }



        if (Input.GetKeyDown(KeyCode.Q))
        {

            TigerFlinching2();
        }

        //Lock On
            //Changing code for now because lock on doesn't work on foes who have been generated into the scene
            //and not already
            //Removed canLockOn == true
        if (Input.GetMouseButtonDown(1))
        {
            //sensor.SetActive(true);
            LockOn();
        }
            //Motion Blur
            //I'm thinking I don't want a motion blur for special attacks
        //Almost forgot to add tigerActive because I don't want the blur for the bird
        if (attack == true && tigerActive == true)
        {
            attackEffect = true;
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
            //Too complicated to place the target on a unique place on a foe atm because I would need to access the enemy's class.
            //Unless I use tags

            //Weirdly fixed by using position instead of Translate(
            //enemyTargetPosition = targetedEnemy.GiveTargetPosition();
            target.transform.position = targetedEnemy.transform.position; //I think I should at most just adjust target's loca
            //target.transform.position = enemyTargetPosition;
            //newTarget.transform.position = new Vector3(targetedEnemy.transform.position.x, targetedEnemy.transform.position.y, 0);

            //Original plan was to keep the original target but inactive to give the player a target to hit
            //And to have the new target appear over the targeted foe
            //newTargetRect = newTarget.GetComponent<RectTransform>();
            //newTargetRect.localPosition = new Vector2(target.transform.position.x, target.transform.position.y);

            //Code to make lockedOn symbol face camera
            //The original simple LookAt(cameraRef.transform) didn't work because it showed the clear backside of the plane/quad instead
            target.transform.LookAt(target.transform.position - (cameraRef.transform.position - target.transform.position));

            //Code to turn camera towards target
            //Test to see if using a method will only place the method once. I don't think so, so try it in the lockedon meth
            distance = Vector3.Distance(targetedEnemy.transform.position, tiger.transform.position); //Didn't realize I'd have
                                                                                                     //Didn't realize I'd have to keep calculating Distance
                                                                                                     //Actually, I will recalculate distance in lockedOn
        }
        else if (lockedOn == false)
        {
            target.SetActive(false);
        }
        if (transforming == true)
        {
            animation.Play("Transform");
            //If birdActive == true, have it tilt all the way up and tilt it back down after the IEnumerator is
        }
        //}


        //if (tigerFlinch == true)
        //{
        //animation.Play("Flinch 1");
        //}

        //Code to ensure the flinching animations will play and interupt any other animation
        //Players are cautioned to not attack with abandon as a result
        if (tigerFlinch == true || tigerFlinch2 == true || birdFlinch == true)
        {
            attack = false;
            dodge = false;
        }

        //Cutscenes
        if (gameManagerScript.startingCutscene == true)
        {
            OpeningRun();
        }
    }

    IEnumerator AttackDuration()
    {
        attack = true;
        if (tigerActive == true)
        {
            //tigerCollider.size = new Vector3(tigerCollider.size.x + 2.5f, tigerCollider.size.y, tigerCollider.size.z + 2.2f);
            //tigerCollider.center = new Vector3(tigerCollider.center.x, tigerCollider.center.y, tigerCollider.center.z + 1.25f);
            tigerAttackEffect.SetActive(true);
        }
        yield return new WaitForSeconds(attackTimeLength);
        attack = false;
        //Debug.Log("Attack");
        if (birdActive == true)
        {
            //Return to the same position in the air
            //Change: Make it so that bird only returns to y position in the air, meaning that like tiger strike,
            //the bird will advance with each attack. Did it by commenting out = reset
            birdRB.AddForce(Vector3.up * 5, ForceMode.Impulse);
            //bird.transform.position = resetY;
        }
        if (tigerActive == true)
        {
            //tigerCollider.size = new Vector3(tigerCollider.size.x - 2.5f, tigerCollider.size.y, tigerCollider.size.z - 2.2f);
            //tigerCollider.center = new Vector3(tigerCollider.center.x, tigerCollider.center.y, tigerCollider.center.z - 1.25f);
            tigerAttackEffect.SetActive(false);
            
        }
        if (attackLanded == false)
        {
            StartCoroutine(StrikeLag());
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
            StartCoroutine(NoAttackLag());
        }
    }
    IEnumerator NoAttackLag()
    {
        yield return new WaitForSeconds(1.5f); //1.3f instead of 0.1f, so lag can't take effect. Temporary?
        attackLanded = false;
    }
    //IEnumerator for charging up Blade of light
    //Blade of light also increases into its full size
    IEnumerator ChargeUp()
    {
        charging = true;
        specialInvincibility = true;
        yield return new WaitForSeconds(1);
        charging = false;
        StartCoroutine(TigerSpecialDuration());
        TigerSpecial();
        playerAudio.PlayOneShot(bladeOfLightChargeUp, 0.2f);
    }

    IEnumerator TigerSpecialDuration()
    {
        attack = true;
        //specialInvincibility = true;
        //playerAudio.PlayOneShot(tigerSpecial, 0.2f);
        bladeOfLight.SetActive(true);
        yield return new WaitForSeconds(2f);
        attack = false;
        specialInvincibility = false;
        bladeOfLight.SetActive(false);
        StartCoroutine(StrikeLag());
    }
    public void Swoop()
    {
        //The intention is to have the bird attack and then return to its starting position when it started its att
        //attackDirection = (target.transform.position - transform.position).normalized;
        //Try using downward and upward forces instead
        //bird.transform.Translate(bird.transform.position.x, 0.8f, bird.transform.position.z);
        birdRB.AddForce(Vector3.down * 5, ForceMode.Impulse);
        birdRB.AddForce(attackDirection * attackForce, ForceMode.Impulse);
        birdAnimation.Play("Attack");
        resetY = new Vector3(bird.transform.position.x, 0.6f, bird.transform.position.z);
        StartCoroutine(SwoopLag());
    }
    public void Strike()
    {
        //float distance = Vector3.Distance(target.transform.position, tiger.transform.position); //I already had a distance calculator
        //from lock on
        //attackDirection = (target.transform.position - transform.position).normalized;
        //My problems with distance not being calculated again is because I didn't recaculate it with each Strike(). Before
        //I moved Debug.Log(distance) to update, I removed the bottom. This is also why Distance Closer won't recalcul
        //I think I need to calculate it constantly in update instead. I calculated it in lockOn because i need to check to see which enemy
        //Is closest to to the player
        //distance = Vector3.Distance(targetedEnemy.transform.position, tiger.transform.position);
        Debug.Log("Attacking from " + distance);
        

        //playerAudio.PlayOneShot(tigerRoar, 0.2f);
        //Why did I think of lockedOn == false, I think I thought of it in the case you aren't locked
        //I think I will create a case where lockedOn == false and for now, I want to make the distance smaller for
        if (lockedOn == false)
        {
            attackTimeLength = normalTigerAttacklength;
            StartCoroutine(AttackDuration());
            tigerRB.AddForce(attackDirection * (attackForce + 10), ForceMode.Impulse);//Changed from 8 to 12
            animation.Play("Attack 1 & 2");
            playerAudio.PlayOneShot(tigerSwing, 0.05f);
        }
        //I guestimated from gameplay that the distance needs to be at least 15
        if ((distance > 10 || distance <= 3) && lockedOn)
        {
            tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 5) ;
            attackTimeLength = normalTigerAttacklength;
            StartCoroutine(AttackDuration());
            tigerRB.AddForce(attackDirection * (attackForce + 14), ForceMode.Impulse);//Changed from 8 to 12
            animation.Play("Attack 1 & 2");
            playerAudio.PlayOneShot(tigerSwing, 0.05f);
        }
        //Want DistanceCloser only to play when the tiger isn't close enough. Was originally going to have a distance > 10 || distance <=3
        //above,but I realized that the below will cover it. Maybe, let's keep testing it out
        if ((distance < 10.5f && distance > 3) && lockedOn)
        {
            //I need some way to stop this
            //Maybe like the wolf, once the tiger reaches the necessary distance, just perform the regular attack
            //I could either have a non impulse movement to close the distance, or an impulse that will definitely get me close
            //enough to the target. I partially want to do the latter, but I think the former is better because the impulse is not consistent
            //Gonna need a method like DistanceCloser
            ///At first, I was wondering if the attack duration plays long enough for distance closer, but it looks like it does
            Debug.Log("Distance Closer");
            closeTheDistance = true;
            StartCoroutine(DistanceCloser());
            //tigerRB.AddForce(attackDirection * (attackForce + 16), ForceMode.Impulse);
        }
    }
    IEnumerator DistanceCloser()
    {

        yield return new WaitForSeconds(0.5f);
        if (closeTheDistance == true)
        {
            Debug.Log("Time reached");
            closeTheDistance = false;
            //I have some reservations about doing this because I need to trigger this in the above if I close the distance before
            //the time limit is reached. I may have to make another method for
            attackTimeLength = distanceCloserTigerAttackLength;
            StartCoroutine(AttackDuration());
            animation.Play("Attack 1 & 2");
            playerAudio.PlayOneShot(tigerSwing, 0.05f);
        }
    }

    //The Vector3 strikeArea is for making the hit effect play directly on the
    public void PlayTigerRegularStrike(Vector3 strikeArea)
    {
        playerAudio.PlayOneShot(tigerRegularStrike, 0.5f);
        regularHitEffect.transform.position = strikeArea;
        regularHitEffect.Play();
    }
    IEnumerator StrikeLag()
    {
        //Atm after all attacks. The player can't do anything, even walk or dodge
        lag = true;
        //Debug.Log("Attack Lag");
        yield return new WaitForSeconds(0.2f);
        lag = false;
    }
    public void TigerSpecial()
    {
        playerAudio.PlayOneShot(tigerSwing, 0.05f);
        tigerRB.AddForce(attackDirection * (attackForce + 14), ForceMode.Impulse); //+ 8 normally, but try + 12 for blade of
        attackRotation = Quaternion.LookRotation(target.transform.position - tiger.transform.position);
        tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 5); //Am using all the attack rotations
        //here because there is a charge up before Tiger Special Attack
        tigerRB.AddRelativeTorque(Vector3.down * 5, ForceMode.Impulse);
        animation.Play("Attack 1 & 2");

        
    }
    public void PlayTigerSpecialStrike(Vector3 strikeArea)
    {
        playerAudio.PlayOneShot(tigerSpecialStrike, 0.5f);
        specialHitEffect.transform.position = strikeArea;
        specialHitEffect.Play();
    }
    IEnumerator Dodge()
    {
        dodge = true;
        //dodgeEffect.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        if (birdActive == true)
        {
            bird.transform.Rotate(0, bird.transform.rotation.y, 39.5f);
        }
        dodge = false;
        //dodgeEffect.SetActive(false);
        StartCoroutine(DodgeLag());
    }
    IEnumerator DodgeLag()
    {
        lag = true;
        Debug.Log("Dodge Lag");
        yield return new WaitForSeconds(0.2f);
        lag = false;
    }
    IEnumerator SwoopLag()
    {
        swoopLag = true;
        yield return new WaitForSeconds(0.2f);
        swoopLag = false;
    }
    public void EnableLockOn()
    {
        canLockOn = true;
    }
    public void LockOn()
    {
        //Create target
        //Make Target appear on foe
        //target.SetActive(true);
        //newTarget.SetActive(true);
        targetedEnemy = GameObject.FindGameObjectWithTag("Enemy");
        //lockedOn = true;
        //Tret stays on foe in Update

        //I dont really like how the camera locks on
        //focalPoint.transform.rotation = Quaternion.LookRotation(targetedEnemy.transform.position);
        //focalPoint.transform.LookAt(targetedEnemy.transform.position); //This one is a lot better than the above
                                                                       //Because it doesn't turn to much in a direc
                                                                       //Put lock on code here
                                                                       //May want to put on a canLockOn bool on enemies instead and then put the target on the closest

        //New LockOn code, atm, track for any enemy that is within a 15 float radius and then lock onto them
        //New bool lockOnTurnedOn, so that if you already locked onto a foe, it will lock onto the closest foe
        //First, identify an enemy in the area. Because foes load/Instantiate only when you enter an area, I don't have to worry
        //About another area's foes being tracked

        //I think I should check if there even is a foe first, just to avoid error message
        //Maybe I'll just make a list, but I will test just for null for now.
        //test for targetedEnemy != null and then make a list
        if (targetedEnemy != null) //There's no immediate way to check if there's an object of tag something, I wished there
        {
            //I think I may need to rewrite this because targetedEnemy is not always going to be the same
            //AND it will be determined in this loop
            if (tigerActive == true)
            {
                //attackRange.transform.position = tiger.transform.position;
                //I think the method always goes (target, own position)
                distance = Vector3.Distance(targetedEnemy.transform.position, tiger.transform.position);
            }
            else if (birdActive == true)
            {
                //attackRange.transform.position = bird.transform.position;
            }
            if (lockOnTurnedOn == false)
            {
                //if (distance <= 25)
                //{
                    target.SetActive(true);
                    lockedOn = true;
                    lockOnTurnedOn = true;
                    //focalPoint.transform.LookAt(targetedEnemy.transform.position); //This one is a lot better than the above
                    //Because it doesn't turn to much in a direc
                    //Put lock on code here
                    //May want to put on a canLockOn bool on enemies instead and then put the target on the closest
                    //cam.transform.LookAt(targetedEnemy.transform.position);
                //}
                //else
                //{
                    //Debug.Log("Nothing in range");
                //}
            }
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
    }
    IEnumerator TransformCountdown()
    {
        transforming = true;
        //transformEffect.SetActive(true);
        Instantiate(transformEffect, tiger.transform.position, Quaternion.identity);
        //transformEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Transform();
        //transformEffect.gameObject.SetActive(false);
        //Destroy(transformEffect);
        transforming = false;
    }
    public void Transform()
    {
        //The problem was that for some reason E was triggering both if cases at the same time, so I made tiger transformation
        //into an else if
        //First is transforming into Tiger
        if (birdActive == true)
        {
            //tiger.transform.Translate(bird.transform.position.x - 6.6f, 0, bird.transform.position.z + 14.5f);
            tiger.transform.Translate(bird.transform.position.x - 14.18f, 0, bird.transform.position.z + 12.8f);
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
        }
        else if (tigerActive == true)
        {
            tiger.SetActive(false);
            //Instantiate(bird);
            //bird.transform.position = new Vector3(tiger.transform.position.x, 2.93f, tiger.transform.position.z);
            //bird.transform.Translate(tiger.transform.position.x - 6.6f, 0, tiger.transform.position.z + 14.5f);
            bird.transform.Translate(tiger.transform.position.x - 14.8f, 0, tiger.transform.position.z + 12.8f);
            //Destroy(tiger);
            bird.SetActive(true);
            //transform.position = new Vector3(bird.transform.position.x, transform.position.y, bird.transform.position.z);
            tigerActive = false;
            birdActive = true;
            //birdSensor.SetActive(true);
            playerMugshot.texture = birdMugshot;
        }
    }
    //Taking damage anima
    public void TigerFlinching()
    {
        //Debug.Log("Flinching from Monkey");
        //Wierdly, animation doesn't work here, needed to do it in up
        tigerFlinch = true;
        StartCoroutine(StunDuration());
    }
    public void TigerFlinching2()
    {
        //Debug.Log("Flinching from Monkey");
        //Wierdly, animation doesn't work here, needed to do it in up
        tigerFlinch2 = true;
        StartCoroutine(StunDuration());
    }
    public void BirdFlinching()
    {
        birdFlinch = true;
        StartCoroutine(StunDuration());
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
    IEnumerator StunDuration()
    {
        playerAudio.PlayOneShot(damaged, 0.1f);
        stunned = true;
        stunnedInvincibility = true;
        if (tigerFlinch == true)
        {
            animation.Play("Flinch 1");
        }
        if (tigerFlinch2 == true)
        {
            animation.Play("Flinch 2");
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

        //The display code works because of the camera. Because of the camera, the player character is always at the center of it, where
        //The text also is
        damageDisplay.text = "" + damageForDisplay;
        yield return new WaitForSeconds(1.4f);
        if (tigerFlinch == true)
        {
            tigerFlinch = false;
        }
        if (tigerFlinch2 == true)
        {
            tigerFlinch2 = false;
        }
        if (tigerKnockedBack == true)
        {
            tigerKnockedBack = false;

            //tigerRB.AddTorque(Vector3.right * 12, ForceMode.Impulse);
            tiger.transform.Rotate(-16.5f, -8f, -1.5f);
            //tigerRB.AddForce(Vector3.down * 5, ForceMode.Impulse);
            tiger.transform.Translate(0, -0.2f, 0);
            tigerRB.constraints = RigidbodyConstraints.FreezePositionY;
            tigerRB.constraints = RigidbodyConstraints.FreezeRotationZ;
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
        stunned = false;
        stunnedInvincibility = false;
        damageDisplay.text = "";
    }
    //public void DisplayHP(int currentHP)
    //{
        //HPText.text = "" + HP;
    //}
    public void LoseHP(int damage)
    {
        HP-= damage;
        damageForDisplay = damage;
        //HPBar.fillAmount = HPBar.fillAmount - damage / maxHPBarFill;
        //This was for showing the damage done from each attack on the Player, but isn't necessary because the player and that
        //text is always at the center of the screen
        //damageDisplay.transform.position = new Vector3(tiger.transform.position.x, tiger.transform.position.y+ 5, tiger.transform.position.z);
        if (HP <= 0)
        {
            //Destroy(gameObject);
            //gameObject.SetActive(false);
            //Time.timeScale = 0;
            Debug.Log("Game O");
            StartCoroutine(GameOverSlowDown());
        }
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
    public void OpeningRun()
    {
        running = true;
        tigerRB.AddForce(Vector3.forward * speed);
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
        Time.timeScale = 0;
        paused = true;
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1;
        paused = false;
    }
    IEnumerator GameOverSlowDown()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(0.8f);
        Time.timeScale = 0;
    }
}

