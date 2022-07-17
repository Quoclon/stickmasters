using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    /*
    #region Singleton
    // Singleton instance.
    public static GameManager Inst = null;

    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Inst == null)
        {
            Inst = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Inst != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }
    #endregion
    */

    // Old type doesn't work?
    
    #region Singleton
    static GameManager _instance;
    
    //[Header("Game Manager in Menu")]
    //public bool isMenuMode;

    public static GameManager Inst
    {
        get
        {
            if (_instance == null)
            {
                // Used to determine if GameManager is in the menu or not
                if(SceneManager.GetActiveScene().buildIndex != 0)
                    _instance = GameObject.Find("SingletonWellKnownName").GetComponent<GameManager>();      // This is Broken -- See SoundManager
            }
            return _instance;
        }
    }

    // Initialize the singleton instance.
    private void Awake()
    {
        _instance = this;
        CheckMobileWebGL();
    }
    #endregion


    [Header("Game Mode")]
    public eGameMode gameMode;
    public bool isMobileWebGL;

    [Header("Global Options - Testing")]
    public float totalHealthPerPartModifier;
    //public bool noDmgIfWeaponGrounded;

    [Header("Spanwed Enemies")]
    public int spawnedEnemies;
    public int enemiesWaveNumber = 1;
    private int enemiesPerWave = 5;
    public int totalWaveDenominator = 5;


    [Header("Controls")]
    public VariableJoystick variableJoystickP1;
    public VariableJoystick variableJoystickP2;

    [Header("Canvas - Game Over")]
    public GameObject canvasGameOver;
    public GameObject mainMenuButton;
    public GameObject nextRoundButton;
    public TextMeshProUGUI gameOverText;

    [Header("Survival Mode")]
    int survivalRoundsCleared = 0;

    [Header("Game Over")]
    public bool isRoundOver;
    public bool isMatchOver;
    public int roundsToWin;
    public Players playerWinner;

    [Header("Canvas - Scores")]
    public TextMeshProUGUI teamTextP1;
    public TextMeshProUGUI teamTextP2;
    public TextMeshProUGUI teamTextP3;
    public TextMeshProUGUI teamTextP4;
    public TextMeshProUGUI scoreTextP1;
    public TextMeshProUGUI scoreTextP2;
    public TextMeshProUGUI scoreTextP3;
    public TextMeshProUGUI scoreTextP4;



    [Header("Player Body List")]
    public List<Body> playerBodyList;

    [Header("NPC Body List")]
    public List<Body> npcBodyList;

    [Header("Camera")]
    public CinemachineTargetGroup cinemachineTargetGroup;
    public float playerCameraTargetWeight;


    // Start is called before the first frame update
    void Start()
    {
        SetGameMode();
        SetupScoreCanvas();
    }

    void CheckMobileWebGL()
    {
        variableJoystickP1 = GameObject.FindGameObjectWithTag("JoystickP1").GetComponent<VariableJoystick>();   // ~ Improve setting this?
        variableJoystickP2 = GameObject.FindGameObjectWithTag("JoystickP2").GetComponent<VariableJoystick>();   // ~ Improve setting this?

        if (Platform.IsMobileBrowser())
        {
            // whatever for mobile browser
            isMobileWebGL = true;
        }
        else
        {
            // whatever for desktop browser
            isMobileWebGL = false;
            variableJoystickP1.gameObject.SetActive(false);
            variableJoystickP2.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TESTING
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isMatchOver)
                ResetScene();
        }

        // Instead of Clicking "Next Round" Button
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            if (isMatchOver)
                LoadMainMenu();

            else if (isRoundOver)
                ResetScene();
        }
        
    }

    void SetupScoreCanvas()
    {
        scoreTextP1.text = ScoreManager.Inst.scoreP1.ToString("F0");
        scoreTextP2.text = ScoreManager.Inst.scoreP2.ToString("F0");
        scoreTextP3.text = ScoreManager.Inst.scoreP3.ToString("F0");
        scoreTextP4.text = ScoreManager.Inst.scoreP4.ToString("F0");

        teamTextP1.gameObject.SetActive(false);
        teamTextP2.gameObject.SetActive(false);
        teamTextP3.gameObject.SetActive(false);
        teamTextP4.gameObject.SetActive(false);
    }

    void SetGameMode()
    {
        if(MainMenuManager.Inst != null)
            gameMode = MainMenuManager.Inst.gameMode;
    }

    public void AddPlayerToList(Body body, Players playerType)
    {
        //Debug.Log(body.gameObject.name + " " + playerType);

        if (playerType == Players.AI)
        {
            npcBodyList.Add(body);
            cinemachineTargetGroup.AddMember(body.chest.transform, 1, 0);

            if(gameMode != eGameMode.Survival)
                EnableScoreUI(playerType);
        }

        //if (playerType == Players.P1 || playerType == Players.P2)   // ~ TODO: use isPlayerTypePlayer()
        if (isPlayerTypePlayer(playerType))
        {
            playerCameraTargetWeight = 1;
            playerBodyList.Add(body);
            cinemachineTargetGroup.AddMember(body.chest.transform, playerCameraTargetWeight, 0);

            
            EnableScoreUI(playerType);
        }
    }

    void EnableScoreUI(Players _playerType)
    {
        // Only one 'text' in Surival or Coop ("Kills")
        if(gameMode == eGameMode.Survival)
        {
            teamTextP1.gameObject.SetActive(true);
            teamTextP1.text = "Kills:";
            return;
        }

        // Only one 'text' in Surival or Coop ("Kills")
        if (gameMode == eGameMode.Coop)
        {
            teamTextP1.gameObject.SetActive(true);
            teamTextP1.text = "Team 1:";

            teamTextP2.gameObject.SetActive(true);
            teamTextP2.text = "Team 2:";
            return;
        }

        // 1v1 + Multiplayer
        switch (_playerType)
        {
            case Players.P1:
                teamTextP1.gameObject.SetActive(true);                
                break;
            case Players.P2:
                teamTextP2.gameObject.SetActive(true);
                break;
            case Players.P3:
                teamTextP3.gameObject.SetActive(true);
                break;
            case Players.P4:
                teamTextP4.gameObject.SetActive(true);
                break;
            case Players.AI:
                teamTextP2.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void RemovePlayerFromList(Body body, Players playerType)
    {
        // ~TODO: These if-statements are essentially the same no, no need for survival specific?

        //Debug.Log("RemovePlayerFromList Ran");
        if (playerType == Players.AI)
        {
            npcBodyList.Remove(body);
            if (gameMode == eGameMode.Survival)
                cinemachineTargetGroup.RemoveMember(body.chest.transform);
        }

        //if (playerType == Players.P1 || playerType == Players.P2)
        if (isPlayerTypePlayer(playerType))
        {
            playerBodyList.Remove(body);
            cinemachineTargetGroup.RemoveMember(body.chest.transform);
        }



        //StartCoroutine(RemovePlayerFromTargetGroup(body, .5f));

    }

    // ~ TODO: it hard pans to last player at Game Over it they are far away from each other
    IEnumerator RemovePlayerFromTargetGroup(Body body, float waitTime)
    {

        yield return new WaitForSeconds(waitTime);
        cinemachineTargetGroup.RemoveMember(body.chest.transform);
    }


    public void CheckGameOver(Players playerKilled, Players playerDealingLastBlow, Body bodyKilled)
    {
        Debug.Log("playerList.length: " + playerBodyList.Count);
        RemovePlayerFromList(bodyKilled, playerKilled);

       

        if(gameMode == eGameMode.MultiPlayer)
        {
            if(playerBodyList.Count == 1)
                GameOver(playerDealingLastBlow);
        }
        else
        {
            if (npcBodyList.Count <= 0 || playerBodyList.Count <= 0)    // one group is completely dead
                GameOver(playerDealingLastBlow);
        }
          
    }

    // ~ Convert this to not be "Player Type" - but rather player "Body" or something specific (i.e. not NPC)
    public void GameOver(Players playerDealingLastBlow)
    {
        //Debug.Log("Game Over - Player Wins Round: " + playerDealingLastBlow.ToString());

        // Used throughout game
        isRoundOver = true;

        if (gameMode == eGameMode.Survival)
        {
            // If it is a player who dealt the "last blow" (i.e. No NPCs) - spawn another
            if(playerBodyList.Count > 0 && npcBodyList.Count <= 0)
            {
                //Debug.Log("Ran the Survival Mode Check for Spawning");

                // Increase Rounds
                survivalRoundsCleared++;

                // Display Text after each round
                ScoreManager.Inst.scoreP1 = survivalRoundsCleared;
                scoreTextP1.text = ScoreManager.Inst.scoreP1.ToString("F0");

                // Pan out camera to see carnage
                StartCoroutine(WaitToSpawn(0.5f));
            }

            // Survival Match Over
            if (playerBodyList.Count <= 0)
            {
                isMatchOver = true;
                gameOverText.text = "Enemies Defeated: " + survivalRoundsCleared.ToString("F0");
                StartCoroutine(WaitToZoomOut(2));

            }
        }

        // Round is Over, Only One Player, Check which player is left alive if Environmental Kill
        if (playerDealingLastBlow == Players.Environment)
        {
            // Pick a player or NPC to be the "Winner" if there's multiple and it was environment
            foreach (var player in npcBodyList)
            {
                if (player.alive)
                    playerDealingLastBlow = player.playerType;
            }

            foreach (var player in playerBodyList)
            {
                if (player.alive)
                    playerDealingLastBlow = player.playerType;
            }

        }

     
        if (gameMode == eGameMode.SinglePlayer || gameMode == eGameMode.MultiPlayer)
        {

            if (playerDealingLastBlow == Players.P1)                                        
            {
                ScoreManager.Inst.scoreP1 += 1;
                scoreTextP1.text = ScoreManager.Inst.scoreP1.ToString("F0");

                if (ScoreManager.Inst.scoreP1 < roundsToWin)
                {
                    gameOverText.text = "Player 1 Wins Round!";
                    nextRoundButton.SetActive(true);
                    mainMenuButton.SetActive(false);
                }
                else
                {
                    isMatchOver = true;
                    gameOverText.text = "Player 1 Wins Match!";
                    mainMenuButton.SetActive(true);
                    nextRoundButton.SetActive(false);
                }

            }

            if (playerDealingLastBlow == Players.P2)
            {
                ScoreManager.Inst.scoreP2 += 1;
                scoreTextP2.text = ScoreManager.Inst.scoreP2.ToString("F0");

                if (ScoreManager.Inst.scoreP2 < roundsToWin)
                {
                    gameOverText.text = "Player 2 Wins Round!";
                    nextRoundButton.SetActive(true);
                    mainMenuButton.SetActive(false);
                }
                else
                {
                    isMatchOver = true;
                    gameOverText.text = "Player 2 Wins Match!";
                    mainMenuButton.SetActive(true);
                    nextRoundButton.SetActive(false);
                }

            }

            if (playerDealingLastBlow == Players.P3)
            {
                ScoreManager.Inst.scoreP3 += 1;
                scoreTextP3.text = ScoreManager.Inst.scoreP3.ToString("F0");

                if (ScoreManager.Inst.scoreP3 < roundsToWin)
                {
                    gameOverText.text = "Player 3 Wins Round!";
                    nextRoundButton.SetActive(true);
                    mainMenuButton.SetActive(false);
                }
                else
                {
                    isMatchOver = true;
                    gameOverText.text = "Player 3 Wins Match!";
                    mainMenuButton.SetActive(true);
                    nextRoundButton.SetActive(false);
                }

            }

            if (playerDealingLastBlow == Players.P4)
            {
                ScoreManager.Inst.scoreP4 += 1;
                scoreTextP4.text = ScoreManager.Inst.scoreP4.ToString("F0");

                if (ScoreManager.Inst.scoreP4 < roundsToWin)
                {
                    gameOverText.text = "Player 4 Wins Round!";
                    nextRoundButton.SetActive(true);
                    mainMenuButton.SetActive(false);
                }
                else
                {
                    isMatchOver = true;
                    gameOverText.text = "Player 4 Wins Match!";
                    mainMenuButton.SetActive(true);
                    nextRoundButton.SetActive(false);
                }

            }

            if (playerDealingLastBlow == Players.AI)
            {
                ScoreManager.Inst.scoreP2 += 1;
                scoreTextP2.text = ScoreManager.Inst.scoreP2.ToString("F0");

                if (ScoreManager.Inst.scoreP2 < roundsToWin)
                {
                    gameOverText.text = "NPC Wins Round!";
                    nextRoundButton.SetActive(true);
                    mainMenuButton.SetActive(false);
                }
                else
                {
                    isMatchOver = true;
                    gameOverText.text = "NPC Wins Match!";
                    mainMenuButton.SetActive(true);
                    nextRoundButton.SetActive(false);
                }
            }
        }

        if(gameMode == eGameMode.Coop)
        {
            if (playerDealingLastBlow == Players.P1 || playerDealingLastBlow == Players.P2)
            {
                ScoreManager.Inst.scoreP1 += 1;
                scoreTextP1.text = ScoreManager.Inst.scoreP1.ToString("F0");

                if (ScoreManager.Inst.scoreP1 < roundsToWin)
                {
                    gameOverText.text = "Player Team Wins Round!";
                    nextRoundButton.SetActive(true);
                    mainMenuButton.SetActive(false);
                }
                else
                {
                    isMatchOver = true;
                    gameOverText.text = "Player Team Wins Match!";
                    mainMenuButton.SetActive(true);
                    nextRoundButton.SetActive(false);
                }
            }

            if (playerDealingLastBlow == Players.AI)
            {
                ScoreManager.Inst.scoreP2 += 1;
                scoreTextP2.text = ScoreManager.Inst.scoreP2.ToString("F0");

                if (ScoreManager.Inst.scoreP2 < roundsToWin)
                {
                    gameOverText.text = "NPC Team Wins Round!";
                    nextRoundButton.SetActive(true);
                    mainMenuButton.SetActive(false);
                }
                else
                {
                    isMatchOver = true;
                    gameOverText.text = "NPC Team Wins Match!";
                    mainMenuButton.SetActive(true);
                    nextRoundButton.SetActive(false);
                }
            }
        }


        if((isRoundOver && gameMode != eGameMode.Survival) || isMatchOver)
        {
            // ~ TODO: Set Canvas after a few seconds using Coroutine
            StartCoroutine(WaitForGammeOverCorourtine(2));

            // Assign Winner
            playerWinner = playerDealingLastBlow;

            //Disable Weapons of all non-winning players
            DisablePlayerWeapons();
        }
    }

    void DisablePlayerWeapons()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Body playerMovement = player.GetComponent<Body>();

            if (playerMovement.playerType != playerWinner)
            {

                foreach (var weapon in player.GetComponentsInChildren<Weapon>())
                {
                    weapon.weaponDisabled = true;
                    weapon.gameObject.layer = 8;
                } 
            }
        }
    }


    // ~ TODO: Couldn't figure out Awake, Enable, Start order with rest of scripts -- So Variables for next round itialized here
    public void ResetScene()
    {
        isRoundOver = false;
        isMatchOver = false;

        survivalRoundsCleared = 0;
        scoreTextP1.text = ScoreManager.Inst.scoreP1.ToString("F0");
        scoreTextP2.text = ScoreManager.Inst.scoreP2.ToString("F0");
        scoreTextP3.text = ScoreManager.Inst.scoreP3.ToString("F0");
        scoreTextP4.text = ScoreManager.Inst.scoreP4.ToString("F0");


        canvasGameOver.SetActive(false);

        Time.timeScale = 1;
        npcBodyList.Clear();
        playerBodyList.Clear();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        isRoundOver = false;
        isMatchOver = false;
        canvasGameOver.SetActive(false);

        Time.timeScale = 1;
        npcBodyList.Clear();
        playerBodyList.Clear();
       
        ScoreManager.Inst.scoreP1 = 0;
        ScoreManager.Inst.scoreP2 = 0;

        SceneManager.LoadScene(0);
    }

    private IEnumerator WaitForGammeOverCorourtine(float waitTime)
    {
        // Wait until running -- do nothing first
        yield return new WaitForSeconds(waitTime);

        if (isRoundOver)
        {
            canvasGameOver.SetActive(true);
        }
            
    }

    private IEnumerator WaitToSpawn(float waitTime)
    {
        // Wait until running -- do nothing first
        yield return new WaitForSeconds(waitTime);
        GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>().SpawnCharacters();
    }

    private IEnumerator WaitToZoomOut(float waitTime)
    {
        // Wait until running -- do nothing first
        yield return new WaitForSeconds(waitTime);
        // ~ TODO: create actual zoom out points, or do the camera in some other more performant way (i.e. just zoom, unfollow target)
        GameObject[] causesDamageEnvironmentObjects = GameObject.FindGameObjectsWithTag("CausesDamage");
        foreach (var npc in npcBodyList)
        {
            cinemachineTargetGroup.RemoveMember(npc.chest.transform);
        }

        foreach (var boundary in causesDamageEnvironmentObjects)
        {
            Debug.Log("wall name: " + boundary.name);
            float weight = 1;
            if (boundary.name == "Block - Top")
                weight = 2;

            cinemachineTargetGroup.AddMember(boundary.transform, weight, 0);
        }
    }

    public void UpdateSpawnedEnemiesStats(int numberOfSpawnedEnemies)
    {
        spawnedEnemies = numberOfSpawnedEnemies;

        //Debug.Log("spawnedEnemies % enemiesPerWave " + spawnedEnemies % enemiesPerWave);
        if (( (spawnedEnemies) % enemiesPerWave == 0))
        {
            enemiesWaveNumber++;
        }
        
    }

    public void SetGameMode(eGameMode _gameMode)
    {
        gameMode = _gameMode;
    }

    public bool isPlayerTypePlayer(Players playerType)
    {
        switch (playerType)
        {
            case Players.P1:
                return true;
            case Players.P2:
                return true;
            case Players.P3:
                return true;
            case Players.P4:
                return true;
            case Players.AI:
                return false;
            case Players.Environment:
                return false;
            default:
                return false;
        }
    }
}

public enum eGameMode
{
    SinglePlayer,
    MultiPlayer,
    Coop,
    Survival
}

public enum Players
{
    P1,
    P2,
    P3,
    P4,
    AI,
    Environment
}

public enum BodyParts
{
    Default,
    Head,
    Body,
    UpperRightArm,
    LowerRightArm,
    UpperLeftArm,
    LowerLeftArm,
    UpperLeftLeg,
    LowerLeftLeg,
    UpperRightLeg,
    LowerRightLeg,
    WeaponHolder
}

public enum eWeaponType
{
    None,
    Katana,
    KatanaDefense,
    GreatSword,
    GreatSwordBalanced,
    GreatSwordDefense,
    Cudgel,
    Dagger,
    Spear,
    BroadSword,
    Whip,
    Flail,
    Nunchuck,
    Sabre,
    Random,
    BrassKnuckles
}

public enum eDamageType
{
    standard,
    bleed
}

public enum eWeaponHolderOptions
{
    NoWeapon,
    Default,
    Random,
    WeightedRandom,
    EntireArm
}

public enum eWeaponArms
{
    Left,
    Right
}

