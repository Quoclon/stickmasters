using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    // for main menu
    public eWeaponArms armType;

    // WeaponHolder Options (i.e. AI)
    public eWeaponHolderOptions spawnOption;

    // Default Selectable
    public eWeaponType defaultWeapon;

    // Array of Weapons
    //public WeaponArm[] weaponArms;

    public List<WeaponArm> weaponArms;

    public void EquipWeaponArm()
    {
     
        // Default Option
        if (spawnOption == eWeaponHolderOptions.Default)
        {
            foreach (var weaponArm in weaponArms)
            {
                if (weaponArm.weaponType != defaultWeapon)
                    weaponArm.gameObject.SetActive(false);
            }

            return;
        }

        // Random Option
        if (spawnOption == eWeaponHolderOptions.Random)
        {
            int randomNum = Random.Range(0, weaponArms.Count);

            for (int i = 0; i < weaponArms.Count; i++)
            {
                if (i != randomNum)
                    weaponArms[i].gameObject.SetActive(false);
            }

            return;
        }

        // Weighted Random Option
        if (spawnOption == eWeaponHolderOptions.WeightedRandom)
        {
            // Calculate total chance, based on each weapons indivdual chances. Used to generate top end of random.
            int totalWeightedRandomChance = 0;
            
            // Sort the weapon list based on likelyhood to be chosen; Least likely first;
            weaponArms = weaponArms.OrderByDescending(weapon => weapon.pickWeaponChanceWeight).ToList();
            
            foreach (var weaponArm in weaponArms)
            {
                totalWeightedRandomChance += weaponArm.pickWeaponChanceWeight;
                weaponArm.totalPickWeaponChance = totalWeightedRandomChance;
                //Debug.Log(weaponArm.name + " Weighted Chance: " + weaponArm.totalPickWeaponChance);
            }

            // Random Chance based on all the of weights added up for each weapon
            int randomNumWeighted = Random.Range(0, totalWeightedRandomChance);
            //Debug.Log("randomNumWeighted: " + randomNumWeighted);

            // First - Set all weaponArms to False
            foreach (var weaponArm in weaponArms)
            {
                weaponArm.gameObject.SetActive(false);
            }

            // Second - go through the sorted list, lowest chance to highest, checking to see if randon Num is below; Return once a weapon is set to active
            foreach (var weaponArm in weaponArms)
            {          
                if(randomNumWeighted <= weaponArm.totalPickWeaponChance)
                {
                    weaponArm.gameObject.SetActive(true);
                    return;
                }
            }

            return;
        }


        // No Weapon
        if (spawnOption == eWeaponHolderOptions.NoWeapon)
        {
            // ~ OLD CODE
            
            foreach (var weaponArm in weaponArms)
            {
                 weaponArm.gameObject.SetActive(false);
            }
            
            foreach (var weaponArm in weaponArms)
            {
                if (weaponArm.weaponType == eWeaponType.None)
                    weaponArm.gameObject.SetActive(true);
            }
            return;

        }

        // Fallback Action
        //weaponArms[0].gameObject.SetActive(true);

    }

}
