using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    // WeaponHolder Options (i.e. AI)
    public eWeaponHolderOptions spawnOption;

    // Default Selectable
    public eWeaponType defaultWeapon;

    // Array of Weapons
    public WeaponArm[] weaponArms;

    public void EquipWeaponArm()
    {
        if (spawnOption == eWeaponHolderOptions.Default)
        {
            foreach (var weaponArm in weaponArms)
            {
                if (weaponArm.weaponType != defaultWeapon)
                    weaponArm.gameObject.SetActive(false);
            }
        }

        if (spawnOption == eWeaponHolderOptions.Random)
        {
            int randomNum = Random.Range(0, weaponArms.Length);

            for (int i = 0; i < weaponArms.Length; i++)
            {
                if (i != randomNum)
                    weaponArms[i].gameObject.SetActive(false);
            }
        }

        if (spawnOption == eWeaponHolderOptions.NoWeapon)
        {
            foreach (var weaponArm in weaponArms)
            {
                 weaponArm.gameObject.SetActive(false);
            }
        }



    }

}
