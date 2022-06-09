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
    public static GameManager Inst
    {
        get
        {
            if (_instance == null)
            {
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

    [Header("Controls")]
    public VariableJoystick variableJoystickP1;
    public VariableJoystick variableJoystickP2;

    [Header("Canvas - Game Over")]
    public GameObject canvasGameOver;
    public GameObject mainMenuButton;
    public GameObject nextRoundButton;
    public TextMeshProUGUI gameOverText;

    [Header("Game Over")]
    public bool isRoundOver;
    public int roundsToWin;
    public Players playerWinner;

    [Header("Canvas - Scores")]
    public TextMeshProUGUI scoreTextP1;
    public TextMeshProUGUI scoreTextP2;

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
        if (Input.GetKeyDown(KeyCode.R))
            ResetScene();
    }

    void SetupScoreCanvas()
    {
        scoreTextP1.text = ScoreManager.Inst.scoreP1.ToString("F0");
        scoreTextP2.text = ScoreManager.Inst.scoreP2.ToString("F0");
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
        }

        if (playerType == Players.P1 || playerType == Players.P2)
        {
            playerCameraTargetWeight = 1;
            playerBodyList.Add(body);
            cinemachineTargetGroup.AddMember(body.chest.transform, playerCameraTargetWeight, 0);
        }

    }
    public void RemovePlayerFromList(Body body, Players playerType)
    {
        if (playerType == Players.AI)
            npcBodyList.Remove(body);

        if (playerType == Players.P1 || playerType == Players.P2)
            playerBodyList.Remove(body);

        cinemachineTargetGroup.RemoveMember(body.chest.transform);

    }


    public void CheckGameOver(Players playerKilled, Players playerDealingLastBlow, Body bodyKilled)
    {

        RemovePlayerFromList(bodyKilled, playerKilled);

        if (npcBodyList.Count <= 0 || playerBodyList.Count <= 0)
            GameOver(playerDealingLastBlow);         
    }

    // ~ Convert this to not be "Player Type" - but rather player "Body" or something specific (i.e. not NPC)
    public void GameOver(Players playerDealingLastBlow)
    {
        Debug.Log("Game Over - Player Wins Round: " + playerDealingLastBlow.ToString());

        if (playerDealingLastBlow == Players.P1)
        {
            ScoreManager.Inst.scoreP1 += 1;
            scoreTextP1.text = ScoreManager.Inst.scoreP1.ToString("F0");
            //gameOverText.text = "Player 1 Wins Round!";
            Debug.Log("ScoreManager.Inst.scoreP1: " + ScoreManager.Inst.scoreP1);
            Debug.Log("Rounds To Win" + roundsToWin);

            if (ScoreManager.Inst.scoreP1 < roundsToWin)
            {
                Debug.Log("ScoreManager.Inst.scoreP1 >= roundsToWin");
                gameOverText.text = "Player 1 Wins Round!";
                nextRoundButton.SetActive(true);
                mainMenuButton.SetActive(false);
            }
            else
            {
                gameOverText.text = "Player 1 Wins Match!";
                mainMenuButton.SetActive(true);
                nextRoundButton.SetActive(false);
            }

        }

        if (playerDealingLastBlow == Players.P2)
        {
            ScoreManager.Inst.scoreP2 += 1;
            scoreTextP2.text = ScoreManager.Inst.scoreP2.ToString("F0");
            //gameOverText.text = "Player 2 Wins Round!";

            if (ScoreManager.Inst.scoreP2 < roundsToWin)
            {
                gameOverText.text = "Player 2 Wins Round!";
                nextRoundButton.SetActive(true);
                mainMenuButton.SetActive(false);
            }
            else
            {
                gameOverText.text = "Player 2 Wins Match!";
                mainMenuButton.SetActive(true);
                nextRoundButton.SetActive(false);
            }

        }

        if (playerDealingLastBlow == Players.AI)
        {
            ScoreManager.Inst.scoreP2 += 1;
            scoreTextP2.text = ScoreManager.Inst.scoreP2.ToString("F0");

            if(ScoreManager.Inst.scoreP2 < roundsToWin)
            {
                gameOverText.text = "NPC Wins Round!";
                nextRoundButton.SetActive(true);
                mainMenuButton.SetActive(false);
            }
            else
            {
                gameOverText.text = "NPC Wins Match!";
                mainMenuButton.SetActive(true);
                nextRoundButton.SetActive(false);
            }
        }

        // ~ TODO: Set Canvas after a few seconds using Coroutine
        StartCoroutine(WaitForGammeOverCorourtine(2));

        // Used throughout game
        isRoundOver = true;

        // Disable all players but Winner
        playerWinner = playerDealingLastBlow;
        DisablePlayers();
    }

    void DisablePlayers()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Movement playerMovement = player.GetComponent<Movement>();

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
        canvasGameOver.SetActive(false);

        Time.timeScale = 1;
        npcBodyList.Clear();
        playerBodyList.Clear();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        isRoundOver = false;
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


    #region Archive
    void SetupBodiesInScene()
    {
        /*
        GameObject[] allPlayersAndNpcs = GameObject.FindGameObjectsWithTag("Player");

        Debug.Log("allPlayersAndNpcs.Count: " + allPlayersAndNpcs.Length);

        foreach (var player in allPlayersAndNpcs)
        {
            Body playerBody = player.GetComponent<Body>();
            playerBody.SetupBody();
        }
        */
    }

    void AwakeSetupMethods()
    {
        //Time.timeScale = 1f;
        //npcBodyList.Clear();
        //playerBodyList.Clear();

        //levelRestarted = true;
        //SetupBodiesInScene(); 
    }

    public void SetPlayerAndNpcArrays()
    {
        /*
        // CLear Lists since Singleton
        npcBodyList.Clear();
        playerBodyList.Clear();

        GameObject[] allPlayersAndNpcs = GameObject.FindGameObjectsWithTag("Player");

        Debug.Log("allPlayersAndNpcs.Count: " + allPlayersAndNpcs.Length);

        int numberOfNpcs = 0;
        int numberOfPlayers = 0;

        foreach (var player in allPlayersAndNpcs)
        {
            Body playerBody = player.GetComponent<Body>();

            Debug.Log(playerBody.gameObject.name);

            if (playerBody.playerType == Players.AI)
            {
                numberOfNpcs++;
                npcBodyList.Add(playerBody);
            }

            if (playerBody.playerType == Players.P1 || playerBody.playerType == Players.P2)
            {
                numberOfPlayers++;
                playerBodyList.Add(playerBody);
            }
        }
        */
    }
    #endregion

    public void SetGameMode(eGameMode _gameMode)
    {
        switch (_gameMode)
        {
            case eGameMode.SinglePlayer:
                gameMode = _gameMode;
                break;
            case eGameMode.MultiPlayer:
                gameMode = _gameMode;
                break;
            default:
                break;
        }
    }
}

public enum eGameMode
{
    SinglePlayer,
    MultiPlayer
}

public enum Players
{
    P1,
    P2,
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
}

public enum eWeaponHolderOptions
{
    NoWeapon,
    Default,
    Random,
    EntireArm
}

