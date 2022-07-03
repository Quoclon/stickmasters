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
    public bool optionMobileControls;

    [Header("Player - Set in PlayerStats.cs")]
    public Players playerType;

    [Header("Invidivual Body Parts")]
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
    public bool winner;

    [Header("Weapons")]
    public WeaponHandler weaponsHandler;
    private GameObject[] weaponArmObjects;

    [Header("Arrays of Attached Components")]
    public Balance[] balancingParts;
    public BodyPart[] bodyParts;
    public Weapon[] weapons;
    public HingeJoint2D[] hingeBodyParts;
    public Collider2D[] colliders;
    public WeaponHandler[] weaponHandlers;

    [Header("Health")]
    public HealthBar healthbar;
    [SerializeField] float health;
    [SerializeField] float maxHealth;
    float healthPerPartModifier;
    float totalHealthModifier;

    [Header("Dmg")]
    Players lastPlayerThatDealtDmg;
    BodyPart lastBodyPartHit;
    float damageThreshold;

    [Header("Bleed Dmg")]
    float bleedPerSec;
    public float bleedPerSecFromWeaponDmg;
    public float bleedPerSecMultiplier;

    [Header("Particles")]
    public ParticleSystem[] dustParticle;

    [Header("Color")]
    public bool onlyColorHead;
    public ColorHandler colorHandler;

    void Start()
    {
        //SetupBody();
    }

    public void SetupBody(Players _playerType)
    {
        //Debug.Log(_playerType);

        // Called via SpawnManager
        playerType = _playerType;
        alive = true;
        SetupBodyPartsArray();
        SetupHinge2DArray();
        SetupBalancingParts();
        SetupWeaponsArray();
        SetupCollidersArray();
        GetComponent<IgnoreCollision>().AvoidInternalCollisions();

        RefineBodyPartsArray();

        // Setup Player Head/Body Color
        SetupColor();

        // Setup Hit Points
        SetupHealth();

        //Debug.Log("Body: " + this + " " + "PlayerType: " + playerType);

        GameManager.Inst.AddPlayerToList(this, playerType);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Inst.isRoundOver)
            return;

        if (GameManager.Inst.isMatchOver)
            return;

        if (!alive)
            return;

        if (bleedPerSec > 0)
        {
            health -= bleedPerSec * Time.deltaTime;

            if(health > 0)
                healthbar.SetHealthBarCurrentHealth(health);
        }

        if(health <= 0)
        {
            KillPlayer(lastPlayerThatDealtDmg);
        }
    }

    void SetupHealth()
    {
        // Survival Mode - Give player more health; Enemy per part will have been reduced already
        if(GameManager.Inst.gameMode == eGameMode.Survival)
        {
            // Overall Health = [all parts health] * totalHealthModifier
            if (GameManager.Inst.isPlayerTypePlayer(playerType))
            {
                healthPerPartModifier = GameManager.Inst.totalHealthPerPartModifier * 1.5f;
                totalHealthModifier = GameManager.Inst.totalHealthPerPartModifier;
            }
            else
            {
                //Debug.Log("GameManager.Inst.spawnedEnemies: " + GameManager.Inst.spawnedEnemies);
                //Debug.Log("GameManager.Inst.enemiesWaveNumber: " + GameManager.Inst.enemiesWaveNumber);
                healthPerPartModifier = GameManager.Inst.totalHealthPerPartModifier * GameManager.Inst.enemiesWaveNumber / GameManager.Inst.totalWaveDenominator;  // .20f first level, .40f second level 
                totalHealthModifier = GameManager.Inst.totalHealthPerPartModifier;
                //Debug.Log("healthPerPartModifier: " + healthPerPartModifier);
            }
        }

        // Non-Survival Modes
        if (GameManager.Inst.gameMode != eGameMode.Survival)
        {
            // Give the player more health
            if (GameManager.Inst.isPlayerTypePlayer(playerType))
            {
                healthPerPartModifier = GameManager.Inst.totalHealthPerPartModifier;
                totalHealthModifier = GameManager.Inst.totalHealthPerPartModifier + 0.25f;  // A little extra health vs npc 1v1, 2v2, etc.
            }
            else
            {
                healthPerPartModifier = GameManager.Inst.totalHealthPerPartModifier;
                totalHealthModifier = GameManager.Inst.totalHealthPerPartModifier;
            }

        }

        // Foreach part, modify it's per-part health; add to total, modify total as needed.
        foreach (var part in bodyParts)
        {
            part.healthMax *= healthPerPartModifier;
            maxHealth += part.healthMax * totalHealthModifier;
        }

        //Debug.Log("Player: " + playerType + " " + "Total Health: " + maxHealth);
        health = maxHealth;

        SetupHealthBar();
    }

    void SetupHealthBar()
    {
        healthbar.SetHealthBarDefaultSetup(maxHealth);
    }

    void SetupColor()
    {
        // Get the Color Handler Script
        colorHandler = GetComponent<ColorHandler>();

        if (GameManager.Inst.isPlayerTypePlayer(playerType))
        {
            foreach (var part in bodyParts)
            {
                //SpriteRenderer sprite = part.GetComponent<SpriteRenderer>();
                switch (playerType)
                {
                    case Players.P1:
                        part.SetupSpriteColor(onlyColorHead, colorHandler.GetPlayerColor(1));
                        break;
                    case Players.P2:
                        part.SetupSpriteColor(onlyColorHead, colorHandler.GetPlayerColor(2));
                        break;
                    case Players.p3:
                        part.SetupSpriteColor(onlyColorHead, colorHandler.GetPlayerColor(3));
                        break;
                    case Players.p4:
                        part.SetupSpriteColor(onlyColorHead, colorHandler.GetPlayerColor(4));
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            foreach (var part in bodyParts)
            {
                part.SetupSpriteColor(onlyColorHead, colorHandler.GetColorByWave(GameManager.Inst.enemiesWaveNumber));
            }
        }
    }
    
    public void AddDirectionalForceToRelevantBodyParts(Direction direction)
    {
        // Handle Jumping
        //bool isGrounded = false;
        bool canJump = false;

        if (direction == Direction.Up)
        {
            foreach (var part in bodyParts)
            {
                if (part.isGrounded && !part.disabled)
                {
                    //Debug.Log(part.name + " isGrounded: " + part.isGrounded);
                    canJump = true;
                    break;
                }
            }

            if (canJump)
            {
                int dustCreatedCount = 0;

                foreach (var part in bodyParts)
                {
                    if((part.eBodyPart == BodyParts.LowerRightLeg || part.eBodyPart == BodyParts.LowerLeftLeg) && dustCreatedCount < 2)
                    {
                        //CreateJumpDust(part.transform, dustCreatedCount);
                        dustCreatedCount++;
                    }
 
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

        // Handle Weapon Force (if any)
        foreach (var weappon in weapons)
        {
            weappon.ApplyDirectionalForce(direction);
        }

    }

    void CreateJumpDust(Transform footTransform, int dustCount)
    {
        //Debug.Log("Dust Jump: " + footTransform.gameObject.name);
        dustParticle[dustCount].transform.position = new Vector3(footTransform.position.x, footTransform.position.y - 0.25f, 0);
        dustParticle[dustCount].Play();
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

    public void DamageBodyPart(BodyPart _bodyPart, float dmg, float bleedDmg, Players attackingPlayersType, Collision2D collision)
    {
        // ~ UNTESTED COULD CAUSE BUGS
        if (!alive)
            return;

        foreach (var bodypart in bodyParts)
        {
            if(bodypart == _bodyPart)
            {
                // If Body Part disabled, return
                if (bodypart.disabled)
                    return;

                // If damage is under threshhold (also checked in Weapon and OnCollision2d)                             
                if (dmg < bodypart.GetDmgThreshold()) // TODO: Make this global for testing, because Weapon + OnCollision2d (for bodypart) also handle magnitude returns
                    return;

                // Get last Attacking PlayerType and BodyPart damaged
                lastPlayerThatDealtDmg = attackingPlayersType;
                lastBodyPartHit = bodypart;

                // Spawn Particles - OPTION: use 'TakeDamage'
                //ParticleManager.Inst.PlayParticle(ParticleManager.Inst.particleBlood, dmg, this.transform);

                // Sound on Damage
                SoundManager.Inst.Play(SoundManager.Inst.playerHit);

                // Spawn Damage Numbers
                DamageNumberManager.Inst.SpawnDamageNumber(dmg, attackingPlayersType, head.transform);

                // Flash Body on Hit
                bodypart.FlashBodyPart(.25f, 0f);

                // Reduce Overall Body Health
                health -= dmg;
                healthbar.SetHealthBarCurrentHealth(health);
           
                // ~ TODO: Pass in "BleedDmg" from weapon (if I end with multiple bleed causing weapons)
                if(bleedDmg > 0)
                {
                    // Player Bleed Denominator
                    float bleedPerSecDenominator = 20f;

                    // Enemy Bleed Denomiator lower if Survival
                    if (!GameManager.Inst.isPlayerTypePlayer(playerType) && GameManager.Inst.gameMode == eGameMode.Survival)
                        bleedPerSecDenominator = 15f;

                    bleedPerSecFromWeaponDmg += dmg / bleedPerSecDenominator;
                    DetermineBleedPerSecond();
                }

                // Reduce bodyPart Health @ a fraction of damage. Makes bodyParts easier to sever
                float bodyPartHealthReductionDenominator = 10f;
                bodypart.health -= (dmg / bodyPartHealthReductionDenominator);
                bool severBodyPart = false;

                // Sever the Body Part if damage above body part health
                if (dmg >= bodypart.health)
                    severBodyPart = true;

                // ~ TESTSING - remove if you don't want parts to be severed on last hit
                if(health <= 0)
                    severBodyPart = true;

                // Check if bodyPart is Destroyed/Disabled
                //if (dmg >= bodypart.health)
                if(severBodyPart)
                {
                    // Spawn Particles - OPTION: use on 'DiableBodyPart'
                    ParticleManager.Inst.PlayParticle(ParticleManager.Inst.particleBlood, dmg, bodypart.transform);
                    DisableBodyPart(bodypart, attackingPlayersType);                  
                }
                else
                {
                    // Spawn Blood Particle for Hit Dmg
                    ParticleManager.Inst.PlayRandomParticle(ParticleManager.Inst.bloodOnHitParticles, 1, collision);

                    // Play Sound -- if hit but not severed
                    //SoundManager.Inst.Play(SoundManager.Inst.playerHit);
                }
            }
        }
    }

    #region DisableBody
    public void DisableBodyPart(BodyPart bodypart, Players playerDealingDamage)
    {
        // Use Slow Time if the option is ticked
        if (slowTimeOnDisableBodyPart)
            TimeManager.Inst.SlowTime(.1f, .5f, true);

        // Play Sound for Disable Body Part
        SoundManager.Inst.PlayRandomFromArray(SoundManager.Inst.severedLimbs);

        // Check all the bodyParts for the part passed in (via BodyPar.cs) to disable
        foreach (var part in bodyParts)
        {
            // Disable the Body Part if it maches one on the Body (which it should
            if (bodypart == part)
            {
                // Disable Hinge
                if (bodypart.bodyPartHinge != null) 
                {
                    // Disable Body Part Hinge (i.e. disable the hinge that connets the lower arm to the Upper Arm)
                    bool disableHinge = true;
                    DisablePart(bodypart, disableHinge);

                    // Check for a connectedBody (i.e. the RigidBody of the Upper Arm, which the Lower Arm was connected to)
                    if (bodypart.bodyPartHinge.connectedBody == null)
                        continue;

                    //Debug.Log("bodyPart.bodyPartHinge.connectedBody: " + bodyPart.bodyPartHinge.connectedBody);
                    DisableDirectlyConnectedHingeJoints(bodypart);
                }

            }
        }


        // Return if the player is already dead
        if (!alive)
            return;

        // Kill the Player if Head or 'Chest' is destroyed
        if (bodypart.eBodyPart == BodyParts.Head || bodypart.eBodyPart == BodyParts.Body)
        {
            KillPlayer(playerDealingDamage);
        }
    }

    void DetermineBleedPerSecond()
    {
        //Debug.Log("DetermineBleedPerSecond Ran");
        bleedPerSec = 0;

        if (legLowerLeft.disabled || legLowerRight.disabled)
        {
            if (legLowerLeft.disabled && legLowerRight.disabled)
            {
                bleedPerSec += 20f;               
            }
            else if (legLowerLeft.disabled)
            {
                bleedPerSec += 4f;
            }
            else if (legLowerRight.disabled)
            {
                bleedPerSec += 4f;
            }
        }

        if (legUpperLeft.disabled || legUpperRight.disabled)
        {
            if (legUpperLeft.disabled && legUpperRight.disabled)
            {
                bleedPerSec += 20f;
            }
            else if (legUpperLeft.disabled)
            {
                bleedPerSec += 4f;
            }
            else if (legUpperRight.disabled)
            {
                bleedPerSec += 4f;
            }
        }

        if (armLowerLeft.disabled || armLowerRight.disabled)
        {
            if (armLowerLeft.disabled && armLowerRight.disabled)
            {
                bleedPerSec += 40f;
            }
            else if (armLowerLeft.disabled)
            {
                bleedPerSec += 2f;
            }
            else if (armLowerRight.disabled)
            {
                bleedPerSec += 2f;
            }

            // If no Weapons Increase BleedPerSec a lot
            foreach (var weapon in weapons)
            {
                int weaponDisabledCount = weapons.Length;
                //Debug.Log(weapon.name);
                if(weapon.weaponDisabled || weapon.weaponType == eWeaponType.None)
                    weaponDisabledCount--;

                if (weaponDisabledCount <= 0)
                    bleedPerSec += 30;
            }
        }

        if (armUpperLeft.disabled || armUpperRight.disabled)
        {
            if (armUpperLeft.disabled && armUpperRight.disabled)
            {
                bleedPerSec += 30f;
            }
            else if (armUpperLeft.disabled)
            {
                bleedPerSec += 4f;
            }
            else if (armUpperRight.disabled)
            {
                bleedPerSec += 4f;
            }
        }


        // Body Part BleedPerSec + Weapon Based BleedPerSec * Preset Multiplier
        bleedPerSec = (bleedPerSec + bleedPerSecFromWeaponDmg) * bleedPerSecMultiplier;
        //Debug.Log(bleedPerSec);

        SetupBleedAnimation(bleedPerSec);      
    }

    void SetupBleedAnimation(float bleedPerSec)
    {
        float animationSpeed = 0f;

        //Debug.Log("bleedPerSec: " + bleedPerSec);

        if (bleedPerSec > 0)
            animationSpeed = .10f;

        if (bleedPerSec >= 10)
            animationSpeed = .25f;

        if (bleedPerSec >= 20)
            animationSpeed = .50f;

        if (bleedPerSec >= 30)
            animationSpeed = 1f;

        if (bleedPerSec >= 40)
            animationSpeed = 2f;

        if (bleedPerSec >= 60)
            animationSpeed = 3f;

        healthbar.SetBleedAnimation(animationSpeed);
    }

    void DisablePart(BodyPart bodyPart, bool disableHinge)
    {
        //Debug.Log("DisableHingeJoint2D"  + hinge.name);

        // Disable Balancing
        if (bodyPart.balancingPart != null)
            bodyPart.balancingPart.force = 0f;

        // Set Layer to Disabled (i.e. only collide with ground - in Project Settings)
        bodyPart.gameObject.layer = 9;

        // Set the state
        bodyPart.disabled = true;

        // Disable Hinge of bodyPart (i.e. lower arm, which releases it from the Upper Arm)
        if(disableHinge)
            bodyPart.bodyPartHinge.enabled = false;

        // Disable a weapon - if it's Hinges 'connectedBody', is this body part (comparing hinges)
        foreach (var weapon in weapons)
        {
            if (weapon.weaponHolderHinge == bodyPart.bodyPartHinge)
                weapon.DisableWeapon();
        }

        // ~ Determine Bleed Amount Per Second after each limb is severed
        DetermineBleedPerSecond();
    }


    // ~ TODO: Improve Hinges
    void DisableDirectlyConnectedHingeJoints(BodyPart bodyPart)
    {
        // Checking all 'lower' in the chain body parts
        // Seeing if they have a 'connected body' that is the body part we are disabling
        foreach (var part in bodyParts)
        {
            if (part.bodyPartHinge == null)
                continue;

            if (part.bodyPartHinge.connectedBody == null)
                continue;

            if (part.bodyPartHinge.connectedBody == bodyPart.rb)
            {
                if (part.enabled)
                {
                    bool disableHinge = false;      // Disable the lower-body-part, but not it's hinge
                    DisablePart(part, disableHinge);
                }
            }
        }
    }

   
    public void KillPlayer(Players playerDealingDamage)    // ~TODO: Trak last palyer doing dmaage for bleed out
    {
        // Remove Health Bar Visuals
        healthbar.DisableUI();

        // Set Body State (i.e. for checking game over)
        alive = false;

        // Final Hit Slow-Down
        TimeManager.Inst.SlowTime(.15f, .2f, false);

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


    #endregion

    #region Setup Reference Arrays
    void SetupBodyPartsArray()
    {
        bodyParts = GetComponentsInChildren<BodyPart>();
    }

    void RefineBodyPartsArray()
    {
        foreach (var bodypart in bodyParts)
        {
            // If the bodyPart "Parent" (i.e. the "Right Arm) is Null, then don't assign the child bodyParts (i.e. to "left arm")
            if(bodypart.gameObject.transform.parent != null)
            {
                if (bodypart.gameObject.transform.parent.gameObject.activeInHierarchy == false)
                    continue;
            }

            switch (bodypart.eBodyPart)
            {
                case BodyParts.Default:
                    break;
                case BodyParts.Head:
                    head = bodypart;
                    break;
                case BodyParts.Body:
                    chest = bodypart;
                    break;
                case BodyParts.UpperRightArm:
                    armUpperRight = bodypart;
                    break;
                case BodyParts.LowerRightArm:
                    armLowerRight = bodypart;
                    break;
                case BodyParts.UpperLeftArm:
                    armUpperLeft = bodypart;
                    break;
                case BodyParts.LowerLeftArm:
                    armLowerLeft = bodypart;
                    break;
                case BodyParts.UpperLeftLeg:
                    legUpperLeft = bodypart;
                    break;
                case BodyParts.LowerLeftLeg:
                    legLowerLeft = bodypart;
                    break;
                case BodyParts.UpperRightLeg:
                    legUpperRight = bodypart;
                    break;
                case BodyParts.LowerRightLeg:
                    legLowerRight = bodypart;
                    break;
                case BodyParts.WeaponHolder:
                    break;
                default:
                    break;
            }
        }
    }

    void SetupWeaponsArray()
    {
        /*
        weaponArmObjects = GameObject.FindGameObjectsWithTag("WeaponArms");
        foreach (var weaponArm in weaponArmObjects)
        {
            weaponArm.SetActive(true);
        }
        */

        weaponHandlers = GetComponentsInChildren<WeaponHandler>();
        foreach (var weaponHandler in weaponHandlers)
        {
            weaponHandler.EquipWeaponArm();            
        }

        int weaponCount = 0;

        // Setup Player with Weapons -- Just the Weapons (not custom arm limit settings, etc.)     
        foreach (var part in bodyParts)
        {
            if (part.GetComponent<WeaponHolder>() != null)
            {
                WeaponHolder weaponHolder = part.GetComponent<WeaponHolder>();
                weaponHolder.EquipWeapon();

                /*
                if (weaponHolder.currentWeaponScript.weaponType != eWeaponType.None) 
                {
                    weaponCount++;
                }
                */

                // ARCHIVE
                //part.GetComponent<WeaponHolder>().EquipWeapon(eWeaponType.Katana);
                //part.GetComponent<WeaponHolder>().EquipRandomWeapon();
            }
        }

  
        weapons = GetComponentsInChildren<Weapon>();

        // Check if there are "no weapons" - and if so rerun setup
        
        foreach (var weapon in weapons)
        {
            if (weapon.weaponType != eWeaponType.None)
                weaponCount++;
        }

        if (weaponCount == 0)
            SetupWeaponsArray();




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
