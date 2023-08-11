using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject cutsceneCam;

    private bool gameOver = false;
    public GameObject player;
    private PlayerController playerScript;
    public GameObject tiger;
    public GameObject barrier;
    private Vector3 playerPositionStart = new Vector3(16.4f, 0, -11.24f);
    public List<GameObject> enemies;
    public string difficulty = "Normal"; //Before using buttons
    private int numOfEnemies = 1;
    public bool battleStart = false;
    public bool gameEnd = false;

    //Canvas
    public Canvas areaCanvas;
    public Image foeHPBar;
    public GameObject startMenu;
    public Button normal;
    public Button hard;
    public Button veryHard;
    public TextMeshProUGUI music;
    private string startMenuMusic = "Lightning Temple By Motoi Sakuraba";
    private string battleMusic = "Field Battle By Manaka Kataoka";
    private string explorationMusic = "Uncharted Island By Greg Edmonson";
    private string victoryMusic = "Victory Fanfare By Hitoshi Sakimoto";
    public TextMeshProUGUI continueMessage;
    private bool storyScroll = false;
    public TextMeshProUGUI storyScrollObject;
    private bool tutorialMessage = false;
    public TextMeshProUGUI tutorialMessageObject;
    private bool tutorialMessage2 = false;
    public TextMeshProUGUI tutorialMessageObject2;
    public GameObject battleCommandsObject;
    public GameObject battleCommands2Object;
    public GameObject currentForm;
    public GameObject HPDisplay;
    public TextMeshProUGUI congratulationsMessage;

    public bool startGame = false; //Set this to true so that you can move the player now

    public bool startingCutscene = true;

    public bool enemyJustDefeated = false;

    public bool foeStruck = false;
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        //musicSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        //StartGame();
        //Area1();
        //Starting Cutscene
        //StartCoroutine(OpeningSeconds());
        //playerScript.Cutscenes();
        //Time.timeScale = 0;
        //StartCoroutine(TheStoryScroll());
        storyScroll = true;
        storyScrollObject.gameObject.SetActive(true);
        continueMessage.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //DisplayEnemyHP();
        //This method will run until startGame == true
        //if (startGame == false)
        //{
        //StartGame();
        //}
        //if (storyScroll == true)
        //{
        //TheStoryScroll();
        //}
        //else if (tutorialMessage == true)
        //{
        //TheTutorialMessage();
        //}
        //Need to make player unable to move until storyScroll and startingRun are 
        if (Input.GetMouseButtonDown(0) && storyScroll == true)
        {
            storyScroll = false;
            storyScrollObject.gameObject.SetActive(false);
            tutorialMessageObject.gameObject.SetActive(true);
            tutorialMessage = true;
        }
        else if (Input.GetMouseButtonDown(0) && tutorialMessage == true)
        {
            tutorialMessage = false;
            tutorialMessageObject.gameObject.SetActive(false);
            tutorialMessageObject2.gameObject.SetActive(true);
            tutorialMessage2 = true;
        }
        else if (Input.GetMouseButtonDown(0) && tutorialMessage2 == true)
        {
            tutorialMessage2 = false;
            tutorialMessageObject2.gameObject.SetActive(false);
            continueMessage.gameObject.SetActive(false);
            startingCutscene = true;
            startGame = true;
            UIAppear();
        }
        if (Input.GetMouseButtonDown(0) && gameOver == true)
        {
            SceneManager.LoadScene("Level 1");
        }
        ///Need to turn this into a meth
        if (numOfEnemies == 0)
        {
            playerScript.Cutscenes();
            congratulationsMessage.gameObject.SetActive(true);
            //Time.timeScale = 0;
            VictoryMusicOn();
        }
        if (Input.GetMouseButtonDown(0) && gameEnd == true)
        {
            SceneManager.LoadScene("Level 1");
        }
    }

    //public void StartGame(string inputedDifficulty)
    //{
    //Instantiate(player, player.transform.position, player.transform.rotation);
    //normal.onClick = difficulty == "Normal";
    //difficulty = inputedDifficulty;
    //startGame = true;
    //Area1();
    //}
    void ResetPlayerPosition()
    {
        player.transform.position = playerPositionStart;
    }
    //Game Manger will spawn enemies in specific waves in each area, tentatively through scenes to save on
    IEnumerator OpeningSeconds()
    {
        yield return new WaitForSeconds(2);
        //playerScript.OpeningRunDone();
        startingCutscene = false;
        playerScript.RunAnimationOff();
        barrier.SetActive(true);
        Area1();
        //musicSource.clip = battle;
        //musicSource.Play();
    }
    IEnumerator TheStoryScroll()
    {
        storyScrollObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        StartCoroutine(TheTutorialMessage());
    }
    IEnumerator TheTutorialMessage()
    {
        storyScrollObject.gameObject.SetActive(false);
        tutorialMessageObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        tutorialMessageObject.gameObject.SetActive(false);
        StartCoroutine(TheTutorialMessage2());
    }
    IEnumerator TheTutorialMessage2()
    {
        tutorialMessageObject2.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        tutorialMessageObject2.gameObject.SetActive(false);
        //Time.timeScale = 1;
        startingCutscene = true;
        startGame = true;
        UIAppear();
    }
    public void UIAppear() {
        battleCommandsObject.SetActive(true);
        battleCommands2Object.SetActive(true);
        HPDisplay.SetActive(true);
        currentForm.SetActive(true);
}
    public void StartGameMethod()
    {
        //mainCam.SetActive(true);
        //cutsceneCam.SetActive(false);
        startingCutscene = false;
        //playerScript.CutsceneOff();
        //playerScript.RunAnimationOff();
        barrier.SetActive(true);
        //Area1();
        TutorialLevel();
        //BattleMusicOn();
    }
    public void BattleMusicOn()
    {
        battleStart = true;
        //May not work when switching music from exploration mus
        music.text = "Music: " + battleMusic;
        music.transform.Translate(44, 0, 0);
    }
    public void VictoryMusicOn()
    {
        gameEnd = true;
        //May not work when switching music from exploration mus
        music.text = "Music: " + victoryMusic;
        music.transform.Translate(0, 0, 0);
        continueMessage.gameObject.SetActive(true);
    }
    public void GameOver()
    {
        gameOver = true;
        continueMessage.gameObject.SetActive(true);
    }
    public void TutorialLevel()
    {
        Vector3 wolfLocation = enemies[0].transform.position;
        Instantiate(enemies[1], new Vector3(wolfLocation.x + 4, wolfLocation.y, wolfLocation.z - 6), enemies[0].transform.rotation);
        Instantiate(enemies[1], new Vector3(wolfLocation.x + 8, wolfLocation.y, wolfLocation.z - 15), enemies[0].transform.rotation);
        Instantiate(enemies[1], new Vector3(wolfLocation.x + 14.5f, wolfLocation.y, wolfLocation.z - 8), enemies[0].transform.rotation);
        numOfEnemies = 3;
    }
    public void Area1()
    {
        //startMenu.SetActive(false);
        //tiger.SetActive(true);
        Vector3 wolfLocation = enemies[0].transform.position;
        //Instantiate(enemies[0], enemies[0].transform.position, enemies[0].transform.rotation);
        //Enemy #
        Instantiate(enemies[1], new Vector3(wolfLocation.x + 1, wolfLocation.y, wolfLocation.z - 6), enemies[0].transform.rotation);
        //Enemy # 2
        //Instantiate(enemies[0], new Vector3(wolfLocation.x + 19, wolfLocation.y, wolfLocation.z - 3), enemies[0].transform.rotation);
        Instantiate(enemies[1], new Vector3(wolfLocation.x + 5, wolfLocation.y, wolfLocation.z - 15), enemies[0].transform.rotation);
        //Instantiate(enemies[1], new Vector3(wolfLocation.x + 7, wolfLocation.y, wolfLocation.z - 10), enemies[0].transform.rotation);
        Instantiate(enemies[1], new Vector3(wolfLocation.x + 11.5f, wolfLocation.y, wolfLocation.z - 8), enemies[0].transform.rotation);
        //Instantiate(enemies[1], new Vector3(enemies[0].transform.position.x + 30, enemies[0].transform.position.y, enemies[0].transform.position.z - 5), enemies[0].transform.rotation);
        //foeHPBar.gameObject.SetActive(true);
        //string name tage foes for the HP Bar and damage display to have something to follow
        //Instantiate(foeHPBar, enemies[1].transform.position, enemies[1].transform.rotation); //I'm not sure this is even going
        //to Instantiate on the Canvas
        //Instantiate(enemies[1], new Vector3(enemies[1].transform.position.x + 10, enemies[1].transform.position.y, enemies[1].transform.position.z - 5), enemies[1].transform.rotation);
        //enemies[1].name = "Monkey";
        //May need to make a boolean for each foe when they get damaged so I can reduce their HP Bar
        numOfEnemies = 3;
    }
    //This is all temporary code because it would be weird for enemy HP to be displayed like this
    public void DisplayEnemyHP ()
    {
        //If I want to display enemy HP, I will need to use Z = 0 all the time and or use different variables, not transform.position
        foeHPBar.transform.position = new Vector3(enemies[0].transform.position.x, enemies[0].transform.position.y + 5, 0);
    }
    //So the slowdown after defeating a foe doesn't get stacked if another foe gets defeated around the same 
    public void EnemyDefeated()
    {
        if (enemyJustDefeated == false)
        {
            enemyJustDefeated = true;
            StartCoroutine(SlowDown());
        }
        numOfEnemies--;
    }
    IEnumerator SlowDown()
    {
        Time.timeScale = 0.5f; //Instead of 0.2 because I think 0.2 makes it slow down even
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 1;
        enemyJustDefeated = false;
    }
    public void NoAttackLag()
    {
        foeStruck = true;
    }
    public void ResetFoeStruck()
    {
        foeStruck = false;
    }

    public void OnMouseUp()
    {
        Debug.Log("Clicked On Stuff");
        if (storyScroll == true && gameObject.CompareTag("Continue"))
        {
            storyScroll = false;
            storyScrollObject.gameObject.SetActive(false);
            tutorialMessage = true;
        }
        if (tutorialMessage == true && gameObject.CompareTag("Continue"))
        {
            tutorialMessage = false;
            tutorialMessageObject.gameObject.SetActive(false);
            startingCutscene = true;
        }
    }
}
