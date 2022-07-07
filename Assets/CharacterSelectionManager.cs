using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
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

    [Header("Character Selectors")]
    public List<CharacterSelect> characterSelectors;
    private int[] playerOrderOnPlatforms = new int[] { 2, 0, 1, 3 };

    private void OnEnable()
    {
        SpawnPlayersForSelection();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnPlayersForSelection()
    {

        //for (int i = 0; i < playerOrderOnPlatforms.Length; i++)
        for (int i = 0; i < characterSelectors.Count; i++)
        {
            characterSelectors[i].SpawnPlayer(i);
            //characterSelectors[i].SpawnPlayer(i, spawnPoints[playerOrderOnPlatforms[i]].transform);
        }
    }





    /*
    Players GetPlayerNumber(int playerNumber)
    {
        Players playerType = Players.P1;

        var playerTypes = System.Enum.GetValues(typeof(Players));

        // Loop through Enum of player types
        int i = 0;
        foreach (var _playerType in playerTypes)
        {
            if (i == playerNumber)
            {
                playerType = (Players)_playerType;
            }

            i++;
        }

        return playerType;
    }

    public void SpawnPlayer(int playerNumber)
    {
        Players _playerType = GetPlayerNumber(playerNumber);

        // Instantiate the player
        // GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        GameObject player = Instantiate(playerPrefab, spawnPoints[playerNumber].position, Quaternion.identity);
        player.SetActive(true);

        // Instantiate the 'platform' (later a cage, or platform, etc.) - could not do preset platforms in WebGL build
        GameObject platform = Instantiate(playerPlatformPrefab, new Vector3(player.transform.position.x, playerPlatformPrefab.transform.position.y, 0), Quaternion.identity);

        // Setup the 'Body' of the player; defaulting to no weapons
        Body playerBody = player.GetComponent<Body>();
        playerBody.SetupBody(_playerType, eWeaponType.None, eWeaponType.None);

        characterSelectors[playerNumber].AddSpawnedPlayer(_playerType, playerBody, platform);

        
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
        

        // ~ Use this LATER??
        //playerBodies.Add(playerBody);
        //playerPlatforms.Add(platform);
    }
    */
}
