using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    //~ TODO:
    // Jump Force coming from body (or upper legs) or change responsibility based on status
    // Add "Effectors" like crazy force pushes
    // Create great environments that can be damaged, and cause damge

    // IDEAS:
    // New "Poses" for players, less gravity on legs, etc.
    // blood spray when parts come off
    // Improve Slow Time on Hit


    // TODO - GameManager
    // Setup Spawning, add to player lists, npc lists, etc.


    #region Singleton
    // Singleton instance.
    //public static GameManager Inst = null;
    static GameManager _instance;
    public static GameManager Inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("SingletonWellKnownName").GetComponent<GameManager>();
            }
            return _instance;
        }
    }

    // Initialize the singleton instance.
    private void Awake()
    {

        // If there is not already an instance of SoundManager, set it to this.

        _instance = this;
        
        //If an instance already exists, destroy whatever this object is to enforce the singleton.

        /*
        else if (Inst != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);

        //cinemachineTargetGroup = FindObjectOfType<CinemachineTargetGroup>();

        Debug.Log("OnAwake");
        */
    }
    #endregion

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


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ResetScene();
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
            int playerCameraTargetWeight = 4;
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

        //Debug.Log("Pre Remove - npcBodyList.Count " + npcBodyList.Count);
        //Debug.Log("Pre Remove -  playerBodyList.Count " + playerBodyList.Count);

        RemovePlayerFromList(bodyKilled, playerKilled);

        //Debug.Log("Post Remove - npcBodyList.Count " + npcBodyList.Count);
        //Debug.Log("Post Remove - playerBodyList.Count " + playerBodyList.Count);

        if (npcBodyList.Count <= 0)
            GameOver(playerDealingLastBlow);

        else if (playerBodyList.Count <= 0)
            GameOver(playerDealingLastBlow);
         
    }

    public void GameOver(Players playerDealingLastBlow)
    {
        Debug.Log("Game Over - Player Wins: " + playerDealingLastBlow.ToString());

        if (playerDealingLastBlow == Players.P1) gameOverText.text = "Player 1 Wins!";
        if (playerDealingLastBlow == Players.P2) gameOverText.text = "Player 2 Wins!";
        if (playerDealingLastBlow == Players.AI) gameOverText.text = "NPC Wins!";

        // ~ TODO: Set Canvas after a few seconds using Coroutine
        //Debug.Log("Game Over Ran");
        //canvasGameOver.SetActive(true);
        StartCoroutine(WaitForGammeOverCorourtine(2));
        isGameOver = true;
        playerWinner = playerDealingLastBlow;
        DisablePlayers();
    }

    void DisablePlayers()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(player.GetComponent<Movement>().playerType != playerWinner)
            {
                foreach (var weapon in player.GetComponentsInChildren<Weapon>())
                {
                    weapon.DisableWeapon();
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
    LowerRightLeg
}
