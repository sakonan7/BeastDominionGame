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


    private new Animation animation;
    private new Animation birdAnimation;
    public bool tigerActive = true;
    public bool birdActive = false;

    public GameObject tiger;
    public GameObject bird;
    

    public bool specialInvincibility = false;
    public GameObject bladeOfLight;
    private bool charging = false;
    private Light staffLight;

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
    private float dodgeForce = 35; //90 barely jumps
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
    private float normalTigerAttacklength = 0.3f;
    private float distanceCloserTigerAttackLength = 0.1f;
    private Quaternion attackRotation;
    //private float distance;

    private Rigidbody playerRb;
    //private Rigidbody tigerRB;
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
    private Enemy enemyScript;
    private Vector3 enemyTargetPosition;
    private Rigidbody foeRB;
    //public bool canLockOn = false;
    public bool lockedOn = false;
    public GameObject focalPoint; //Turn camera towards targetted foe
    //New lock on code
    private float distance = 0;
    private bool lockOnTurnedOn = false;
    private bool noHealingItems = true;

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
    private int originalHP = 10;
    public RawImage playerMugshot;
    public Texture tigerMugshot;
    public Texture birdMugshot;
    public Image HPBar;
    float maxHPBarFill; //References the max amount of physical space the HP fills
    public TextMesh damageDisplay;
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
        //cam = cameraRef.transform;
        playerRb = GetComponent<Rigidbody>();
        
        animation = tiger.GetComponent<Animation>();
        

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
        maxHPBarFill = 1;//Changed this from HPBar.FillAmount because that is always going to equal 1

        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //ballRB = ball.GetComponent<Rigidbody>();
        staffLight = GameObject.Find("Staff").GetComponent<Light>();
        damageDisplay.color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && paused == false)
        {
            PauseGame();
            Debug.Log("Paused?");
        }
        else if (Input.GetKeyDown(KeyCode.F) && paused == true)
        {
            UnpauseGame();
            Debug.Log("Unpause? I think this problem started by switching to FixedUpdate");
        }
    }
    //I tried putting everything in regular Update(), but it makes everything a lot slower
    void FixedUpdate()
    {
        //Debug.Log(distance);

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
            //orientationObject.transform.position = new Vector3(tiger.transform.position.x, tiger.transform.position.y + 0.77f, tiger.transform.position.z + 0.82f);
            //orientationObject.transform.rotation = tiger.transform.rotation;
        }

        //movement
        forwardInput = Input.GetAxisRaw("Vertical");
        sideInput = Input.GetAxisRaw("Horizontal");
        //direction = new Vector3(forwardInput, 0, sideInput).normalized;



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
        if (lag == false && attack == false && dodge == false && stunned == false && transforming == false && closeTheDistance == false
            && charging == false && specialCloseTheDistance == false)
        {
            //I can still perform attack while running, so I will need to make sure I can't run while attacking. Just checked,
            //the same happens when I use a dodge
            //I already checked, I can't move while having an attack or dodge lag
            ///The current conditions I have put in haven't solved anything yet, so I will have to try something
            ///maybe try a running boolean
            ///this isn't a problem for attack, but it's a problem for dodging
            ///I think that the problem is that the animation keeps going, not that the action gets interrupt
            ///
            if (attack == false && dodge == false && gameManagerScript.startingCutscene == false)
            {
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
            if (Input.GetKeyDown(KeyCode.Space))
            {


                //Make an else case for the current code for when the player isn't pressing a direction
                //Placed animations here instead so that the full animation plays
                if (running == true)
                {
                    running = false;
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
                    }
                    else if (tigerActive == true)
                    {
                        //Tried tiger.transform.forward, but the transform of the tiger doesn't
                        playerRb.AddForce(moveDirection.normalized * dodgeForce, ForceMode.Impulse);
                        animation.Play("Jump Tweak");

                    }
                }
                else if (running == false)
                {
                    playerRb.AddForce(Vector3.fwd * dodgeForce, ForceMode.Impulse);
                    animation.Play("Jump Tweak");
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
                //Moved attackDirection here because the player object gets rotated now
                attackDirection = (target.transform.position - transform.position).normalized;

                attackRotation = Quaternion.LookRotation(target.transform.position - transform.position);

                //attackDirection isn't changing directions because it's using the empty Player object as the reference and the Player
                //object doesn't move
                if (birdActive == true)
                {
                    Swoop();
                }
                else if (tigerActive == true)
                {

                    Strike();
                }
                if (running == true)
                {
                    running = false;
                }
            }
            if (Input.GetMouseButtonDown(0) && lockedOn == false)
            {
                

                //Temporary fix atm because while the below is what I want, I can't fix the non moving attack at
                if (running == true)
                {
                    running = false;
                    attackDirection = moveDirection; //changed from Vector3.fwd. Changed back from moveDirection
                                                     //orientation.forward did not work out
                                                     //While moveDirection makes the attack
                                                     //moveDirection on its own will not move the player
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
                }
                if (running == false)
                {
                    attackDirection = Vector3.fwd;
                    Strike();
                }
            }
            //Special Attack
            //Changed it so that ChargeUp will determine what direction Tiger will go
            if (Input.GetKeyDown(KeyCode.Z))
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
        ///Keeping this here even though I moved the code from this to the IEnumerator to play the animation
        if (closeTheDistance == true && stunned == false)
        {
            animation.Play("Distance Closer");
                //I would prefer to use nonImpulse, but it is too slow and using Impulse is unexpectedly cool
                playerRb.AddForce(attackDirection * 10, ForceMode.Impulse); //attack force wasn't enough //Also, it isn't enough here //Try impulse
                //ForceMode Impulse is amazing. Needed to go from speed to 5 becaue of how fast and far it went
                transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 5);
            playerRb.velocity = attackDirection * 10;
            if (distance < 3 && closeTheDistance == true)
            {
                Debug.Log("Distance met For Regul");
                closeTheDistance = false;
                attackTimeLength = distanceCloserTigerAttackLength;
                StartCoroutine(AttackDuration());
                animation.Play("Attack 1 & 2");
                playerAudio.PlayOneShot(tigerSwing, 0.05f);
            }
            if (stunned == true)
            {
                closeTheDistance = false;
                Debug.Log("I don't want to do it, but, closeTheDistance = " + closeTheDistance);
            }

        }
        if (specialCloseTheDistance == true)
        {
            //For some reason I commented out the code for the distance closing lo
            animation.Play("Distance Closer");
            //I would prefer to use nonImpulse, but it is too slow and using Impulse is unexpectedly cool
            playerRb.AddForce(attackDirection * 5, ForceMode.Impulse); //attack force wasn't enough //Also, it isn't enough here //Try impulse
                                                                       //ForceMode Impulse is amazing. Needed to go from speed to 5 becaue of how fast and far it went
            transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 5);
            //Put the closeTheDistance code in here
            if (distance < 8)
            {
                Debug.Log("Distance Met At " + distance);
                //This will work because specialInvincibility is on and TigerSpecialDuration() cancels
                StartCoroutine(TigerSpecialDuration());
                TigerSpecial();
                specialCloseTheDistance = false;
            }
        }



        if (Input.GetKeyDown(KeyCode.Q))
        {

            TigerFlinching2();
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
            //Put this in a conditional and changed from enemyScript.HP > 0 so I don't get an error when an enemy is defeat
            if (targetedEnemy != null)
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
                distance = Vector3.Distance(targetedEnemy.transform.position, transform.position); //Didn't realize I'd have
                                                                                                   //Didn't realize I'd have to keep calculating Distance
                                                                                                   //Actually, I will recalculate distance in lockedOn
                                                                                                   //I need this here
                                                                                                   //to keep calculating the distance between foe
                                                                                                   //and player
            }
            //I may want to change this because I can trigger an error by trying to access enemyScript when targetEnemy has been killed
            if (enemyScript.HP <= 0)
            {
                lockedOn = false;
                Debug.Log("Can Lock On again"); //I think this should work because target gameObject is not part of
                                                //The enemy and is only sent to the enemies' location. So I think the main issue was the target disappears
                                                //because the enemy's position disappears
                                                //I thought the main problem was that target was getting destroyed
                                                //OOOps, I accidentally set the conditional to if targetEnemy != null
                                                //No wonder the LockOn method still
            }
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
    private void LateUpdate()
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
        yield return new WaitForSeconds(1.5f);
        charging = false;
        
        bladeOfLight.SetActive(true);
        staffLight.intensity = 2;

        playerAudio.PlayOneShot(bladeOfLightChargeUp, 0.2f);
        if (lockedOn == true)
        {
            attackDirection = (target.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 5);
            attackRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            specialCloseTheDistance = true;
        }
        else if (lockedOn == false)
        {
            attackDirection = Vector3.fwd;
            playerRb.AddForce(attackDirection * (attackForce + 154), ForceMode.Impulse); //+ 8 normally, but try + 12 for blade of
            TigerSpecial();
            StartCoroutine(TigerSpecialDuration());
        }
    }

    IEnumerator TigerSpecialDuration()
    {
        //attack = true;
        //specialInvincibility = true;
        //playerAudio.PlayOneShot(tigerSpecial, 0.2f);
        //bladeOfLight.SetActive(true);
        yield return new WaitForSeconds(2f);
        attack = false;
        specialInvincibility = false;
        bladeOfLight.SetActive(false);
        StartCoroutine(StrikeLag());
        staffLight.intensity = 0;
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
        //Debug.Log("Attacking from " + distance);


        //playerAudio.PlayOneShot(tigerRoar, 0.2f);
        //Why did I think of lockedOn == false, I think I thought of it in the case you aren't locked
        //I think I will create a case where lockedOn == false and for now, I want to make the distance smaller for
        //Debug.Log(playerRb.velocity);
        if (lockedOn == false)
        {
            attackTimeLength = normalTigerAttacklength;
            StartCoroutine(AttackDuration());
            //playerRb.AddForce(attackDirection * (attackForce + 10), ForceMode.Impulse);//Changed from 8 to 12
            playerRb.velocity = attackDirection * (attackForce + 10);
            animation.Play("Attack 1 & 2");
            playerAudio.PlayOneShot(tigerSwing, 0.05f);
        }
        //I guestimated from gameplay that the distance needs to be at least 15
        //I think I may want to rewrite this because I don't think this works
        if (lockedOn == true)
        {
            Debug.Log(distance);
        }
        if (distance > 15 && lockedOn)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 10) ;
            attackTimeLength = normalTigerAttacklength;
            StartCoroutine(AttackDuration());
            //playerRb.AddForce(attackDirection * (attackForce + 14), ForceMode.Impulse);//Changed from 8 to 12
            playerRb.velocity = attackDirection * (attackForce + 14);
            animation.Play("Attack 1 & 2");
            playerAudio.PlayOneShot(tigerSwing, 0.05f);
            Debug.Log("Non Distance Closer");
        }
        //Want DistanceCloser only to play when the tiger isn't close enough. Was originally going to have a distance > 10 || distance <=3
        //above,but I realized that the below will cover it. Maybe, let's keep testing it out
        else if ((distance < 15 && distance > 4) && lockedOn)
        {
            //I need some way to stop this
            //Maybe like the wolf, once the tiger reaches the necessary distance, just perform the regular attack
            //I could either have a non impulse movement to close the distance, or an impulse that will definitely get me close
            //enough to the target. I partially want to do the latter, but I think the former is better because the impulse is not consistent
            //Gonna need a method like DistanceCloser
            ///At first, I was wondering if the attack duration plays long enough for distance closer, but it looks like it does
            //Debug.Log("Distance Closer");
            transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 10);
            closeTheDistance = true;
            //StartCoroutine(DistanceCloser());
            //tigerRB.AddForce(attackDirection * (attackForce + 16), ForceMode.Impulse);
        }
        else if (distance < 4 && lockedOn)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 10);
            attackTimeLength = normalTigerAttacklength;
            StartCoroutine(AttackDuration());
            playerRb.AddForce(attackDirection * (attackForce + 14), ForceMode.Impulse);//Changed from 8 to 12
            animation.Play("Attack 1 & 2");
            playerAudio.PlayOneShot(tigerSwing, 0.05f);
            Debug.Log("Short Ranged Att");
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
        animation.Play("Attack 1 & 2");
        playerAudio.PlayOneShot(tigerSwing, 0.05f);
        
        attackRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, 10); //Am using all the attack rotations
        //here because there is a charge up before Tiger Special Attack
        playerRb.AddRelativeTorque(Vector3.down * 5, ForceMode.Impulse);
        
        //TigerSpecialSecondStrike();
        //StartCoroutine(UseTigerSpecialSecond());
        
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
        //tigerRB.AddForce(attackDirection * (attackForce + 14), ForceMode.Impulse); //+ 8 normally, but try + 12 for blade of
        //attackRotation = Quaternion.LookRotation(target.transform.position - tiger.transform.position);
        //tiger.transform.rotation = Quaternion.Slerp(tiger.transform.rotation, attackRotation, 5); //Am using all the attack rotations
        //here because there is a charge up before Tiger Special Attack
        playerRb.AddRelativeTorque(Vector3.down * 5, ForceMode.Impulse);
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

        if (targetedEnemy != null) //There's no immediate way to check if there's an object of tag something, I wished there
        {
            GameObject [] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float [] distanceList = new float[enemies.Length]; //Changed this from list to array
            float newMin = 0;
            //int newMinIndex = 0;
            bool smallestDistanceFound = false;
            int j = 0;

            //Doing this in case there's already a foe that's been locked
            //if (lockedOn == true)
            //{
                //enemyScript.LockOff();
            //}
            //I think I may need to rewrite this because targetedEnemy is not always going to be the same
            //AND it will be determined in this loop
                //attackRange.transform.position = tiger.transform.position;
                //I think the method always goes (target, own position)
                //distance = Vector3.Distance(targetedEnemy.transform.position, tiger.transform.position);
                //Create a loop that keeps making a new minimum
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
                                                   if (lockedOn == true)
                                                   {
                                                   enemyScript.LockOff();
                                                   }
                    enemyScript = targetedEnemy.GetComponent<Enemy>();
                    //if (enemyScript.lockedOn == false)
                    //{
                        enemyScript.LockOn();
                    //}
                        
                    }
                    j++;
                }
                //if (targetedEnemy == null)
                //{
                //Debug.Log("Targeted Enemy is null");
                //}
                target.SetActive(true);
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
    }
    IEnumerator TransformCountdown()
    {
        transforming = true;
        //transformEffect.SetActive(true);
        Instantiate(transformEffect, transform.position, Quaternion.identity);
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
    //Turn this into StunInvincibilityDuration() because that is the main purpose of
    IEnumerator StunDuration()
    {
        
        stunned = true;
        stunnedInvincibility = true;



        //The display code works because of the camera. Because of the camera, the player character is always at the center of it, where
        //The text also is

        yield return new WaitForSeconds(1.4f);
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
        stunned = false;
        stunnedInvincibility = false;
        
    }
    //public void DisplayHP(int currentHP)
    //{
        //HPText.text = "" + HP;
    //}
    public void LoseHP(int damage, int stunType)
    {

        //float damageDone = damage / originalHP / maxHPBarFill;

        //HPBar.fillAmount = HPBar.fillAmount - damageDone;
        HPBar.fillAmount -= 1 - ((maxHPBarFill / originalHP) * (HP - damage));
        //HPBar.fillAmount -= damage / maxHPBarFill;
        HP -= damage;
        damageForDisplay = damage;
        damageDisplay.gameObject.SetActive(true);

        damageDisplay.text = "" + damageForDisplay;
        Debug.Log(HPBar.fillAmount);
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
    public void OpeningRun()
    {
        running = true;
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
    public void OnCollisionEnter(Collision collision)
    {

        //For some reason trigger doesn't work with meat, but it worked for attack range
        if (collision.gameObject.CompareTag("Heal"))
        {
            Destroy(collision.gameObject);
            IncreaseHealingItems();
            if (noHealingItems == true)
            {
                noHealingItems = false;
                DisplayNumberOfItems();
            }
        }
        //May have to call anything that stops the player Wall
        //May need to put this in PlayerController
        //Worked so far because last time I did tigerSpecial near a wall, it caused Tiger to be rotated off the x-ax
        if (collision.gameObject.CompareTag("Wall") && (attack == true || specialInvincibility == true))
        {
            playerRb.velocity = Vector3.zero;
        }
    }
        public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Start Game Boundary")
        {
            gameManagerScript.StartGame();
            Destroy(other);
            //Debug.Log("Game Start");
        }

        //Play attack effect in Enemy and load the effect in the individual script. IE, if Xemnas is using his ethereal blades,
        //load the ethereal blade effect into the private variable in Enemy. If Xemnas is using his spark orbs, load the spark orbs
        //effect into the private variable in Enem
        if (other.CompareTag("Enemy Attack Range") && (dodge == false && specialInvincibility == false && stunnedInvincibility == false))
        {
            Enemy enemyScript = other.gameObject.GetComponentInParent<Enemy>();

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
            playerRb.AddForce(enemyScript.attackDirection * enemyScript.attackForce, ForceMode.Impulse);
            enemyScript.AttackLanded(0);
            //playerRb.AddForce(Vector3.back * 12, ForceMode.Impulse); //I don't know why I have this
            //playerScript.AttackLandedTrue();
            //}
            LoseHP(enemyScript.damage, enemyScript.hitNumber);
            StartCoroutine(DamageDisplayed());
            //enemyScript.PlayAttackEffect();
            if (enemyScript.comboAttack == true && enemyScript.comboFinisher == true)
            {
                StartCoroutine(StunDuration());
            }
        }
    }
}

