using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    //private Vector2 turn;
    public GameObject player;
    //private Vector3 offset = new Vector3(-0.10f, 0.02f, -1.9f);
    //private Vector3 offset = new Vector3(16.32f, -1.57f, -14.6f);
    //private Vector3 offset = new Vector3(0.03f, 1.12f, -3.88f);
    //private Vector3 birdOffset = new Vector3(0.08f, -0.69f, -4.23f);
    //private Vector3 birdOffset = new Vector3(0.03f, 0.12f, -3.88f); //birdOffset is always y-1 compared to regular off
    //private Vector3 birdAttackOffset = new Vector3(0.08f, 1.31f, -4.23f); //logic is for the camera to not move vertically while an attack is going
    //private Vector3 birdAttackOffset = new Vector3(0.03f, 2.12f, -3.88f); //logic is for the camera to not move vertically while an attack is going

    //New offsets now that I'm using a focal point
    private Vector3 offset = new Vector3(0, 0.5f, 0.5f); //For some reason doesn't have the same XYZ as before playing
    private Vector3 birdOffset = new Vector3(0, -0.5f, 0.5f);
    //private Vector3 birdAttackOffset = new Vector3(0, 1.5f, 0.5f);

    public GameObject tiger;
    public GameObject bird;

    private PlayerController playerScript;

    private float speed = 7; //instead of 10 instead of 8

    //New camera controls for first person to be adapt
    public float sensX;
    public float sensY;
    //For orientation, just use player character's position
    float xRotation;
    float yRotation;
    public Transform orientation;

    private GameManager gameManager;
    void Start()
    {
        //player = GameObject.Find("player");
        playerScript = player.GetComponent<PlayerController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    //Changed from LateUpdate
    void Update()
    {
        //float mouseX = Input.GetAxis("MouseX");
        //float mouseY = Input.GetAxis("MouseY");
        float mouseX = Input.GetAxisRaw("MouseX") * Time.deltaTime * speed;
        float mouseY = Input.GetAxisRaw("MouseY") * Time.deltaTime * speed;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, 0);
        //transform.Translate(player.transform.position.x + 21, player.transform.position.y + 2, player.transform.position.z);
        //Maybe need to set rotation too        
        //transform.Translate(cameraPosX + 5.15f, cameraPosY + 1.89f, cameraPosZ - 0.01f);

        Vector3 viewDir = player.transform.position - new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        orientation.transform.forward = viewDir.normalized;
        if (playerScript.tigerActive == true)
        {
            //transform.position = tiger.transform.position + offset;
            //transform.position = tiger.transform.position + new Vector3(0, 1f, -3);
            transform.position = tiger.transform.position + new Vector3(0, 1f, 0);
        }
        if (playerScript.birdActive == true)
        {
            //transform.position = bird.transform.position + birdOffset;
            //if (playerScript.attack == false)
            //{
            //transform.position = bird.transform.position + birdOffset;
            //}
            //if (playerScript.attack == true)
            //{
            //transform.position = bird.transform.position + birdAttackOffset;
            //}
        }

        //For some reason startGame == true doesn't allow the next code to work
        if (gameManager.startingCutscene == false)
        {

            //Controlling camera
            //transform.Rotate(Vector3.up * MouseX * 10 * Time.deltaTime);
            //turn.x += Input.GetAxis("Mouse X") * 2;
            //transform.localRotation = Quaternion.Euler(0, turn.x, 0);

            //transform.Rotate(Vector3.up, mouseX * speed * Time.deltaTime);

            //if (transform.rotation.x >= 15 || transform.rotation.x <= -11)
            //{

            //transform.Rotate(Vector3.right, mouseY * speed * Time.deltaTime);
            //}

            //First Person Camera Con
            //Actually, I think this is part of it, because the 3rd Person tutorial stuff covers movement more
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0); //This is the confusing part and I'm pretty sure the part that correlates
            //with first person. Also, because I have the camera attached to the player
            Vector3 inputDir = orientation.transform.forward * verticalInput + orientation.transform.right * horizontalInput;
            if (inputDir != Vector3.zero)
            {
                tiger.transform.forward = Vector3.Slerp(tiger.transform.forward, inputDir.normalized, Time.deltaTime * speed);
            }

            if (Input.GetMouseButtonDown(2))
            {
                //transform.rotation = new Quaternion(0, 0, 0, 0);
                xRotation = 0;
                yRotation = 0;
            }
        }

    }
}
