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
                // Player
                GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
                player1.SetActive(true);
                player1.GetComponent<Body>().SetupBody(Players.P1);
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
            GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
            GameObject player2 = Instantiate(playerPrefab, spawnPoints[1].position, Quaternion.identity);

            player1.SetActive(true);
            player2.SetActive(true);

            //Debug.Log("SpawnManager - Multiplayer");
            player1.GetComponent<Body>().SetupBody(Players.P1);
            player2.GetComponent<Body>().SetupBody(Players.P2);
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
