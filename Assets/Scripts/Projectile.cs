using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerScript;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    public float attackForce = 6;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();       
    }
    public void SetAttackForce()
    {

    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * 15 * Time.deltaTime);
        //lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
