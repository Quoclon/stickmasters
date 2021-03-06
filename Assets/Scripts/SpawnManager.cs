using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemies Spawned")]
    int enemiesSpawned;

    [Header("Prefabs")]
    public GameObject playerPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Survival Mode")]
    public bool firstSpawnSurvivalMode;

    // Start is called before the first frame update
    void Start()
    {
        RemovePlayersThatShouldHaveBeenSpawned();
        SpawnCharacters();
    }

    void RemovePlayersThatShouldHaveBeenSpawned()
    {
        GameObject[] playersLeftInSceneOnStart = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playersLeftInSceneOnStart)
        {
            Destroy(player);
        }
    }


    public void SpawnCharacters()
    {
        if (GameManager.Inst.gameMode == eGameMode.Survival)
        {
            // Used to track number of enemies and level them up - BodySetup() will access this
            enemiesSpawned++;
            GameManager.Inst.UpdateSpawnedEnemiesStats(enemiesSpawned);

            if (firstSpawnSurvivalMode)
            {
    
                // Use Weapon Choices from MainMenuManager if it is available, otherwise.. Random?
                if (MainMenuManager.Inst != null && MainMenuManager.Inst.playerTypes.Count > 0) 
                {                  
                    if (MainMenuManager.Inst.playerWeaponsLeft[0] == eWeaponType.None && MainMenuManager.Inst.playerWeaponsRight[0] == eWeaponType.None)
                    {
                        // Player
                        GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
                        player1.SetActive(true);
                        player1.GetComponent<Body>().SetupBody(Players.P1);
                    }
                    else
                    {
                        GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
                        player1.SetActive(true);
                        player1.GetComponent<Body>().SetupBody(Players.P1, MainMenuManager.Inst.playerWeaponsLeft[0], MainMenuManager.Inst.playerWeaponsRight[0]);
                    }
                }
                else
                {
                    GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
                    player1.SetActive(true);
                    player1.GetComponent<Body>().SetupBody(Players.P1);
                }
                
                
                // First Spawn Needs to be noted
                firstSpawnSurvivalMode = false;

                // Enemy
                GameObject player2 = Instantiate(playerPrefab, spawnPoints[1].position, Quaternion.identity);
                player2.SetActive(true);
                player2.GetComponent<Body>().SetupBody(Players.AI);
                //Debug.Log("SpawnManager - Survival Mode");

            }
            else
            {
                bool checkingPlayerDistanceFromSpawnPoint = true;
                Vector3 spawnPoint = new Vector3(0, 0, 0);  // default

                while (checkingPlayerDistanceFromSpawnPoint)
                {
                    foreach (var player in GameManager.Inst.playerBodyList)
                    {
                        int rand = Random.Range(0, spawnPoints.Length);
                        spawnPoint = new Vector3(spawnPoints[rand].position.x, spawnPoints[rand].position.y + 1f, 0);
                        
                        float spawnPointdistanceFromAnyPlayer = Vector3.Distance(spawnPoint, player.chest.transform.position);                       
                        //Debug.Log("spawnPointdistanceFromAnyPlayer: " + spawnPointdistanceFromAnyPlayer);
                        if (spawnPointdistanceFromAnyPlayer > 5)
                            checkingPlayerDistanceFromSpawnPoint = false;
                    }
                }
                                   
                GameObject playerNew = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
                playerNew.SetActive(true);
                playerNew.GetComponent<Body>().SetupBody(Players.AI);
                //Debug.Log("SpawnManager - Survival Mode");
            }

            // Ensure these are setup? ~
            GameManager.Inst.isRoundOver = false;
            GameManager.Inst.isMatchOver = false;
        }
        
        if (GameManager.Inst.gameMode == eGameMode.SinglePlayer)
        {
            GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
            GameObject player2 = Instantiate(playerPrefab, spawnPoints[1].position, Quaternion.identity);

            player1.SetActive(true);
            player2.SetActive(true);

            //Debug.Log("SpawnManager - SinglePlayer");
            player1.GetComponent<Body>().SetupBody(Players.P1);
            player2.GetComponent<Body>().SetupBody(Players.AI);
        }

        if (GameManager.Inst.gameMode == eGameMode.MultiPlayer)
        {

            var playerTypes = System.Enum.GetValues(typeof(Players));
            if (MainMenuManager.Inst != null)
            {
                // If there happens to be less than two players
                if (MainMenuManager.Inst.playerTypes.Count < 1)
                {                   
                    GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
                    player1.SetActive(true);
                    player1.GetComponent<Body>().SetupBody(Players.P1);

                    GameObject player2 = Instantiate(playerPrefab, spawnPoints[1].position, Quaternion.identity);
                    player2.SetActive(true);
                    player2.GetComponent<Body>().SetupBody(Players.P2);
                  
                }
                else
                {
                    Debug.Log("MainMenuManager.Inst.playerTypes.Count: " + MainMenuManager.Inst.playerTypes.Count);
                    int x = 0;
                    foreach (var menuListOfPlayerTypes in MainMenuManager.Inst.playerTypes)
                    {
                        //Debug.Log("x: " + x);
                        int i = 0;
                        foreach (var _playerType in playerTypes)
                        {
                            //Debug.Log("i: " + i);
                            if (menuListOfPlayerTypes.ToString() == _playerType.ToString())
                            {                         
                                //if (MainMenuManager.Inst.playerWeaponsLeft.Count > 0 && MainMenuManager.Inst.playerWeaponsRight.Count > 0)
                                if (MainMenuManager.Inst.playerWeaponsLeft[x] == eWeaponType.None && MainMenuManager.Inst.playerWeaponsRight[x] == eWeaponType.None)
                                {
                                    //GameObject player = Instantiate(playerPrefab, spawnPoints[x].position, Quaternion.identity);
                                    //player.SetActive(true);
                                    //player.GetComponent<Body>().SetupBody((Players)_playerType);
                                }
                                else
                                {
                                    GameObject player = Instantiate(playerPrefab, spawnPoints[x].position, Quaternion.identity);
                                    player.SetActive(true);
                                    player.GetComponent<Body>().SetupBody((Players)_playerType, MainMenuManager.Inst.playerWeaponsLeft[x], MainMenuManager.Inst.playerWeaponsRight[x]);
                                }
                            }
                            i++;
                        }
                        x++;
                    }
                }
            }
            else
            {                
                GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
                player1.SetActive(true);
                player1.GetComponent<Body>().SetupBody(Players.P1);

                GameObject player2 = Instantiate(playerPrefab, spawnPoints[1].position, Quaternion.identity);
                player2.SetActive(true);
                player2.GetComponent<Body>().SetupBody(Players.P2);
            }



            /*
            GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
            GameObject player2 = Instantiate(playerPrefab, spawnPoints[1].position, Quaternion.identity);

            player1.SetActive(true);
            player2.SetActive(true);

            //Debug.Log("SpawnManager - Multiplayer");
            player1.GetComponent<Body>().SetupBody(Players.P1);
            player2.GetComponent<Body>().SetupBody(Players.P2);
            */
        }

        if (GameManager.Inst.gameMode == eGameMode.Coop)
        {
            GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
            GameObject player2 = Instantiate(playerPrefab, spawnPoints[1].position, Quaternion.identity);
            GameObject player3 = Instantiate(playerPrefab, spawnPoints[2].position, Quaternion.identity);
            GameObject player4 = Instantiate(playerPrefab, spawnPoints[3].position, Quaternion.identity);

            player1.SetActive(true);
            player2.SetActive(true);
            player3.SetActive(true);
            player4.SetActive(true);

            //Debug.Log("SpawnManager - Multiplayer");
            player1.GetComponent<Body>().SetupBody(Players.P1);
            player2.GetComponent<Body>().SetupBody(Players.P2);
            player3.GetComponent<Body>().SetupBody(Players.AI);
            player4.GetComponent<Body>().SetupBody(Players.AI);
        }
    }
}
