using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Spawnpoints")]
    public Transform[] spawnPoints;

    [Header("Player Types")]
    public Players[] playerTypeOptions  = new Players[] { Players.P1, Players.P2, Players.P3, Players.P4, Players.AI };
    public List<Players> playerTypesAvailable;

    [Header("Character Selectors")]
    public List<CharacterSelect> characterSelectors;

    private void OnEnable()
    {
        // All PlayerTypes Available at first (OnEnable)
        if (playerTypesAvailable != null)
            playerTypesAvailable.Clear();

        for (int i = 0; i < playerTypeOptions.Length; i++)
        {
            Debug.Log(playerTypeOptions[i]);      
            playerTypesAvailable.Add(playerTypeOptions[i]); // Including AI, but AI never 'adds' or 'resmove' from this list later
        }

        SpawnPlayerSelectorUI();
    }

    public void AddToAvailablePlayerTypes(Players _playerType)
    {
        Debug.Log("AddToAvailablePlayerTypes: " + _playerType);
        if (_playerType != Players.AI)
            playerTypesAvailable.Add(_playerType);
    }

    public void RemoveFromAvailablePlayerTypes(Players _playerType)
    {
        Debug.Log("RemoveFromAvailablePlayerTypes: " + _playerType);
        if(_playerType != Players.AI)
            playerTypesAvailable.Remove(_playerType);
    }

    public Players GetAvailablePlayerType()
    {
        // Ensures Ordered Players Return
        foreach (var _playerOption in playerTypeOptions)
        {
            foreach (var _playerType in playerTypesAvailable)
            {
                if(_playerOption == _playerType)
                    return _playerType;
            }
        }
       

        //Default
        return Players.AI;
    }

    public Players GetNextPlayerType(Players currentPlayerType)
    {
        Debug.Log("current playerType: " + currentPlayerType);
        
        Players _playerTypeToSelect = currentPlayerType;
        
        
        int startingPoint = 0;
        for (int j = 0; j < playerTypeOptions.Length; j++)
        {
            if (currentPlayerType == playerTypeOptions[j])
                startingPoint = j;
        }

        Debug.Log("Starting Point inPlayerOptions Array: " + startingPoint);

        int loopStartingPoint = startingPoint + 1;
        if (loopStartingPoint >= playerTypeOptions.Length)
            loopStartingPoint = 0;

        Debug.Log("Looping Starting Point: " + loopStartingPoint);
        Debug.Log("playerTypeOptions.Length: " + playerTypeOptions.Length);

        for (int i = loopStartingPoint; i < playerTypeOptions.Length; i++)
        {
            if (i == startingPoint)
            {
                Debug.Log("i == startingPoint");
                return _playerTypeToSelect;
            }

            for (int x = 0; x < playerTypesAvailable.Count; x++)
            {

                if ((playerTypeOptions[i] == playerTypesAvailable[x]) && (playerTypeOptions[i] != currentPlayerType))
                {

                    // Next Item in the List is playerTypeOptions[i]
                    if (currentPlayerType == Players.AI)
                    {
                        playerTypesAvailable.Remove(playerTypeOptions[i]);
                        //playerTypesAvailable.Add(currentPlayerType);
                    }
                    else
                    {
                        //playerTypesAvailable.Remove(playerTypeOptions[i]);
                        playerTypesAvailable.Add(currentPlayerType);
                        if (playerTypeOptions[i] != Players.AI)
                            playerTypesAvailable.Remove(playerTypeOptions[i]);
                    }

                    return playerTypeOptions[i];
                }

            }

            Debug.Log("Loop (i): " + i +" playerTypeOptions[i]: " + playerTypeOptions[i]);
            if (i == playerTypeOptions.Length-1)
                i = -1; //i++ runs before next loop
        }

        /*
        int i = loopStartingPoint;
        while(i != startingPoint)
        {
            if(i >= playerTypeOptions.Length)
            {
                Debug.Log("Reset i to Zero");
                i = 0;
            }
            else
            {
                Debug.Log("i: " + i);
            }

            i++;
        }
        */


        /*
        for (int i = loopStartingPoint; i < playerTypeOptions.Length; i++)
        {
            //Debug.Log("i:" + i);
            if (i == startingPoint)
            {
                Debug.Log("Loop Done");
                return currentPlayerType;
            }

            for (int x = 0; x < playerTypesAvailable.Count; x++)
            {
                
                if (playerTypeOptions[i] == playerTypesAvailable[x] && playerTypeOptions[i] != currentPlayerType)
                {
                    playerTypesAvailable.Remove(playerTypesAvailable[x]);
                    playerTypesAvailable.Add(currentPlayerType);
                    return playerTypeOptions[i];
                }
                
            }

            if (i + 1 >= playerTypeOptions.Length)
                i = -1; // because i++ runs before next loop
        }
        */
        

        return currentPlayerType;
    }

    public Players GetNextPlayerType_1(Players currentPlayerType)
    {
        Debug.Log("GetNextPlayerType: " + currentPlayerType);
        Players _playerTypeToSelect = currentPlayerType;
        foreach (var playerType in playerTypesAvailable)
        {
            if(playerType != currentPlayerType)
                return playerType;
        }

        return _playerTypeToSelect;
    }


        public void GetNextPlayerType_OLD(Players currentPlayerType)
    {
        int startingPoint = 0;
        for (int i = 0; i < playerTypesAvailable.Count; i++)
        {
            if (currentPlayerType == playerTypesAvailable[i])
                startingPoint = i;
        }

        int x = startingPoint + 1;
        while(x != startingPoint)
        {
            if (x > playerTypesAvailable.Count)
                x = 0;

            for (int j = 0; j < playerTypesAvailable.Count; j++)
            {
                //if(playerTypesAvailable[j] == )
            }

            x++;
        }

        foreach (var characterSelector in characterSelectors)
        {
            
        }
    }

    void SpawnPlayerSelectorUI()
    {
        for (int playerNum = 0; playerNum < characterSelectors.Count; playerNum++)
        {
            characterSelectors[playerNum].gameObject.SetActive(true);
            //characterSelectors[playerNum].SpawnPlayerAndUI();
        }
    }

void SpawnPlayersForSelection()
    {
        for (int playerNum = 0; playerNum < characterSelectors.Count; playerNum++)
        {
            //characterSelectors[playerNum].SpawnPlayer(playerPrefab, playerPlatformPrefab, playerNum, canvasObject);
        }
    }
}
