using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponArm : MonoBehaviour
{
    public eWeaponType weaponType;
    public WeaponHolder weaponHolder;

    public int pickWeaponChanceWeight;
    public int totalPickWeaponChance;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPickWeaponChance(int pickWeaponChance)
    {
        totalPickWeaponChance = pickWeaponChance;
    }

    public int GetPickWeaponChance()
    {
        return totalPickWeaponChance;
    }
}
