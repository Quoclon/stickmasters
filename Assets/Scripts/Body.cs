using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [Header("Options")]
    public bool weaponCollidesWithGround;
    public bool slowTimeOnHit;
    public bool slowTimeOnDisableBodyPart;
    public bool useJumpTimer;

    [Header("Player - Set in PlayerStats.cs")]
    public Players playerType;

    [Header("Invidivual Body Parts -- Remove Later?")]
    public BodyPart head;
    public BodyPart chest;

    public BodyPart armUpperRight;
    public BodyPart armLowerRight;

    public BodyPart armUpperLeft;
    public BodyPart armLowerLeft;

    public BodyPart legUpperRight;
    public BodyPart legLowerRight;

    public BodyPart legUpperLeft;
    public BodyPart legLowerLeft;

    [Header("State")]
    public bool alive;

    [Header("Weapons")]
    public WeaponHandler weaponsHandler;

    [Header("Arrays of Attached Components")]
    public Balance[] balancingParts;
    public BodyPart[] bodyParts;
    public Weapon[] weapons;
    public HingeJoint2D[] hingeBodyParts;
    public Collider2D[] colliders;

    void Start()
    {
        SetupBody();
    }

    public void SetupBody()
    {
        // Called is 
        alive = true;
        SetupBodyPartsArray();
        SetupHinge2DArray();
        SetupBalancingParts();
        SetupWeaponsArray();
        SetupCollidersArray();
        GetComponent<IgnoreCollision>().AvoidInternalCollisions();

        //Debug.Log("Body: " + this + " " + "PlayerType: " + playerType);

        GameManager.Inst.AddPlayerToList(this, playerType);
    }

    // Update is called once per frame
    void Update()
    {

    

    }


    public void AddDirectionalForceToRelevantBodyParts(Direction direction)
    {
        // Handle Jumping
        bool isGrounded = false;
        if (direction == Direction.Up)
        {
            foreach (var part in bodyParts)
            {
                if (part.isGrounded)
                {
                    isGrounded = true;
                    break;
                }
            }

            if (isGrounded)
            {
                foreach (var part in bodyParts)
                {
                    part.ApplyDirectionalForce(direction);
                }
            }
        }
        // Handle All Non-Jumping Movement
        else
        {
            foreach (var part in bodyParts)
            {
                part.ApplyDirectionalForce(direction);
            }
        }

        foreach (var weappon in weapons)
        {
            weappon.ApplyDirectionalForce(direction);
        }

    }

    #region StaggeredMovementBetterAnimation
    // This should produce a "walking" motion by waiting before each "step" (i.e. Left/Right legs)
    // ~ TODO: This produces slightly better animations, but requires identifying each leg
    public void AddDirectionalForceToRelevantBodyParts(Direction direction, float waitTimeBetweenEachPartsMovement)
    {
        StartCoroutine(StaggerMovement(direction, waitTimeBetweenEachPartsMovement));
    }

    IEnumerator StaggerMovement(Direction direction, float seconds)
    {
        if (direction == Direction.Left)
        {
            foreach (var part in bodyParts)
            {
                if (part.eBodyPart == BodyParts.LowerRightLeg)
                    part.ApplyDirectionalForce(direction);
            }

            yield return new WaitForSeconds(seconds);

            foreach (var part in bodyParts)
            {
                if (part.eBodyPart == BodyParts.LowerLeftLeg)
                    part.ApplyDirectionalForce(direction);
            }
        }

        if (direction == Direction.Right)
        {
            foreach (var part in bodyParts)
            {
                if (part.eBodyPart == BodyParts.LowerLeftLeg)
                    part.ApplyDirectionalForce(direction);
            }

            yield return new WaitForSeconds(seconds);

            foreach (var part in bodyParts)
            {
                if (part.eBodyPart == BodyParts.LowerRightLeg)
                    part.ApplyDirectionalForce(direction);
            }
        }


    }

    #endregion

    #region DisableBody
    public void DisableBodyPart(BodyPart bodyPart)
    {
        foreach (var part in bodyParts)
        {
            if (bodyPart == part)
            {
                // Disable Hinge
                if (bodyPart.bodyPartHinge != null)
                    DisableHingeJoint2D(bodyPart.bodyPartHinge);

                // Disable Balancing
                if (bodyPart.balancingPart != null)
                    bodyPart.balancingPart.force = 0f;

                // ! TEST: Disable connected joints                     //~ DOes this ever work? Test in DidsbleDirectly...
                if (bodyPart.bodyPartHinge != null)
                    if (bodyPart.bodyPartHinge.connectedBody != null)
                        DisableDirectlyConnectedHingeJoints(bodyPart);

                // Set Layer to Disabled (i.e. only collide with ground - in Project Settings)
                bodyPart.gameObject.layer = 9;

                bodyPart.disabled = true;
            }
        }
    }

    public void DisableBody(Players playerDealingDamage)
    {
        // Set Body State (i.e. for checking game over)
        alive = false;

        // Disable any remaining "Balancing" parts (so player falls)
        DisableAllBalancingBodyParts();
        DisableAllBodyParts();

        foreach (var weapon in weapons)
        {
            weapon.DisableWeapon();
        }

        // Set Game Over State -- let GameManager know winner
        GameManager.Inst.CheckGameOver(playerType, playerDealingDamage, this);
    }
    public void DisableAllBalancingBodyParts()
    {
        for (int i = 0; i < balancingParts.Length; i++)
        {
            balancingParts[i].force = 0;
        }
    }

    public void DisableAllBodyParts()
    {
        foreach (var part in bodyParts)
        {
            part.gameObject.layer = 9;
            part.disabled = true;
        }
    }

    // ~ TODO: Improve Hinges
    void DisableDirectlyConnectedHingeJoints(BodyPart bodyPart)
    {
        foreach (var hingeInBody in hingeBodyParts)
        {
            // If the "connected body" is connected to this bodyParts hinge
            if (hingeInBody.connectedBody == bodyPart.bodyPartHinge.attachedRigidbody)
            {
                foreach (var weapon in weapons)
                {
                    if (weapon.weaponHolderHinge == hingeInBody || weapon.weaponHolderHinge == bodyPart.bodyPartHinge)
                        weapon.DisableWeapon();
                }
            }
        }
    }

    void DisableHingeJoint2D(HingeJoint2D hinge)
    {
        hinge.enabled = false;
    }

    #endregion

    #region Setup Reference Arrays
    void SetupBodyPartsArray()
    {
        bodyParts = GetComponentsInChildren<BodyPart>();
    }

    void SetupWeaponsArray()
    {
        /*
        weaponsHandler = GetComponent<WeaponHandler>();
        foreach (var part in bodyParts)
        {
            if(part.eBodyPart == BodyParts.LowerLeftArm || part.eBodyPart == BodyParts.LowerRightArm)
            {
                weaponsHandler.EquipWeapon(part.gameObject, part.eBodyPart);
            }
        }
        */

        foreach (var part in bodyParts)
        {
            if (part.GetComponent<WeaponHolder>() != null)
            {
                WeaponHolder weaponHolder = part.GetComponent<WeaponHolder>();
                weaponHolder.EquipWeapon();
                //part.GetComponent<WeaponHolder>().EquipWeapon(eWeaponType.Katana);
                //part.GetComponent<WeaponHolder>().EquipRandomWeapon();
            }


        }

        weapons = GetComponentsInChildren<Weapon>();
    }

    void SetupBalancingParts()
    {
        balancingParts = GetComponentsInChildren<Balance>();
    }
    
    void SetupHinge2DArray()
    {
        hingeBodyParts = GetComponentsInChildren<HingeJoint2D>();
    }

    public void SetupCollidersArray()
    {
        colliders = GetComponentsInChildren<Collider2D>();
    }
    #endregion

    public void FlippingExperimentation()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            //transform.position = new Vector2(-transform.position.x, transform.position.y);
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);

            foreach (var part in bodyParts)
            {
                part.transform.position = new Vector2(transform.position.x - part.transform.position.x, transform.position.y - part.transform.position.y);
            }
        }
    }


}
