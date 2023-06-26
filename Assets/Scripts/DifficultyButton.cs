using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    private GameManager gameManager;
    public string difficulty;
    private Button button;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    { 
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        button = GetComponent<Button>(); //Can do this because this script is linked directly to a button
        //rb = GetComponent<Rigidbody>();
        //I can do this for say player and playercontroller, but I don't because playercontroller script is directly on player
        //It is like accessing Rigidbody
        //button.onClick.AddListener(SetDifficulty);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //public void SetDifficulty()
    //{
        //gameManager.StartGame(difficulty);
    //}
}
