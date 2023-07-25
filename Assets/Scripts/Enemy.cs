using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int HP;
    public int damage = 0;
    public ParticleSystem [] attackEffect = new ParticleSystem [3];
    public Vector3 attackDirection;
    public float attackForce;
    public int hitNumber = 0;
    public bool comboAttack = false;
    public bool comboFinisher = false;
    private AudioSource enemyAudio;
    public AudioClip[] enemySounds = new AudioClip[3];

    public bool lockedOn = false;
    private Rigidbody enemyRb;
    private PlayerController playerScript;
    private GameObject player;
    private Rigidbody playerRb;
    private GameManager gameManager;
    public ParticleSystem dyingEffect;
    private bool attacked = false;
    private bool hitAgainstWall = true;
    public bool hitLanded = false;
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(HP);
        //Was originally an else if case, but moved it into the above because the wolf doesn't die the minute its HP falls under 0
        //Was originally going to place this under each conditional, but it looks like this conditional works on its own
        if (HP <= 0)
        {
            dyingEffect.Play();
            Destroy(gameObject);
            //playerScript.LockOff(); I didn't realize this was here. And it was being used the whole time
            //Debug.Log("Wolf Dies");
            gameManager.EnemyDefeated();
        }
    }
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
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
        PlayAttackEffect(whichEffect);
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
    IEnumerator FoeAttacked()
    {
        attacked = true;
        yield return new WaitForSeconds(0.5f);
        attacked = false;
    }
    IEnumerator HitWall()
    {
        hitAgainstWall = true;
        yield return new WaitForSeconds(0.5f);
        hitAgainstWall = false;
    }
    public void OnTriggerEnter(Collider other)
    {

        //I will have to keep some of the damagedin each individual script because I need those individual scripts for damage
        //animations
        //Debug.Log(HP + "Left");
        if (other.CompareTag("Tiger Attack Regular"))
        {
            //For now, just trigger stun. I will use both of their directions to perform the knockback
            //TakeDamage();
            
            HP -= 0;
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
            enemyRb.AddForce(playerScript.attackDirection * 160, ForceMode.Impulse);
            //enemyRb.velocity = playerScript.attackDirection * 160;
            //enemyRb.velocity = new Vector3(playerScript.attackDirection.x * 160, 0, playerScript.attackDirection.z * 160);

            Vector3 consistentVel = new Vector3(enemyRb.velocity.x, 0, enemyRb.velocity.z);


            if (consistentVel.magnitude > 160)
            {
                Vector3 limitedVel = consistentVel.normalized * 160;
                enemyRb.velocity = new Vector3(limitedVel.x, 0, limitedVel.z);
            }
            float distance = Vector3.Distance(player.transform.position, transform.position);
            playerScript.AttackLandedTrue();
            Debug.Log(distance + " " + enemyRb.velocity);
            StartCoroutine(FoeAttacked());
        }
        if (other.CompareTag("Tiger Special"))
        {
            //For now, just trigger stun. I will use both of their directions to perform the knockback
            //TakeDamage();

            HP -= 0;
            //Damaged();
            playerScript.PlayTigerSpecialStrike(transform.position);
            //Vector3 knockbackDirection = (transform.position - tiger.transform.position).normalized;
            //knockback force is inconsistent. Sometimes it doesn't knockback at all. Sometimes it knocks back too much
            //It doesn't matter what the value is.
            //It may not matter because I will have the attack lag minimized
            //But I don't want the player to whiff attacks, so I think I will make sure the tiger is the right distance from the wolf
            //Unless I can make a force play until a certain distance is reached
            //I can't use forcemode.impulse then
            enemyRb.AddForce(playerScript.attackDirection * 280, ForceMode.Impulse);
            //playerScript.AttackLandedTrue();
        }
    }
}
