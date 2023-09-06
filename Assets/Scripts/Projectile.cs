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
        if (moving == true)
        {
            transform.Translate(Vector3.back * 15 * Time.deltaTime);
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
