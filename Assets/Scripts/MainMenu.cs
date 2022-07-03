using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public WeaponArm[] weaponArmsLeft;
    public WeaponArm[] weaponArmsRight;

    public TextMeshProUGUI[] weaponNames;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWeaponArms(WeaponArm[] _weaponArms, eWeaponArms armName)
    {
        if(armName == eWeaponArms.Left)
        {
            weaponArmsLeft = _weaponArms;
        }

        if (armName == eWeaponArms.Right)
        {
            weaponArmsRight = _weaponArms;
        }


    }
    public void SetSinglePlayerMode()
    {
        MainMenuManager.Inst.gameMode = eGameMode.SinglePlayer;
        MainMenuManager.Inst.LoadScene(1);
    }

    public void SetSinglePlayerSurvivalMode()
    {
        MainMenuManager.Inst.gameMode = eGameMode.Survival;
        MainMenuManager.Inst.LoadScene(1);
    }

    public void SetMultiplayerPlayerMode()
    {
        MainMenuManager.Inst.gameMode = eGameMode.MultiPlayer;
        MainMenuManager.Inst.LoadScene(1);
    }


    public void SetCoopPlayerMode()
    {
        MainMenuManager.Inst.gameMode = eGameMode.Coop;
        MainMenuManager.Inst.LoadScene(1);
    }
}
