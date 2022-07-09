using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Canvas for Spawning UI")]
    public GameObject canvasObject;

    // TODO: These will each be on their own "Panel" for each player
    [Header("Player Select Prefabs")]
    public GameObject playerPrefab;
    public GameObject playerPlatformPrefab;

    [Header("Spawned Objects")]
    public List<Body> playerBodies;
    public List<GameObject> playerPlatforms;

    [Header("Spawnpoints")]
    public Transform[] spawnPoints;

    [Header("Character Selectors")]
    public List<CharacterSelect> characterSelectors;
    private int[] playerOrderOnPlatforms = new int[] { 2, 0, 1, 3 };    // ?????

    private void OnEnable()
    {
        SpawnPlayersForSelection();
    }

    void SpawnPlayersForSelection()
    {
        for (int playerNum = 0; playerNum < characterSelectors.Count; playerNum++)
        {
            characterSelectors[playerNum].SpawnPlayer(playerPrefab, playerPlatformPrefab, playerNum, canvasObject);
        }
    }
}
