using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tiger : MonoBehaviour
{
    private PlayerController playerScript;
    public GameObject struckFoe;
    public Monkey monkeyScript;
    private NewWolf wolfScript;
    public bool noHealingItems = true;
    private float attackForce = 5; //Originally 5, 11 was too little and 20 is a little too much
    private float tigerSpecialForce = 7;
    public AudioClip tigerRegularStrike;
    private AudioSource tigerAttackAudio;
    public AudioClip tigerSpecialStrike;
    private Rigidbody tigerRb;

    public ParticleSystem regularHitEffect;

    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        tigerAttackAudio = GetComponent<AudioSource>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        tigerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnCollisionEnter(Collision collision)
    {

        //For some reason trigger doesn't work with meat, but it worked for attack range
        if (collision.gameObject.CompareTag("Heal"))
        {
            Destroy(collision.gameObject);
            playerScript.IncreaseHealingItems();
            if (noHealingItems == true)
            {
                noHealingItems = false;
                playerScript.DisplayNumberOfItems();
            }
        }
    }
    //Need this for when the foe is right next to the tig
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Start Game Boundary")
        {
            gameManager.StartGame();
            Destroy(other);
        }
        if (other.CompareTag("Wolf Attack") && (playerScript.dodge == false && playerScript.specialInvincibility == false && playerScript.stunnedInvincibility == false))
        {
            //wolfScript = other.gameObject.GetComponent<NewWolf>();
            //playerScript.LoseHP(wolfScript.damage); //I need to reference Wolf's attack damage based on difficulty, but that's not hard
            wolfScript = other.gameObject.GetComponentInParent<NewWolf>();
            //Was intially going to try putting this in the bigger if loop
            if (wolfScript.attackLanded == false)
            {
                playerScript.TigerFlinching();
                playerScript.LoseHP(wolfScript.damage);
                //This is going to be more challenging, because I need a specific Wolf's attack direc
                //I got it, draw a wolf script from the other.gameObject.
                //I hope this doesn't cause an issue when multiple attacks land on the player
                //Serendipity, I can use this to determine damage
                //
                wolfScript.SetAttackLanded();
                wolfScript.PlayAttackEffect();
                tigerRb.AddForce(wolfScript.followDirection * 15, ForceMode.Impulse);
                //tigerRb.AddForce(Vector3.back * 12, ForceMode.Impulse);
                //playerScript.AttackLandedTrue();
            }

        }
    }
}
