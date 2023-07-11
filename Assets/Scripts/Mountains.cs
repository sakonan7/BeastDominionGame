using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mountains : MonoBehaviour
{
    private GameObject tiger;
    private GameObject bird;
    private PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        tiger = GameObject.Find("Tiger");
        bird = GameObject.Find("Bird");
        player = GameObject.Find("Player"). GetComponent<PlayerController>();
        //transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.tigerActive == true)
        {
            //The Z is based on  The tiger's and player's z position
            //The x is to keep the Mountain object as close to 0 for x as possible
            transform.position = new Vector3(8.6f, 0, tiger.transform.position.z + 26.45f + 0.5f);
        }
    }
}
