using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //Stuff for like HP, whether the foe is locked on, and battle damage so that I don't have to keep
    //checking each foe and grabbing their component, like grabbing Wolf and then grabbing Wolf's Wolf script
    //Will be public, because stuff like HP will be modified in the inspecter
    //This would also be a good place to place attribute and attack eff
    //I could link animations here by linking to animator and using the same bools such as damage for all foes
    //For a game with a lot of enemies, keep calling on Enemy script and set up things like hit damage and knockback values
    //Unless I'm making a complex game like Kingdom Hearts where each attack has a different attribute and different damage val
    //I could also put attack ranges in here too, so that I don't need to keep calling individual enemy scripts.
    //I thought of complex attacks like Kingdom Hearts, and I could just modify the size of the collider in the individual enemy script
    //Could also feed the damage of the enemy script in here
    //Could do XemnasHelicopterSlash(), enemyScript.damage = 10; XemnasSparkOrbs(), enemyScript.damage = 5, enemyScript.attribute = thunder

    //I need to set Vector3 attackDirection here, because I need it for playerControll
    private int HP;
    private float originalHP;
    public GameObject HPBarHolder;
    public Image HPBar;
    public GameObject targetReticule;
    private float maxHPBarFill;
    private GameObject camera;
    public int damage = 0;
    public ParticleSystem [] attackEffect = new ParticleSystem [3];
    //public GameObject HPBar;
    //private EnemyHPBar HPBarScript;
    public TextMesh damageDisplay;
    public Vector3 attackDirection;
    public float attackForce = 0;
    public int hitNumber = 0;
    public bool comboAttack = false;
    public bool comboFinisher = false;
    private AudioSource enemyAudio;
    public AudioClip[] enemySounds = new AudioClip[3];
    public bool leftAttack = false;
    public bool rightAttack = false;
    public bool backAttack = false;

    public bool lockedOn = false;
    private GameObject target;
    private Rigidbody enemyRb;
    private PlayerController playerScript;
    private GameObject player;
    private Rigidbody playerRb;
    private GameManager gameManager;
    public ParticleSystem dyingEffect;
    private bool attacked = false;
    private bool hitAgainstWall = true;
    public bool hitLanded = false;
    private bool hitByBirdSpecial = false;
    private bool cantBeHit = false;
    public bool isBird = false;
    public bool isFlying = false;
    public bool giantEnemy = false;
    public bool giantBoss = false;
    public bool revengeValue = false;
    public int currentRevengeValue = 0;
    public int revengeValueCount = 3;
    public bool isLingering = false;

    public bool playerDodged = false;
    //private ConstantForce enemyForce;
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyAudio = GetComponent<AudioSource>();
        //HPBar.SetActive(false);
        
        //HPBarScript = HPBar.GetComponent<EnemyHPBar>();
        //enemyForce = GetComponent<ConstantForce>();
        //enemyHPBarPosition = GameObject.Find("Enemy HP Bar");
        maxHPBarFill = 1;
        camera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(HP);
        //Was originally an else if case, but moved it into the above because the wolf doesn't die the minute its HP falls under 0
        //Was originally going to place this under each conditional, but it looks like this conditional works on its own
        if (HP <= 0)
        {
            //dyingEffect.Play();
            
            
            //playerScript.LockOff(); I didn't realize this was here. And it was being used the whole time
            //Debug.Log("Wolf Dies");
            gameManager.EnemyDefeated(transform.position);
            lockedOn = false;
            //Destroy(gameObject);
            gameObject.SetActive(false); //I'm doing this instead because I think destroying the object causes problems by making
            //me unable to access the object's transform.posi
        }
        if (lockedOn == true && HP > 0)
        {
            //Debug.Log("HP Bar Out");
            //I guess I guess I need to do this code in here. I guess it's like the code with Target.
            target = GameObject.Find("Target");
            targetReticule.SetActive(true);
            gameObject.tag = "Targeted Enemy";
            HPBarHolder.SetActive(true);
            //HPBar.transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 2.5f, target.transform.position.z);
            //HPBar.transform.position = new Vector3(0, 100, 0);
        }
        else if (lockedOn == false)
        { 
            HPBarHolder.SetActive(false);
            targetReticule.SetActive(false);
            gameObject.tag = "Enemy";
        }
        HPBarHolder.transform.rotation = camera.transform.rotation;
        targetReticule.transform.rotation = camera.transform.rotation;
    }
    public void SetHP(int newHP)
    {
        HP = newHP;
        originalHP = HP;
    }
    public int GetHP()
    {
        return HP;
    }
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
    public void SetFlying()
    {
        isFlying = !isFlying;
    }
    public void IsGiantEnemy()
    {
        giantEnemy = true;
    }
    //Is importantbecause giant bosses don't have a rigidbod
    public void IsGiantBoss()
    {
        giantBoss = true;
    }
    //public void SetAttackEffect(ParticleSystem newEffect)
    //{
        //attackEffect = newEffect;
    //}
    public void SetAttackDirection(Vector3 newDirection)
    {
        attackDirection = newDirection;
    }
    public void SetForce(float newForce)
    {
        attackForce = newForce;
    }
    public void AttackLanded(int whichEffect)
    {
        hitLanded = true; //I could always do hitLanded = !hitLanded, but that would make it potentially confusing if
        //I use a long combo
        hitNumber++;
        //PlayAttackEffect(whichEffect);
        Debug.Log("Hit Landed is " + hitLanded);
    }
    public void ResetHitLanded()
    {
        hitLanded = false;
    }
    public void ResetHitNumber()
    {
        hitNumber = 0;
    }
    public void SetComboAttack()
    {
        comboAttack = !comboAttack;
    }
    public void SetComboFinisher()
    {
        comboFinisher = !comboFinisher;
    }
    public void SetCantBeHit()
    {
        cantBeHit = !cantBeHit;
    }
    //I really want to feed everything into here for simplicity's sake
    //I could alternatively just use Enemy to play the effect from Monkey
    //Actually, this will be hardand complicated, because I'd have to call Monkey then
    //I think what I was thinking is, that if Monkey uses hitLanded, then just activate effect and sound effect
    //Something tells me it would be less complicated to do this all in the individual enemy script. Like putting Xemnas's
    //ethereal blade slash effects and spark bomb effects in Xemnas' invididual enemy
    //It's complicated because it requires a lot of feed

        //If I do play effects and sounds from the individual scripts, use hitLanded to do this.
    public void PlayAttackEffect(int whichEffect)
    {
        attackEffect[whichEffect].Play();
        Debug.Log("Attack Effect");
        enemyAudio.PlayOneShot(enemySounds[whichEffect], 0.1f);
    }
    public void LockOn()
    {
        lockedOn = true;
        //Debug.Log("Enemy is Locked On");
        //HPBar.SetActive(true);
    }
    public void LockOff()
    {
        lockedOn = false;
        //Debug.Log("Enemy is Locked Off");
    }
    public void LeftKnockBack()
    {
        leftAttack = true;
    }
    public void RightKnockBack()
    {
        rightAttack = true;
    }
    public void BackKnockBack()
    {
        backAttack = true;
    }
    public void ResetKnockbacks()
    {
        leftAttack = false;
        rightAttack = false;
        backAttack = false;
    }
    public void SetLingering()
    {
        isLingering = !isLingering;
    }
    public void SetPlayerDodged()
    {
        playerDodged = true;
    }
    public void UnsetPlayerDodged()
    {
        playerDodged = false;
    }
    public void HasRevengeValue()
    {
        revengeValue = true;
    }
    public void ResetRevengeValue()
    {
        revengeValueCount = 0;
    }
    public void RevengeValueUp()
    {
        currentRevengeValue++;
    }
    public void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.CompareTag("Wall") && attacked == true)
        {
            enemyRb.velocity = Vector3.zero;
            //Debug.Log("Code Worked!");
            StartCoroutine(HitWall());
        }
       //else if (collision.gameObject.CompareTag("Player") && hitAgainstWall == true)
        //{
            //playerRb.AddForce((transform.position - player.transform.position).normalized * 30, ForceMode.Impulse);
            //playerRb.velocity = (transform.position - player.transform.position).normalized * 30;
            //Debug.Log("Player Pushed Back");
        //}
    }
    IEnumerator FoeAttacked(float enterAttackForce)
    {
        attacked = true;
        if (giantEnemy == false)
        {
            float playerAttackForce = enterAttackForce;
            //enemyRb.AddForce(playerScript.attackDirection * attackForce, ForceMode.Force);
            enemyRb.velocity = new Vector3(playerScript.attackDirection.x * playerAttackForce, 0, playerScript.attackDirection.z * playerAttackForce);
        }


        yield return new WaitForSeconds(0.5f);
        attacked = false;
    }
    //This will be helpful for combo att
    IEnumerator SecondHit()
    {
        attacked = true;
if (giantEnemy == false)
        {
                    float playerAttackForce = 280;
        //enemyRb.AddForce(playerScript.attackDirection * attackForce, ForceMode.Force);
        enemyRb.velocity = new Vector3(playerScript.attackDirection.x * playerAttackForce, 0, playerScript.attackDirection.z * attackForce);
        }

        
        yield return new WaitForSeconds(1f);
        attacked = false;
        HP -= 6;
        //HPBarScript.HPDecrease(4, originalHP);
        //damageDisplay.gameObject.SetActive(false);
        //float distance = Vector3.Distance(player.transform.position, transform.position);
        //Debug.Log("Distance is equal to " + distance);
        //enemyForce.relativeForce = new Vector3(0, 0, 0);
        gameManager.HitByTigerSpecial(transform.position);
        StartCoroutine(DamageDisplayDuration(6));
        playerScript.PlayTigerSpecialStrike(transform.position);

        HPBarDecrease(6);
    }
    public void HPBarDecrease(int newDamage)
    {
        HPBar.fillAmount = (maxHPBarFill / originalHP) * (HP - newDamage);
    }
    //I may want to do all damage display on 
    IEnumerator DamageDisplayDuration(int damage)
    {
        damageDisplay.gameObject.SetActive(true);
        //Above HPBar instead of Target because non targeted enemies canget hit too
        damageDisplay.transform.position = new Vector3(HPBar.transform.position.x, HPBar.transform.position.y + 1f, HPBar.transform.position.z);
        damageDisplay.text = "" + damage;
        yield return new WaitForSeconds(0.5f);
        damageDisplay.gameObject.SetActive(false);
    }
    IEnumerator HitWall()
    {
        hitAgainstWall = true;
        yield return new WaitForSeconds(0.5f);
        hitAgainstWall = false;
    }
    IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(3);
        hitByBirdSpecial = false;
    }
    public void OnTriggerEnter(Collider other)
    {

        //I will have to keep some of the damagedin each individual script because I need those individual scripts for damage
        //animations
        //Debug.Log(HP + "Left");
        if (other.CompareTag("Tiger Attack Regular"))
        {

            if (playerScript.attackLanded == false)
            {
                //For now, just trigger stun. I will use both of their directions to perform the knockback
                //TakeDamage();
                //Debug.Log(HP + " left");
                HP -= 2;
                //HPBarScript.HPDecrease(2, originalHP);
                //Damaged();
                playerScript.PlayTigerRegularStrike(transform.position);
                //Vector3 knockbackDirection = (transform.position - tiger.transform.position).normalized;
                //knockback force is inconsistent. Sometimes it doesn't knockback at all. Sometimes it knocks back too much
                //It doesn't matter what the value is.
                //It may not matter because I will have the attack lag minimized
                //But I don't want the player to whiff attacks, so I think I will make sure the tiger is the right distance from the wolf
                //Unless I can make a force play until a certain distance is reached
                //I can't use forcemode.impulse then
                //Wow, I had to upgrade from 15 to 200 just to push foe back at most a few meters
                //enemyRb.AddForce(playerScript.attackDirection * 160, ForceMode.Impulse);
                //enemyRb.velocity = playerScript.attackDirection * 160;
                //enemyRb.velocity = new Vector3(playerScript.attackDirection.x * 160, 0, playerScript.attackDirection.z * 160);

                //Vector3 consistentVel = new Vector3(enemyRb.velocity.x, 0, enemyRb.velocity.z);


                //if (consistentVel.magnitude > 160)
                //{
                //Vector3 limitedVel = consistentVel.normalized * 160;
                //enemyRb.velocity = new Vector3(limitedVel.x, 0, limitedVel.z);
                //}

                playerScript.AttackLandedTrue();
                //Debug.Log(distance + " " + enemyRb.velocity);
                //StartCoroutine(FoeAttacked(120));
                //This code is for airborne enemies, but actually, it is for enemies that use animation instead of animator
                //because animtor makes characters a lot slower
                if (isBird == true)
                {
                    StartCoroutine(FoeAttacked(10));
                }
                if (isBird == false && isFlying == false)
                {
                    StartCoroutine(FoeAttacked(120));
                }
                StartCoroutine(DamageDisplayDuration(2));
                HPBarDecrease(2);
            }
        }
        if (other.CompareTag("Bird Attack Range"))
        {
            Debug.Log("Hit By Bird");
            if (playerScript.attackLanded == false)
            {
                HP -= 1;
                //HPBarScript.HPDecrease(1, originalHP);
                //Damaged();
                playerScript.PlayBirdRegularStrike(transform.position);


                playerScript.AttackLandedTrue();
                //Debug.Log(distance + " " + enemyRb.velocity);
                //May want barely any pushbackfrom bird attacks for the islandportion
                if (isBird == true)
                {
                    StartCoroutine(FoeAttacked(15));
                }
                if (isFlying == false)
                {
                    StartCoroutine(FoeAttacked(50));
                }
                StartCoroutine(DamageDisplayDuration(1));
                HPBarDecrease(1);
            }
        }
        if (other.CompareTag("Tiger Special"))
        {
            //For now, just trigger stun. I will use both of their directions to perform the knockback
            //TakeDamage();

            HP -= 4;
            //HPBarScript.HPDecrease(3, originalHP);
            //Damaged();
            playerScript.PlayTigerSpecialStrike(transform.position);
            gameManager.HitByTigerSpecial(transform.position);
            playerScript.AttackLandedTrue();
            //Vector3 knockbackDirection = (transform.position - tiger.transform.position).normalized;
            //knockback force is inconsistent. Sometimes it doesn't knockback at all. Sometimes it knocks back too much
            //It doesn't matter what the value is.
            //It may not matter because I will have the attack lag minimized
            //But I don't want the player to whiff attacks, so I think I will make sure the tiger is the right distance from the wolf
            //Unless I can make a force play until a certain distance is reached
            //I can't use forcemode.impulse then
            //enemyRb.AddForce(playerScript.attackDirection * 280, ForceMode.Impulse);
            //playerScript.AttackLandedTrue();
            //StartCoroutine(FoeAttacked());
            StartCoroutine(DamageDisplayDuration(4));
            HPBarDecrease(4);
            if (HP > 0)
            {

                StartCoroutine(SecondHit());
                //Debug.Log("Second Hit");
            }
        }
        if (other.CompareTag("Bird Special") && hitByBirdSpecial == false)
        {
            HP -= 5;
            playerScript.AttackLandedTrue();
            StartCoroutine(DamageDisplayDuration(5));
            HPBarDecrease(5);
            if (isBird == true)
            {
                StartCoroutine(FoeAttacked(50));
            }
            else
            {
                StartCoroutine(FoeAttacked(200));
            }
            hitByBirdSpecial = true;
            StartCoroutine(Invincibility());
            //Put a coroutine here so that enemies can only be hit by Bird Special once
        }
    }
}
