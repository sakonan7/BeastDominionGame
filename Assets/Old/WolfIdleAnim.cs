using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfIdleAnim : MonoBehaviour
{
    //private GameObject wolfFocalPoint;
    //private Rigidbody wolfRb;
    public float speed = 9;
   private GameObject player;
    public GameObject wolf;
    private Wolf wolfScript;
    public bool idle = true;

    private int leftWalk = 0;
    private int rightWalk = 1;
    private int walkRandomizer;
    private bool chooseDirection = true;

    private PlayerController playerScript;
    private GameObject tiger;
    private GameObject bird;

    // Start is called before the first frame update
    void Start()
    {
        //wolfFocalPoint = GameObject.Find("Wolf Focal Point");
        //olfRb = GetComponent<Rigidbody>();
        wolfScript = wolf.GetComponent<Wolf>();

        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        player = GameObject.Find("Player");
        tiger = playerScript.tiger;
        bird = playerScript.bird;
        transform.position = new Vector3(tiger.transform.position.x, 0, tiger.transform.position.z); //Have to use playerScript because tiger is on PlayerController, not
        //player, which is just an ob
    }

    // Update is called once per frame
    void Update()
    {
        if (chooseDirection == true)
        {
            WalkDirection(); //Also sets the focal point that the wolf will walk around
            //Got rid of that point setting from Idle because Idle will keep setting the focal point on the player character's posi
        }
        if (idle == true)
        {
            //Can't just use WalkDirection() because Update will keep playing it
            
            StartCoroutine(Idle());
        }
    }
    IEnumerator Idle()
    {
        //Circles the player for a few seconds before attacking
        //Use a randomizer between 1 and 0 to determine if Wolf will move left or right


        //Basically if walkRandomizer == 0 or 
        if (walkRandomizer == leftWalk)
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
        }
        else if (walkRandomizer == rightWalk)
        {
            transform.Rotate(Vector3.down * speed * Time.deltaTime);
        }
        
        yield return new WaitForSeconds(3);
        idle = false;
        wolfScript.ChaseOn();
    }

    public void ResetIdle()
    {
        idle = true;
        //chooseDirection = true;
        //Debug.Log(idle);
    }
    public void WalkDirection()
    {
        //Int random is not max inclusive
        walkRandomizer = Random.Range(leftWalk, rightWalk + 1);
        //if (playerScript.tigerActive == true)
        //{
        //transform.position = new Vector3(tiger.transform.position.x, 0, tiger.transform.position.z);
        //}
        //else if (playerScript.birdActive == true)
        //{
        //transform.position = new Vector3(bird.transform.position.x, 0, bird.transform.position.z);
        //}
        transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        chooseDirection = false;
    }
}
