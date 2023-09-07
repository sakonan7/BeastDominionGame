using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCamera : MonoBehaviour
{
    private AudioSource musicSource;
    public AudioClip title;
    public AudioClip battle;
    public AudioClip exploration;
    public AudioClip boss;
    public AudioClip victory;
    public Transform playerObj;
    public Transform player;
    public Transform birdFollow;
    private PlayerController playerScript;
    public Transform orientation;

    public GameObject tiger;
    public GameObject bird;
    private float speed = 4;
    public Rigidbody playerRb;
    private GameManager gameManager;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public CinemachineFreeLook cinemachineFL;

    private float tigerPosition;
    private float birdPosition;
    private Vector3 viewDir;

    private bool evokeOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        //tiger = GameObject.Find("Tiger");
        //bird = GameObject.Find("Bird");
        playerScript = player.gameObject.GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        cinemachineFL.m_XAxis.m_MaxSpeed = 0.0f;
        cinemachineFL.m_YAxis.m_MaxSpeed = 0.0f;
        cinemachineFL.gameObject.SetActive(false);
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = title;
        musicSource.Play();

        tigerPosition = player.position.y;
        birdPosition = player.position.y + 1;
       
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
        viewDir = player.position - new Vector3(transform.position.x, tigerPosition, transform.position.z);
        orientation.forward = viewDir.normalized;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Trying to make it so that the camera can't move in certain parts like the opening run as well as cutscenes
        ///This method doesn't work atm
        if (gameManager.startGame == true)
        {
            //Debug.Log("Game Has Start");
            cinemachineFL.m_XAxis.m_MaxSpeed = 120f;
            cinemachineFL.m_YAxis.m_MaxSpeed = 0.1f;
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
            //Debug.Log("Recentered");
            //transform.position = originalPosition;
            //transform.rotation = originalRotation;
            cinemachineFL.m_RecenterToTargetHeading.m_enabled = true;
            cinemachineFL.m_YAxisRecentering.m_enabled = true;
            StartCoroutine(TurnOffRecenter());
        }
        //if (gameManager.battleStart == true)
        //{
            //PlayBattleMusic();
        //}
        //if (gameManager.gameEnd == true && evokeOnce == true)
        //{
            //musicSource.Stop();
            //PlayVictoryMusic();
            //Debug.Log("Change the music..");
            //evokeOnce = false;
        //}
    }
    public void ChangeForms()
    {
        if (playerScript.tigerActive == true)
        {
            playerObj = bird.transform;
            cinemachineFL.m_Follow = birdFollow;
            //cinemachineFL.m_Follow = player.transform.position + new Vector3(0, 1.5f, 0);
            //viewDir = player.position - new Vector3(transform.position.x, birdPosition, transform.position.z);
        }
        if (playerScript.birdActive == true)
        {
            playerObj = tiger.transform;
            cinemachineFL.m_Follow = player;
            //viewDir = player.position - new Vector3(transform.position.x, tigerPosition, transform.position.z);
        }
    }
    public void ChangeMusic()
    {
        evokeOnce = true;
    }
    public void PlayBattleMusic()
    {
        musicSource.Stop();
        musicSource.clip = battle;
        musicSource.Play();
    }
    public void PlayVictoryMusic()
    {
        musicSource.Stop();
        musicSource.clip = victory;
        musicSource.Play();
    }
    public void PlayExplorationMusic()
    {
        musicSource.Stop();
        musicSource.clip = exploration;
        musicSource.Play();
    }
        public void PlayUsualBossMusic()
    {
        musicSource.Stop();
        musicSource.clip = boss;
        musicSource.Play();
    }
    IEnumerator TurnOffRecenter()
    {
        yield return new WaitForSeconds(0.1f);
        cinemachineFL.m_RecenterToTargetHeading.m_enabled = false;
        cinemachineFL.m_YAxisRecentering.m_enabled = false;
    }
    public void TurnToTarget(Transform target)
    {
        //transform.LookAt(target);
        cinemachineFL.m_LookAt = target;
        //cinemachineFL.
        //StartCoroutine(BackToPlayer());
    }
    public void LockOff()
    {
        cinemachineFL.m_LookAt = player.transform;
    }
    IEnumerator BackToPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        if (playerScript.tigerActive == true)
        {
            cinemachineFL.m_LookAt = player.transform;
        }
        if (playerScript.tigerActive == true)
        {
            cinemachineFL.m_LookAt = birdFollow;
        }
    }
    public void ScreenShakeMethod()
    {
        StartCoroutine(ScreenShake());
    }
    IEnumerator ScreenShake()
    {
        cinemachineFL.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 5f;
        yield return new WaitForSeconds(0.5f);
        cinemachineFL.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }
}
