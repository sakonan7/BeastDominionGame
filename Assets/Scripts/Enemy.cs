using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Stuff for like HP, whether the foe is locked on, and battle damage so that I don't have to keep
    //checking each foe and grabbing their component, like grabbing Wolf and then grabbing Wolf's Wolf script
    //Will be public, because stuff like HP will be modified in the inspecter
    //This would also be a good place to place attribute and attack eff
    public int HP;
    public bool lockedOn = false;
    private Rigidbody enemyRb;
    private PlayerController playerScript;
    private GameManager gameManager;
    public ParticleSystem dyingEffect;
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
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
            Debug.Log("Wolf Dies");
            gameManager.EnemyDefeated();
        }
    }
    public void OnTriggerEnter(Collider other)
    {

        //I will have to keep some of the damagedin each individual script because I need those individual scripts for damage
        //animations
        Debug.Log(HP + "Left");
        if (other.CompareTag("Tiger Attack Regular"))
        {
            //For now, just trigger stun. I will use both of their directions to perform the knockback
            //TakeDamage();

            HP -= 2;
            //Damaged();
            playerScript.PlayTigerRegularStrike(transform.position);
            //Vector3 knockbackDirection = (transform.position - tiger.transform.position).normalized;
            //knockback force is inconsistent. Sometimes it doesn't knockback at all. Sometimes it knocks back too much
            //It doesn't matter what the value is.
            //It may not matter because I will have the attack lag minimized
            //But I don't want the player to whiff attacks, so I think I will make sure the tiger is the right distance from the wolf
            //Unless I can make a force play until a certain distance is reached
            //I can't use forcemode.impulse then
            enemyRb.AddForce(playerScript.attackDirection * 15, ForceMode.Impulse);
            playerScript.AttackLandedTrue();
        }
        if (other.CompareTag("Tiger Special"))
        {
            //For now, just trigger stun. I will use both of their directions to perform the knockback
            //TakeDamage();

            HP -= 7;
            //Damaged();
            playerScript.PlayTigerSpecialStrike(transform.position);
            //Vector3 knockbackDirection = (transform.position - tiger.transform.position).normalized;
            //knockback force is inconsistent. Sometimes it doesn't knockback at all. Sometimes it knocks back too much
            //It doesn't matter what the value is.
            //It may not matter because I will have the attack lag minimized
            //But I don't want the player to whiff attacks, so I think I will make sure the tiger is the right distance from the wolf
            //Unless I can make a force play until a certain distance is reached
            //I can't use forcemode.impulse then
            enemyRb.AddForce(playerScript.attackDirection * 20, ForceMode.Impulse);
            //playerScript.AttackLandedTrue();
        }
    }
}
