using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : MonoBehaviour
{
    private new Animation animation;
    private GameObject player;
    private PlayerController playerScript;
    private Quaternion lookRotation;
    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animation>();
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
        animation.Play("Eagle Fly");
    }

}
