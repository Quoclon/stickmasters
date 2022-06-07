using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class GameManager : MonoBehaviour
{
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

    [Header("Canvas")]
    public GameObject canvasGameOver;
    public TextMeshProUGUI gameOverText;

    [Header("Game Over")]
    public bool isGameOver;
    public Players playerWinner;

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
            //variableJoystickP1.gameObject.SetActive(false);
            //variableJoystickP2.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ResetScene();
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
        Debug.Log("Game Over - Player Wins: " + playerDealingLastBlow.ToString());

        if (playerDealingLastBlow == Players.P1) gameOverText.text = "Player 1 Wins!";
        if (playerDealingLastBlow == Players.P2) gameOverText.text = "Player 2 Wins!";
        if (playerDealingLastBlow == Players.AI) gameOverText.text = "NPC Wins!";

        // ~ TODO: Set Canvas after a few seconds using Coroutine
        StartCoroutine(WaitForGammeOverCorourtine(2));

        isGameOver = true;
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
                //playerMovement.dis

                foreach (var weapon in player.GetComponentsInChildren<Weapon>())
                {
                    weapon.weaponDisabled = true;
                    weapon.gameObject.layer = 8;
                    //weapon.DisableWeapon();
                } 
            }
        }
    }


    // ~ TODO: Couldn't figure out Awake, Enable, Start order with rest of scripts -- So Variables for next round itialized here
    public void ResetScene()
    {
        isGameOver = false;
        canvasGameOver.SetActive(false);

        Time.timeScale = 1;
        npcBodyList.Clear();
        playerBodyList.Clear();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        isGameOver = false;
        canvasGameOver.SetActive(false);

        Time.timeScale = 1;
        npcBodyList.Clear();
        playerBodyList.Clear();

        SceneManager.LoadScene(0);
    }

    private IEnumerator WaitForGammeOverCorourtine(float waitTime)
    {
        // Wait until running -- do nothing first
        yield return new WaitForSeconds(waitTime);
        if(isGameOver)
            canvasGameOver.SetActive(true);
    }


    #region Archive
    void SetupBodiesInScene()
    {
        GameObject[] allPlayersAndNpcs = GameObject.FindGameObjectsWithTag("Player");

        Debug.Log("allPlayersAndNpcs.Count: " + allPlayersAndNpcs.Length);

        foreach (var player in allPlayersAndNpcs)
        {
            Body playerBody = player.GetComponent<Body>();
            playerBody.SetupBody();
        }
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
    }
    #endregion

    public void SetGameMode(eGameMode _gameMode)
    {
        Debug.Log(gameMode);
        Debug.Log(_gameMode);

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

        Debug.Log(gameMode);
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

