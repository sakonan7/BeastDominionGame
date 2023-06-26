using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    //Wolf code isn't working, so I'm making a new class where the Wolf walks differently and to test to see if Jump Attack will work now
    private new Animation animation;
    public GameObject focalPoint;
    public GameObject attackAura;
    private WolfIdleAnim idleAnim;
    private Rigidbody wolfRb;
    private Vector3 followDirection;
    private float speed = 31;
    private float jumpForce = 3; //Originally 8
    private float attackForce = 15; //Originally 10
    private float jumpAttackForce = 2;
    private bool chase = false;
    private bool attack = false; //Accidentally set this to true, causing Wolf to attack twice
    private bool cooldown = false;

    private GameObject player;
    private PlayerController playerScript;
    private Vector3 playerPosition;
    private GameObject tiger;
    private GameObject bird;
    private Rigidbody tigerRB;
    private Rigidbody birdRB;

    public bool isOnGround = true;
    // Start is called before the first frame update
    void Start()
    {
        idleAnim = focalPoint.GetComponent<WolfIdleAnim>();
        animation = GetComponent<Animation>();
        wolfRb = GetComponent<Rigidbody>();

        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        tiger = playerScript.tiger;
        tigerRB = tiger.GetComponent<Rigidbody>();
        bird = playerScript.bird;
        birdRB = bird.GetComponent<Rigidbody>();

        Physics.gravity *= 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (idleAnim.idle == true && isOnGround == true)
        {
            animation.Play("Walk 1");
        }
        //So actions are only performed on the ground
        else if (idleAnim.idle == false && chase == true && cooldown == false && isOnGround == true)
        {
            Run();
            //animation.Play("Jump_Attack");
            //StartCoroutine(GroundAttack());
        }
        if (cooldown == true)
        {
            animation.Play("Idle Wolf");
        }
        attackAura.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }
    public void ChaseOn()
    {
        chase = true;
    }
    public void Run()
    {
        if (playerScript.tigerActive == true)
        {
            playerPosition = tiger.transform.position;
        }
        else if (playerScript.birdActive == true)
        {
            playerPosition = bird.transform.position;
        }
        followDirection = (playerPosition - transform.position).normalized;
        
        wolfRb.AddForce(followDirection * speed);
        animation.Play("Run Wolf");
        //Debug.Log("Running");
        //chase = true;
        //yield return new WaitForSeconds(2f);
        //attack = true;
        //if (playerScript.tigerActive == true)
        //{
            //GroundAttack();
        //}
        
        //StartCoroutine(Attack());
    }
    //IEnumerator is needed to keep the attack bool active because GroundAttack() will end it right away
    //All methods acts in the same moment, so a method for making attack false will active right a


    IEnumerator GroundAttack()
    {
        attack = true;
        if (attack == true)
        {
            //For now, have the Wolf stop and then jump attack
            //wolfRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);
            
            animation.Play("Jump_Attack");
            Debug.Log("Jump Attack");
        }
        if (playerScript.dodge == false)
        {
            Debug.Log("Attack land");
            //wolfRb.AddForce((transform.position - playerPosition).normalized * 2, ForceMode.Impulse);
            //wolfRb.velocity = Vector3.zero;
            tigerRB.AddForce(followDirection * attackForce, ForceMode.Impulse);
            //tigerRB.constraints = RigidbodyConstraints.None;
            //tigerRB.AddTorque(Vector3.left * 4.5f, ForceMode.Impulse);
            //tigerRB.AddForce(Vector3.up, ForceMode.Impulse);
            attack = false;
            playerScript.TigerFlinching();
        }
        //May not do the same as I did for Monkey because Monkey's attacks specifically have very little knock
        yield return new WaitForSeconds(3f);
        attack = false;
        //idleAnim.ResetIdle();
        StartCoroutine(StartCoolDown());
    }
    //Temporary attack
    IEnumerator TemporaryGroundAttack()
    {
        attack = true;
        attackAura.SetActive(true);
        
        if (attack == true)
        {
            //For now, have the Wolf stop and then jump attack
            wolfRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);

            //animation.Play("Jump_Attack");
            Debug.Log("Jump Attack");
        }
        if (playerScript.dodge == false)
        {
            Debug.Log("Attack land");
            //wolfRb.AddForce((transform.position - playerPosition).normalized * 2, ForceMode.Impulse);
            //wolfRb.velocity = Vector3.zero;
            tigerRB.AddForce(followDirection * attackForce, ForceMode.Impulse);
            //tigerRB.constraints = RigidbodyConstraints.None;
            //tigerRB.AddTorque(Vector3.left * 4.5f, ForceMode.Impulse);
            //tigerRB.AddForce(Vector3.up, ForceMode.Impulse);
            attack = false;
            playerScript.TigerFlinching();
        }
        //May not do the same as I did for Monkey because Monkey's attacks specifically have very little knock
        yield return new WaitForSeconds(1f);
        attack = false;
        attackAura.SetActive(false);
        //idleAnim.ResetIdle();
        StartCoroutine(StartCoolDown());
    }
    //public void AirAttack()
    //{
    //attack = true;
    //animation.Play("Jump_Attack");
    //wolfRb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    //wolfRb.AddForce(followDirection * jumpForce, ForceMode.Impulse);
    //Debug.Log("Jump Attack");
    //}
    IEnumerator AirAttack()
    {
        attack = true;
        if (attack == true)
        {
            wolfRb.AddForce(Vector3.up * 4, ForceMode.Impulse);
            wolfRb.AddForce(followDirection * jumpAttackForce, ForceMode.Impulse);
            animation.Play("Jump_Attack");
            Debug.Log("Jump Attack");
            isOnGround = false;
        }
        if (playerScript.dodge == false)
        {
            Debug.Log("Attack land");
            birdRB.AddForce(followDirection * jumpAttackForce, ForceMode.Impulse);
            playerScript.BirdKnockBack();
        }
        yield return new WaitForSeconds(1.3f);
        attack = false;
        //idleAnim.ResetIdle();
        StartCoroutine(StartCoolDown()); //May need a FallDown IEnumerator before StartCool
    }
    IEnumerator StartCoolDown()
    {
        cooldown = true;
        Debug.Log("Cool down");
        if (playerScript.tigerActive == true)
        {
            yield return new WaitForSeconds(3);
        }
        else if (playerScript.birdActive == true)
        {
            //wolfRb.AddForce(Vector3.down * 2, ForceMode.Impulse);
            yield return new WaitForSeconds(8);
        }
        cooldown = false;
        idleAnim.ResetIdle();
    }
    public void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Player") && attack == true && playerScript.dodge == false)
        //{

        //}
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wolf Range") && attack == false && chase == true)
        {
            chase = false;
            //animation.Stop();
            //Different case if Tiger or Bird is act
            if (playerScript.tigerActive == true)
            {
                StartCoroutine(TemporaryGroundAttack());
            }
            else if (playerScript.birdActive == true)
            {
                StartCoroutine(AirAttack());
            }
            //Debug.Log("Chase bool = " + chase);
        }
        if (other.gameObject.CompareTag("Sensor"))
        {
            playerScript.EnableLockOn();
            //Debug.Log("Lock onto Wolf");
        }
    }
}
