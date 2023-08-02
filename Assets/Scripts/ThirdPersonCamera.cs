using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform playerObj;
    public Transform player;
    public Transform orientation;
    private GameObject tiger;
    private GameObject bird;
    private float speed = 5;
    public Rigidbody playerRb;
    private GameManager gameManager;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public CinemachineFreeLook cinemachineFL;
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
        cinemachineFL.m_XAxis.m_MaxSpeed = 0.0f;
        cinemachineFL.m_YAxis.m_MaxSpeed = 0.0f;
        cinemachineFL.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Had to replace instances of tiger with player and playerObj
        //I'm sure using tiger instead of playerObj caused prob
        //Acc8dentally used .transform when I'm already using Transform
        ///The major thing I had to fix was using the Player Object's and playerObj's transform
        ///I just caught a mistake where I'm still using tiger.transform, but it doesn't look like
        ///using tiger.transform caused any problems. It probably will cause problems when I use bird
        ///Doing this actually didn't change anything, probably because the tiger faces the same direction as the player ob
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Trying to make it so that the camera can't move in certain parts like the opening run as well as cutscenes
        ///This method doesn't work atm
        if (gameManager.startGame == true)
        {
            //Debug.Log("Game Has Start");
            cinemachineFL.m_XAxis.m_MaxSpeed = 200f;
            cinemachineFL.m_YAxis.m_MaxSpeed = 0.5f;
            cinemachineFL.gameObject.SetActive(true);
        }
        if (inputDir != Vector3.zero && gameManager.startGame == true)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * speed);
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
