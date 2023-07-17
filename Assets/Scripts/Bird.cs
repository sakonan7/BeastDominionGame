using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private PlayerController playerScript;
    //Need to put foe objects here because I need to stun and damage them
    public GameObject struckFoe;
    public Monkey monkeyScript;

    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter(Collision collision)
    {
        //This code affects every object that has the tag ene
        if (collision.gameObject.CompareTag("Enemy") && playerScript.attack == true)
        {

            
            //Going to rewrite the code slightly to take into account hitting nonTargeted
            //foeRB = targetedEnemy.GetComponent<Rigidbody>();
            //attackDirection = (target.transform.position - transform.position).normalized;
            //attackDirection = (target.transform.position - bird.transform.position).normalized;
            //collision.gameObject.GetComponent<Rigidbody>().AddForce(playerScript.attackDirection * playerScript.attackForce, ForceMode.Impulse);
            //trying to make struck foes rise up slightly from a strike
            collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 100, ForceMode.Impulse);
            //Debug.Log("attack land");
            //Destroy(collision.gameObject);

            //I think this will work out because this script was intended for hitting one foe, although it applies to any collision
            //That happens when you're attacking
            //This code is for applying damage
            //If this doesn't work, I may need to manually name each say Monkey a different name like Monkey 1 and 2, possibly through code
            //I can do this because each area has arranged enemies. The enemies in the group are not randomized. IE, there will always
            //be 3 archers in say Area3
            if (collision.gameObject.name == "Monkey(Clone)")
            {
                //What I'm hoping is that not all foes of name Monkey are hit
                struckFoe = GameObject.Find("Monkey(Clone)");
                monkeyScript = struckFoe.GetComponent<Monkey>();
                monkeyScript.TakeDamage();
                monkeyScript.Stunned();
                Debug.Log(monkeyScript.HP);
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Start Game Collider")
        {
            gameManager.StartGame();
            Destroy(other);
        }
    }
}
