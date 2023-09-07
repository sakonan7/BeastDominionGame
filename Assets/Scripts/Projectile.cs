using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerScript;
    private Vector3 playerPosition;
    private GameObject tiger;
    private GameObject bird;
    private Rigidbody rb;
    private Vector3 followDirection;
    private Vector3 attackDirection;
    private Quaternion lookRotation;
    public float attackForce = 0;
    public int damage;
    private float speed;
    public bool comboFinisher = false;
    //Unfortunately, for objects created instantaneously, you have to set up these values in the inspector. Projectiles always move
    //if the projectilewas set to move before using the set. Itmeans the setter didn't work
    public bool moving = false;
    public bool destroyable = true;
    public float lifeTime;
    public bool leftAttack = false;
    public bool rightAttack = false;
    public bool backAttack = false;
    public bool isLingering = false;
    public bool playerDodged = false;
    public bool canHurtFlying = true;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        //For some reason I had to reversethe lookRotation or the arrow would have been inverted. It happened even when I inverted
        //the projectile's y-rotation
        //it's possible it's because you need to orient the object in the right direction. The 3D models don'tautomatically know what is
        //front and what is back, so youcan have your character run towards another object while facing away from
        if (playerScript.tigerActive == true)
        {
            tiger = GameObject.Find("Tiger");
            playerPosition = new Vector3(tiger.transform.position.x, tiger.transform.position.y + 0.1f, tiger.transform.position.z);
            lookRotation = Quaternion.LookRotation(transform.position - tiger.transform.position);
        }
        if (playerScript.birdActive == true)
        {
            bird = GameObject.Find("Bird");
            playerPosition = bird.transform.position;
            lookRotation = Quaternion.LookRotation(transform.position - bird.transform.position);
        }
        rb = GetComponent<Rigidbody>();
    }
    public void SetAttackForce()
    {

    }
    // Update is called once per frame
    void Update()
    {
        followDirection = (playerPosition - transform.position).normalized;
        if (moving == true)
        {
            rb.AddForce(followDirection * 2, ForceMode.Impulse);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
        }
        if (destroyable == false)
        {
            StartCoroutine(DestroyAfterTime());
        }
        //lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3);
    }
    //Setting up setters for speed will allow me todo stuff like doing Luxord's trick shots
    public void IsMoving(bool trueFalse)
    {
        moving = trueFalse;
    }
    public void IsDestroyable(bool trueFalse)
    {
        destroyable = trueFalse;
    }
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void SetLifeTime(float newLifeTime)
    {
        lifeTime = newLifeTime;
    }
    public void LeftKnockBack()
    {
        leftAttack = true;
    }
    public void RightKnockBack()
    {
        rightAttack = true;
    }
    public void BackKnockBack()
    {
        backAttack = true;
    }
    public void ResetKnockbacks()
    {
        leftAttack = false;
        rightAttack = false;
        backAttack = false;
    }
    public void SetLingering()
    {
        isLingering = !isLingering;
    }
    public void SetPlayerDodged()
    {
        playerDodged = true;
    }
    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
        Debug.Log("Time'sUp");
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (destroyable == true && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground")))
        {
            Destroy(gameObject);
        }
    }
}
