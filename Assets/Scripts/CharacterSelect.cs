using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CharacterSelect : MonoBehaviour
{
    [Header("Charater Selection Manager")]
    public CharacterSelectionManager characterSelectionManager;

    // TODO: These will each be on their own "Panel" for each player
    [Header("Player Select Prefabs")]
    public GameObject playerPrefab;
    public GameObject playerPlatformPrefab;

    [Header("Spawned Objects")]
    public List<Body> playerBodies;
    public List<GameObject> playerPlatforms;

    [Header("Spawnpoints")]
    //public Transform[] spawnPoints;
    public Transform spawnPoint;

    [Header("Weapon Lists")]
    public List<eWeaponType> leftWeapons;
    public int leftWeaponsIndex = 0;
    public List<eWeaponType> rightWeapons;
    public int rightWeaponsIndex = 0;

    [Header("Weapon Selections")]
    public eWeaponType leftArmWeapon;
    public eWeaponType rightArmWeapon;

    [Header("Player Type")]
    public int playerNumber;
    public Players playerType;
    public TextMeshProUGUI playerTypeName;

    [Header("Input Type")]
    public string playerNumberForInput;
    private float inputDebounceTimer;
    private float inputDebounceTimerMax;

    [Header("UI Buttons")]
    public Button[] uiButtons;

    private void OnEnable()
    {
        //characterSelectionManager = GameObject.FindObjectOfType<CharacterSelectionManager>();
        inputDebounceTimerMax = 0.15f;
        inputDebounceTimer = inputDebounceTimerMax;

        // ~ TEST
        playerTypeName.text = "Click";
        DisableSelectorButtons();

        //playerTypeName.text = playerType.ToString();
    }

    public void NextPlayerType()
    {
        Players currentPlayerType = playerType;
        playerType = characterSelectionManager.GetNextPlayerType(playerType);
        
        if(playerType != currentPlayerType)
        {
            Destroy(playerBodies[0].gameObject);
            playerBodies.Clear();
            playerTypeName.text = playerType.ToString();
            SpawnPlayer();
        }
           
    }

    Players GetPlayerNumber(int playerNumber)
    {
        Players playerType = Players.P1;
        var playerTypes = System.Enum.GetValues(typeof(Players));

        // Loop through Enum of player types
        int i = 0;
        foreach (var _playerType in playerTypes)
        {
            if(i == playerNumber)
            {
                playerType = (Players)_playerType;
            }

            i++;
        }
        return playerType;
    }

    public void ToggleSelectorButtons()
    {
        if (playerBodies.Count > 0)
        {
            Destroy(playerBodies[0].gameObject);
            playerBodies.Clear();
            playerTypeName.text = "Click";
            if (playerType != Players.AI)
                characterSelectionManager.AddToAvailablePlayerTypes(playerType);
        }
        else
        {
            //characterSelectionManager.AddChosenPlayerToPlayerTypesAvailableList(playerType);
            //characterSelectionManager.AddToAvailablePlayerTypes(playerType);
            playerType = characterSelectionManager.GetAvailablePlayerType();
            playerTypeName.text = playerType.ToString();
            SpawnPlayer();
            if(playerType != Players.AI)
                characterSelectionManager.RemoveFromAvailablePlayerTypes(playerType);
        }

        foreach (var button in uiButtons)
        {
            if (button.gameObject.activeInHierarchy)
            {
                button.gameObject.SetActive(false);
            }
            else
            {
                button.gameObject.SetActive(true);
            }
        }
    }

    public void EnableSelectorButtons()
    {
        ToggleSelectorButtons();
        /*
        if(playerBodies.Count == 0)
            SpawnPlayer();

        foreach (var button in uiButtons)
        {
            button.gameObject.SetActive(true);
        }
        */
    }

    public void DisableSelectorButtons()
    {
        foreach (var button in uiButtons)
        {
            button.gameObject.SetActive(false);
        }
    }


    public void SpawnPlayer()
    {
        //Players playerTypeToSpawn = GetPlayerNumber(playerNumber);

        // Instantiate the player
        GameObject player = Instantiate(playerPrefab, transform.position + new Vector3(0f, 4.2f, 0), Quaternion.identity);
        player.SetActive(true);

        // Setup the 'Body' of the player; defaulting to no weapons
        Body playerBody = player.GetComponent<Body>();
        playerBody.SetupBody(playerType, eWeaponType.None, eWeaponType.None);

        // Setup List of Player Left/Right Weapons on Arms/Weapons in Player Prefab
        foreach (var weaponHandler in playerBody.weaponHandlers)
        {
            foreach (var weaponArm in weaponHandler.weaponArms)
            {
                if (weaponHandler.armType == eWeaponArms.Left)
                    leftWeapons.Add(weaponArm.weaponType);

                if (weaponHandler.armType == eWeaponArms.Right)
                    rightWeapons.Add(weaponArm.weaponType);
            }
        }

        for (int i = 0; i < leftWeapons.Count; i++)
        {
            if (leftWeapons[i] == eWeaponType.None)
            {
                leftWeaponsIndex = i;
                leftArmWeapon = leftWeapons[leftWeaponsIndex];
            }

        }

        for (int i = 0; i < rightWeapons.Count; i++)
        {
            if (rightWeapons[i] == eWeaponType.None)
            {
                rightWeaponsIndex = i;
                rightArmWeapon = rightWeapons[rightWeaponsIndex];
            }
        }

        playerBodies.Add(playerBody);
        //playerPlatforms.Add(platform);
    }


    /*
    public void SpawnPlayer(GameObject playerPrefab, GameObject playerPlatformPrefab, int playerNumber, GameObject canvasToAttachUI)
    {
        //Debug.Log(playerNumber);
        Players playerTypeToSpawn = GetPlayerNumber(playerNumber);

        // Instantiate the player
        //GameObject player = Instantiate(playerPrefab, spawnPoints[playerNumber].position, Quaternion.identity);
        GameObject player = Instantiate(playerPrefab, transform.position + new Vector3(0f, 4.2f, 0), Quaternion.identity);
        player.SetActive(true);

        // [~NOTE: Built into CharacterSelect now] Instantiate the 'platform' (later a cage, or platform, etc.) - could not do preset platforms in WebGL build
        //GameObject platform = Instantiate(playerPlatformPrefab, new Vector3(player.transform.position.x, playerPlatformPrefab.transform.position.y, 0), Quaternion.identity);

        // Setup the 'Body' of the player; defaulting to no weapons
        Body playerBody = player.GetComponent<Body>();
        playerBody.SetupBody(playerTypeToSpawn, eWeaponType.None, eWeaponType.None);


        // Setup List of Player Left/Right Weapons on Arms/Weapons in Player Prefab
        foreach (var weaponHandler in playerBody.weaponHandlers)
        {
            foreach (var weaponArm in weaponHandler.weaponArms)
            {
                if (weaponHandler.armType == eWeaponArms.Left)
                    leftWeapons.Add(weaponArm.weaponType);

                if (weaponHandler.armType == eWeaponArms.Right)
                    rightWeapons.Add(weaponArm.weaponType);
            }
        }

        for (int i = 0; i < leftWeapons.Count; i++)
        {
            if (leftWeapons[i] == eWeaponType.None)
            {
                leftWeaponsIndex = i;
                leftArmWeapon = leftWeapons[leftWeaponsIndex];
            }

        }

        for (int i = 0; i < rightWeapons.Count; i++)
        {
            if (rightWeapons[i] == eWeaponType.None)
            {
                rightWeaponsIndex = i;
                rightArmWeapon = rightWeapons[rightWeaponsIndex];
            }
        }

        playerBodies.Add(playerBody);
        //playerPlatforms.Add(platform);
    }
    */


    void Update()
    {
        WeaponSelect();
    }

    void WeaponSelect()
    {
        inputDebounceTimer -= Time.deltaTime;
        if (inputDebounceTimer > 0)
            return;
     
        // OLD INPUT APPROACH
        //float xInput = Input.GetAxisRaw("Horizontal" + playerNumberForInput);
        //float yInput = Input.GetAxisRaw("Vertical" + playerNumberForInput);

        // NEW INPUT APPROACH
        float xInput = 0;
        float yInput = 0;

        //Debug.Log("value.Get<Vector2>().x " + value.Get<Vector2>().x);
        //Debug.Log("value.Get<Vector2>().y " + value.Get<Vector2>().y);

        if (playerBodies.Count == 0)
            return;

        int playerNumber = -1;
        switch (playerType)
        {
            case Players.P1:
                playerNumber = 0;
                break;
            case Players.P2:
                playerNumber = 1;
                break;
            case Players.P3:
                playerNumber = 2;
                break;
            case Players.P4:
                playerNumber = 3;
                break;
            case Players.AI:
                playerNumber = 4;
                break;
            case Players.Environment:
                break;
            default:
                break;
        }

        //Debug.Log(playerNumber);

        if (playerNumber < 0 || playerNumber > Gamepad.all.Count-1)
            return;

        xInput = Mathf.RoundToInt(Gamepad.all[playerNumber].leftStick.ReadValue().x);
        yInput = Mathf.RoundToInt(Gamepad.all[playerNumber].leftStick.ReadValue().y);

        if (xInput > 0)
        {
            LeftWeaponNext();
            inputDebounceTimer = inputDebounceTimerMax; //Reset Timer
        }

        if (xInput < 0)
        {
            LeftWeaponPrevious();
            inputDebounceTimer = inputDebounceTimerMax; //Reset Timer
        }

        if (yInput > 0)
        {
            RightWeaponNext();
            inputDebounceTimer = inputDebounceTimerMax; //Reset Timer
        }

        if (yInput < 0)
        {
            RightWeaponPrevious();
            inputDebounceTimer = inputDebounceTimerMax; //Reset Timer
        }
    }

    public void LeftWeaponNext()
    {
        //Debug.Log("LeftWeaponNext");
        leftWeaponsIndex++;
        if (leftWeaponsIndex > leftWeapons.Count - 1)
            leftWeaponsIndex = 0;

        leftArmWeapon = leftWeapons[leftWeaponsIndex];
        playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
    }

    public void LeftWeaponPrevious()
    {
        leftWeaponsIndex--;
        if (leftWeaponsIndex < 0)
            leftWeaponsIndex = leftWeapons.Count - 1;

        leftArmWeapon = leftWeapons[leftWeaponsIndex];  
        playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
    }

    public void RightWeaponNext()
    {
        rightWeaponsIndex++;
        if (rightWeaponsIndex > rightWeapons.Count - 1)
            rightWeaponsIndex = 0;

        rightArmWeapon = rightWeapons[rightWeaponsIndex];
        playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
    }

    public void RightWeaponPrevious()
    {
        rightWeaponsIndex--;
        if (rightWeaponsIndex < 0)
            rightWeaponsIndex = rightWeapons.Count - 1;

        rightArmWeapon = rightWeapons[rightWeaponsIndex];
        playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
    }
}
