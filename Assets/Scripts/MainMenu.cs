using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject panelMainMenu;
    public GameObject panelPlayerSelect;

    [Header("Weapons Selection")]
    public WeaponArm[] weaponArmsLeft;
    public WeaponArm[] weaponArmsRight;
    public TextMeshProUGUI[] weaponNames;

    [Header("Weapons Selected")]
    //public List<Body> playerBodies;
    public List<eWeaponType> playerWeaponsLeft;
    public List<eWeaponType> playerWeaponsRight;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePlayerSelectPanel()
    {
        panelPlayerSelect.SetActive(true);
        panelMainMenu.SetActive(false);
    }

    public void DisablePlayerSelectPanel()
    {
        Body[] playersInMenuScene = GameObject.FindObjectsOfType<Body>();
        GameObject[] platformsInMenuScene = GameObject.FindGameObjectsWithTag("Ground");

        foreach (var playerBody in playersInMenuScene)
        {
            Destroy(playerBody.gameObject);
        }

        foreach (var platform in platformsInMenuScene)
        {
            Destroy(platform.gameObject);
        }

        // Clear all the Lists, since we are going back to MainMenu. Will respawn Bodies and weapon choices
        CharacterSelectionManager _characterSelectionManager = panelPlayerSelect.GetComponent<CharacterSelectionManager>();
        _characterSelectionManager.playerBodies.Clear();
        _characterSelectionManager.playerPlatforms.Clear();

        foreach (var characterSelector in _characterSelectionManager.characterSelectors)
        {
            characterSelector.playerBodies.Clear();
            characterSelector.playerPlatforms.Clear();
        }

        //CharacterSelect _characterSelect = panelPlayerSelect.GetComponent<CharacterSelect>();
        //panelPlayerSelect.GetComponent<CharacterSelect>().playerBodies.Clear();
        //panelPlayerSelect.GetComponent<CharacterSelect>().playerPlatforms.Clear();
        MainMenuManager.Inst.playerWeaponsLeft.Clear();
        MainMenuManager.Inst.playerWeaponsRight.Clear();
        
        panelPlayerSelect.SetActive(false);
        panelMainMenu.SetActive(true);
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

    public void SetSinglePlayerSurvivalModeCustom()
    {
        AssignWeapons();
        MainMenuManager.Inst.gameMode = eGameMode.Survival; // ~ TODO: Need to ensure there is at least one weapon
        MainMenuManager.Inst.LoadScene(1);
    }

    void AssignWeapons()
    {
        // Clear any chosen weapons from previous rounds
        MainMenuManager.Inst.playerWeaponsLeft.Clear();
        MainMenuManager.Inst.playerWeaponsRight.Clear();

        // Get all the weapons chosen in the Character Selector
        CharacterSelect[] characterSelectors = GameObject.FindObjectsOfType<CharacterSelect>();
        //CharacterSelectionManager characterSelectionManager = GameObject.FindObjectOfType<CharacterSelectionManager>();

        foreach (var characterSelector in characterSelectors)
        {
            Debug.Log(characterSelector.name);
            MainMenuManager.Inst.playerWeaponsLeft.Add(characterSelector.leftArmWeapon);
            MainMenuManager.Inst.playerWeaponsRight.Add(characterSelector.rightArmWeapon);
        }

    }

    void OldWeaponAssignment()
    {

        // Clear any chosen weapons from previous rounds
        MainMenuManager.Inst.playerWeaponsLeft.Clear();
        MainMenuManager.Inst.playerWeaponsRight.Clear();

        // Get all the weapons chosen in the Character Selector
        CharacterSelect[] characterSelectors = GameObject.FindObjectsOfType<CharacterSelect>();
        foreach (var characterSelector in characterSelectors)
        {
            MainMenuManager.Inst.playerWeaponsLeft.Add(characterSelector.leftArmWeapon);
            MainMenuManager.Inst.playerWeaponsRight.Add(characterSelector.rightArmWeapon);
        }

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
