using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform playerObj;
    public Transform player;
    public Transform orientation;
    private GameObject tiger;
    private GameObject bird;
    private float speed = 5;
    private GameManager gameManager;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    // Start is called before the first frame update
    void Start()
    {
        tiger = GameObject.Find("Tiger");
        bird = GameObject.Find("Bird");
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //Had to replace instances of tiger with player and playerObj
        //I'm sure using tiger instead of playerObj caused prob
        Vector3 viewDir = tiger.transform.position - new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Trying to make it so that the camera can't move in certain parts like the opening run as well as cutscenes
        ///This method doesn't work atm
        if (gameManager.startGame == false)
        {
            
        }
        if (inputDir != Vector3.zero && gameManager.startGame == true)
        {
            playerObj.transform.forward = Vector3.Slerp(playerObj.transform.forward, inputDir.normalized, Time.deltaTime * speed);
        }
        if (Input.GetMouseButtonDown(2))
        {
            //horizontalInput = 0;
            //verticalInput = 0;
            //viewDir = new Vector3(0,0,0);
            Debug.Log("Recentered");
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }
    }
}
