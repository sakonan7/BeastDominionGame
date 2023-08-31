using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject mainCam;
    private ThirdPersonCamera camScript;
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
    public bool stageCleared = false;

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

    public static bool tutorialStage = true;
    public static bool stage2 = false;
    public static bool stage3 = false;
    public static bool bossStage = false;

    public bool startGame = false; //Set this to true so that you can move the player now

    public bool startingCutscene = true;

    public bool enemyJustDefeated = false;

    public bool foeStruck = false;
    public ParticleSystem dyingEffect;
    public ParticleSystem specialHitEffect;
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        camScript = mainCam.GetComponent<ThirdPersonCamera>();
        //musicSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        //StartGame();
        //Area1();
        //Starting Cutscene
        //StartCoroutine(OpeningSeconds());
        //playerScript.Cutscenes();
        //Time.timeScale = 0;
        //StartCoroutine(TheStoryScroll());
        //storyScroll = true;
        //storyScrollObject.gameObject.SetActive(true);
        //continueMessage.gameObject.SetActive(true);
        if (tutorialStage == true && storyScroll == true)
        {
           
            StartCoroutine(TheStoryScroll());
        }
        else if (tutorialStage == true && storyScroll == false)
        {
            StartCoroutine(NonStoryScroll());
        }
        else if (tutorialStage == false)
        {
            StartCoroutine(NonStoryScroll());
        }
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

        if (Input.GetKeyDown(KeyCode.V) && storyScroll == true)
        {
            storyScroll = false;
            storyScrollObject.gameObject.SetActive(false);
            continueMessage.gameObject.SetActive(false);
            startingCutscene = true;
            startGame = true;
            UIAppear();
            //camScript.PlayBattleMusic();
        }

        if (Input.GetMouseButtonDown(0) && gameOver == true)
        {
            SceneManager.LoadScene("Level 1");
        }
        ///Need to turn this into a meth
        
        //The only thing I can think of that's causing a problem is that the Update mthod is causing this piece of code to keep repeating over
        //and over again. This might be the case because the method I wrote in ThirdPersonCamera stops the music player before
        //changing the track and playing .
        //Maybe I should evoke the method the minute the last foe is defeated in EnemeyDefeated(
        //The weird thing is that i'm very damn sure I tried changing the music with the button press methods yesterday after this
        //conditional, but it worked the day aft
        if (numOfEnemies <= 0)
        {
            //playerScript.Cutscenes();
            //congratulationsMessage.gameObject.SetActive(true);
            //Time.timeScale = 0;
            //VictoryMusicOn();
            //camScript.ChangeMusic();
            //UIDisappear();
            stageCleared = true;
            //Destroy(barrier);
            if (tutorialStage == true)
            {
                tutorialStage = false;
                stage2 = true;
            }
            if (stage2 == true)
            {
                tutorialStage = false;
                stage3 = true;
            }
            
        }
        //Oh, thiswas causing a glitch
        //if (Input.GetMouseButtonDown(0) && gameEnd == true)
        //{
            //SceneManager.LoadScene("Level 1");
        //}
        if (Input.GetKeyDown(KeyCode.R))
        {
            camScript.PlayVictoryMusic();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            camScript.PlayBattleMusic();
        }
        //if (stage2 == true || bossStage == true)
        //{
            //StartCoroutine(StageSpawner());
        //}
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
        //StartCoroutine(TheTutorialMessage());
        storyScroll = false;
        storyScrollObject.gameObject.SetActive(false);
        startingCutscene = true;
        startGame = true;
        UIAppear();
        camScript.PlayBattleMusic();
    }
    IEnumerator NonStoryScroll()
    {
        yield return new WaitForSeconds(0.5f);
        //StartCoroutine(TheTutorialMessage());
        //storyScroll = false;
        //storyScrollObject.gameObject.SetActive(false);
        startingCutscene = true;
        startGame = true;
        UIAppear();
        camScript.PlayBattleMusic();
    }
    IEnumerator TheTutorialMessage()
    {
        storyScrollObject.gameObject.SetActive(false);
        tutorialMessageObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        //tutorialMessageObject.gameObject.SetActive(false);
        //StartCoroutine(TheTutorialMessage2());
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
    public void UIDisappear()
    {
        battleCommandsObject.SetActive(false);
        battleCommands2Object.SetActive(false);
        HPDisplay.SetActive(false);
        currentForm.SetActive(false);
    }

    //Turn this into Start Level and load the appropriatecutscene
    public void StartLevelMethod()
    {
        //mainCam.SetActive(true);
        //cutsceneCam.SetActive(false);
        startingCutscene = false;
        //playerScript.CutsceneOff();
        //playerScript.RunAnimationOff();
        //barrier.SetActive(true);
        //Area1();

        //BattleMusicOn();
        stageCleared = false;
        if (tutorialStage == true)
        {
            TutorialLevel();
        }
        if (stage2 == true)
        {
            Level2();
        }
        if (stage3 == true)
        {
            Level3();
        }
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
        camScript.PlayVictoryMusic();
    }
    public void GameOver()
    {
        gameOver = true;
        continueMessage.gameObject.SetActive(true);
    }
    public void StagesOff()
    {
        stage2 = false;
        bossStage = false;
    }
    public void TutorialLevel()
    {
        Vector3 wolfLocation = enemies[0].transform.position;
        //wolfLocation.x + 4
        Instantiate(enemies[1], new Vector3(player.transform.position.x - 1.5f, wolfLocation.y + 0.1f, wolfLocation.z - 6), enemies[0].transform.rotation);
        //Instantiate(enemies[1], new Vector3(wolfLocation.x + 8, wolfLocation.y, wolfLocation.z - 15), enemies[0].transform.rotation);
        //Instantiate(enemies[1], new Vector3(wolfLocation.x + 14.5f, wolfLocation.y, wolfLocation.z - 8), enemies[0].transform.rotation);
        numOfEnemies = 1;
    }
    public void Level2()
    {
        //Instantiate(enemies[2], new Vector3(enemies[2].transform.position.x, enemies[2].transform.position.y, enemies[2].transform.position.z), enemies[2].transform.rotation);
        Vector3 wolfLocation = enemies[0].transform.position;
        //wolfLocation.x + 4
        Instantiate(enemies[1], new Vector3(player.transform.position.x - 1.5f, wolfLocation.y, wolfLocation.z - 6), enemies[0].transform.rotation);
        playerScript.TransformLock();
        numOfEnemies = 1;
    }
    public void Level3()
    {
        Vector3 wolfLocation = enemies[0].transform.position;
        //wolfLocation.x + 4
        Instantiate(enemies[1], new Vector3(player.transform.position.x - 6f, wolfLocation.y, wolfLocation.z - 6), enemies[0].transform.rotation);
        playerScript.TransformLock();
    }
    public void BossLevel()
    {
        Debug.Log("Anything that may be needed to be loaded in thisscenelol");
    }

    //not sure how I will do this in Update because Update will keep evoking this. if it keeps invoking, it will keep instantiating
    //the enemies
    //Especially for sic. I just thought of it though, I will
    //only be evoking stages by using colliders. That means that it will only invoke the method once.
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
    //So the slowdown after defeating a foe doesn't get stacked if another foe gets defeated around the same 
    public void EnemyDefeated(Vector3 strikeArea)
    {
        dyingEffect.transform.position = new Vector3(strikeArea.x, strikeArea.y + 1.2f, strikeArea.z); ;
        dyingEffect.Play();
        Debug.Log("Dying Efect");
        if (enemyJustDefeated == false)
        {
            enemyJustDefeated = true;
            camScript.LockOff();
            if (playerScript.specialInvincibility == false)
            {
                StartCoroutine(SlowDown());
            }
            
        }
        numOfEnemies--;
        if (numOfEnemies <= 0)
        {
            camScript.PlayExplorationMusic();
        }
    }
    public void HitByTigerSpecial(Vector3 strikeArea)
    {
        //playerScript.PlayTigerSpecialStrike(strikeArea);
        specialHitEffect.transform.position = new Vector3(strikeArea.x, strikeArea.y + 1, strikeArea.z);
        specialHitEffect.Play();
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
    IEnumerator StageSpawner()
    {
        yield return new WaitForSeconds(2);
        playerScript.CutsceneOff();
        startGame = true;
        if (stage2 == true)
        {
            Level2();
        }
        if (bossStage == true)
        {
            BossLevel();
        }
        StagesOff();
    }
    
}
