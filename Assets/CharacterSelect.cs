using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    // TODO: These will each be on their own "Panel" for each player
    [Header("Player Select Prefabs")]
    public GameObject playerPrefab;
    public GameObject playerPlatformPrefab;

    [Header("Spawned Objects")]
    public List<Body> playerBodies;
    public List<GameObject> playerPlatforms;

    [Header("Spawnpoints")]
    public Transform[] spawnPoints;

    [Header("Weapon Lists")]
    public List<eWeaponType> leftWeapons;
    public int leftWeaponsIndex = 0;
    public List<eWeaponType> rightWeapons;
    public int rightWeaponsIndex = 0;

    [Header("Weapon Selections")]
    public eWeaponType leftArmWeapon;
    public eWeaponType rightArmWeapon;

    [Header("Character Selection Manager")]
    public CharacterSelectionManager characterSelectionManager;

    [Header("Player Type")]
    public Players playerType;

    private void OnEnable()
    {
        //SpawnPlayersForSelection();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void AddSpawnedPlayer(Players _playerType, Body _playerBody, GameObject _platform)
    {
        playerType = _playerType;
        playerBodies.Add(_playerBody);
        playerPlatforms.Add(_platform);
        SetupWeapons();
    }

    void SpawnPlayersForSelection()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            //SpawnPlayer(i);
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

    public void SpawnPlayer(int playerNumber)
    {
        Players playerTypeToSpawn = GetPlayerNumber(playerNumber);

        // Instantiate the player
        GameObject player = Instantiate(playerPrefab, spawnPoints[playerNumber].position, Quaternion.identity);
        player.SetActive(true);

        // Instantiate the 'platform' (later a cage, or platform, etc.) - could not do preset platforms in WebGL build
        GameObject platform = Instantiate(playerPlatformPrefab, new Vector3(player.transform.position.x, playerPlatformPrefab.transform.position.y, 0), Quaternion.identity);

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

        //playerBodies.Add(playerBody);
        //playerPlatforms.Add(platform);
    }

    public void SetupWeapons()
    {

        // Setup List of Player Left/Right Weapons on Arms/Weapons in Player Prefab
        foreach (var weaponHandler in playerBodies[0].weaponHandlers)
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
    }
  
    void Update()
    {
        // ~TODO: Move this to it's own script that sits on the player or platform
        // Spawning Player/Platform shiould be it's own script
        // ~TODO: Change player type, change player weapons, change player health, change color?
        WeaponSelect(); 
    }

    void WeaponSelect()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            leftWeaponsIndex++;
            if (leftWeaponsIndex > leftWeapons.Count - 1)
                leftWeaponsIndex = 0;

            leftArmWeapon = leftWeapons[leftWeaponsIndex];
            playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftWeaponsIndex--;
            if (leftWeaponsIndex < 0)
                leftWeaponsIndex = leftWeapons.Count - 1;

            leftArmWeapon = leftWeapons[leftWeaponsIndex];
            playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rightWeaponsIndex++;
            if (rightWeaponsIndex > rightWeapons.Count - 1)
                rightWeaponsIndex = 0;

            rightArmWeapon = rightWeapons[rightWeaponsIndex];
            playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            rightWeaponsIndex--;
            if (rightWeaponsIndex < 0)
                rightWeaponsIndex = rightWeapons.Count - 1;

            rightArmWeapon = rightWeapons[rightWeaponsIndex];
            playerBodies[0].SetupBody(playerType, leftArmWeapon, rightArmWeapon);
        }

    }
}
