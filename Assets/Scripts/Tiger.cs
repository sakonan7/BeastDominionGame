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

}
