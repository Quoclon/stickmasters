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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipWeapon()
    {
        foreach (var weapon in weapons)
        {
            if(weapon.activeInHierarchy == true)
            {
                currentWeapon = weapon;
                currentWeaponScript = weapon.GetComponent<Weapon>();
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
        int random = Random.Range(0, weapons.Length);
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
