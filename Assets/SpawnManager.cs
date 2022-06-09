using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject npcPrefab;

    public Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        RemovePlayersThatShouldHaveBeenSpawned();
        SpawnCharacters();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RemovePlayersThatShouldHaveBeenSpawned()
    {
        GameObject[] playersLeftInSceneOnStart = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playersLeftInSceneOnStart)
        {
            Destroy(player);
        }
    }

    void SpawnCharacters()
    {
        GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
        GameObject player2 = Instantiate(playerPrefab, spawnPoints[1].position, Quaternion.identity);

        if(GameManager.Inst.gameMode == eGameMode.SinglePlayer)
        {
            Debug.Log("SpawnManager - SinglePlayer");
            player1.GetComponent<Body>().SetupBody(Players.P1);
            player2.GetComponent<Body>().SetupBody(Players.AI);
        }

        if (GameManager.Inst.gameMode == eGameMode.MultiPlayer)
        {
            Debug.Log("SpawnManager - Multiplayer");
            player1.GetComponent<Body>().SetupBody(Players.P1);
            player2.GetComponent<Body>().SetupBody(Players.P2);
        }



    }
}
