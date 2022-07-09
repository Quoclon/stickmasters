using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    // TODO: These will each be on their own "Panel" for each player
    [Header("Player Select Prefabs")]
    //public GameObject playerPrefab;
    //public GameObject playerPlatformPrefab;

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
    public Players playerType;

    [Header("Input Type")]
    public string playerNumberForInput;
    private float inputDebounceTimer;
    private float inputDebounceTimerMax;

    private void OnEnable()
    {
        inputDebounceTimerMax = 0.15f;
        inputDebounceTimer = inputDebounceTimerMax;
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

    public void SpawnPlayer(GameObject playerPrefab, GameObject playerPlatformPrefab, int playerNumber, GameObject canvasToAttachUI)
    {
        //Debug.Log(playerNumber);
        Players playerTypeToSpawn = GetPlayerNumber(playerNumber);

        // Instantiate the player
        //GameObject player = Instantiate(playerPrefab, spawnPoints[playerNumber].position, Quaternion.identity);
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        player.SetActive(true);

        // Instantiate the 'platform' (later a cage, or platform, etc.) - could not do preset platforms in WebGL build
        GameObject platform = Instantiate(playerPlatformPrefab, new Vector3(player.transform.position.x, playerPlatformPrefab.transform.position.y, 0), Quaternion.identity);
        //platform.transform.SetParent(this.transform);

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


    void Update()
    {
        WeaponSelect();
    }

    void WeaponSelect()
    {
        inputDebounceTimer -= Time.deltaTime;
        if (inputDebounceTimer > 0)
            return;
      
        float xInput = Input.GetAxisRaw("Horizontal" + playerNumberForInput);
        float yInput = Input.GetAxisRaw("Vertical" + playerNumberForInput);

        if(xInput > 0)
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

    void LeftWeaponNext()
    {
        leftWeaponsIndex++;
        if (leftWeaponsIndex > leftWeapons.Count - 1)
            leftWeaponsIndex = 0;

        leftArmWeapon = leftWeapons[leftWeaponsIndex];
        playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
    }

    void LeftWeaponPrevious()
    {
        leftWeaponsIndex--;
        if (leftWeaponsIndex < 0)
            leftWeaponsIndex = leftWeapons.Count - 1;

        leftArmWeapon = leftWeapons[leftWeaponsIndex];
        playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
    }

    void RightWeaponNext()
    {
        rightWeaponsIndex++;
        if (rightWeaponsIndex > rightWeapons.Count - 1)
            rightWeaponsIndex = 0;

        rightArmWeapon = rightWeapons[rightWeaponsIndex];
        playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
    }

    void RightWeaponPrevious()
    {
        rightWeaponsIndex--;
        if (rightWeaponsIndex < 0)
            rightWeaponsIndex = rightWeapons.Count - 1;

        rightArmWeapon = rightWeapons[rightWeaponsIndex];
        playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
    }
}
