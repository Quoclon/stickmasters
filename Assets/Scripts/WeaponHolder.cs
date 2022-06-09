using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    // WeaponHolder Options (i.e. AI)
    public eWeaponHolderOptions spawnOption;

    // Default Selectable
    public eWeaponType defaultWeapon;

    // Current Weapon/WeaponScript
    public Weapon currentWeaponScript;
    public GameObject currentWeapon;

    // Array of Weapons
    public GameObject[] weapons;

    // Array of Weapons
    public GameObject[] weaponArms;

    public void EquipWeapon()
    {
        foreach (var weapon in weapons)
        {
            if(weapon.activeInHierarchy == true)
            {
                //Debug.Log(this.gameObject.name + " " + weapon.name);
                currentWeapon = weapon;
                currentWeaponScript = weapon.GetComponent<Weapon>();             // ~ can creat a callback that informs of weapon is selected (i.e. in case we only want one)
                currentWeaponScript.SetupWeapon();
                return;
            }
        }

        if (spawnOption == eWeaponHolderOptions.Default)
            EquipDefaultWeapon();

        if (spawnOption == eWeaponHolderOptions.Random)
            EquipRandomWeapon();
    }

    public void EquipDefaultWeapon()
    {
         EquipWeapon(defaultWeapon);
    }

    public void EquipRandomWeapon()
    {
        // Reduce Cudgel
        int rareCudgel = Random.Range(0, 100);
        if(rareCudgel >= 95)
        {
            EquipWeapon(weapons[weapons.Length-1]);
            return;
        }

        // Katana or GreatSword
        int random = Random.Range(0, weapons.Length-1);
        EquipWeapon(weapons[random]);
    }

    public void EquipWeapon(GameObject weaponObject)
    {
        currentWeapon = weaponObject;
        currentWeaponScript = weaponObject.GetComponent<Weapon>();

        weaponObject.SetActive(true);
        currentWeaponScript.SetupWeapon();
    }

    public void EquipWeapon(eWeaponType equipWeaponByType)
    {
        foreach (var weapon in weapons)
        {
            Weapon weaponScript = weapon.GetComponent<Weapon>();
            if (weaponScript.weaponType == equipWeaponByType)
            {
                weapon.SetActive(true);
                currentWeapon = weapon;
                currentWeaponScript = weaponScript;
                weaponScript.SetupWeapon();
            }
        }
    }
}
