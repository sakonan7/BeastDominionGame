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
    public float damage;
    private float speed;
    private bool moving = true;
    private bool destroyable = true;
    private float lifeTime;
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
    public void SetDamage(float newDamage)
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
    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (destroyable == true && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground")))
        {
            Destroy(gameObject);
        }
    }
}
