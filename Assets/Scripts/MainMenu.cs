using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetSinglePlayerMode()
    {
        MainMenuManager.Inst.gameMode = eGameMode.SinglePlayer;
        MainMenuManager.Inst.LoadScene(1);
    }

    public void SetMultiplayerPlayerMode()
    {
        MainMenuManager.Inst.gameMode = eGameMode.MultiPlayer;
        MainMenuManager.Inst.LoadScene(1);
    }
}
