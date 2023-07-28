using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : MonoBehaviour
{
    private new Animation animation;
    private GameObject player;
    private PlayerController playerScript;
    private Quaternion lookRotation;

    public GameObject HPBar;
    private GameObject cameraRef;
    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animation>();
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        cameraRef = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        HPBar.transform.position = new Vector3(transform.position.x, transform.position.y + 1.9f, transform.position.z + 0.1f);
        //HPBar.transform.LookAt(HPBar.transform.position - (cameraRef.transform.position - HPBar.transform.position));
        lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
        animation.Play("Eagle Fly");
    }

}
