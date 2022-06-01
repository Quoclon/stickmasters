using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    /*
    public GameObject[] weaponPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipWeapon(GameObject bodyPartGO, BodyParts bodyPart)
    {
        int random = Random.Range(0, weaponPrefabs.Length - 1);

        // GameObject weapon = Instantiate(weaponPrefabs[random], bodyPartGO.transform.position, Quaternion.identity);
        GameObject weapon = Instantiate(weaponPrefabs[random], bodyPartGO.transform);

        //weapon.transform.position = new Vector2(weapon.transform.position.x, -weapon.transform.position.y);

        //weapon.transform.position = new Vector2(0, 1.6f);
        //weapon.transform.rotation.eulerAngles.Set(0, 0, 90);

        Weapon weaponScript = weapon.GetComponent<Weapon>();
        weaponScript.SetupWeaponComplicated(bodyPartGO);
    }
    */
}
