using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    [Header("Body Deatails")]
    public Body body;
    public BodyParts eBodyPart;
    private float dmgThreshold = 5f;

    [Header("State")]
    public bool disabled;
    public bool isGrounded;

    [Header("Rigid Body")]
    public Rigidbody2D rb;

    [Header("Rigid Body")]
    public DirectionalForce directionalForce;

    [Header("Balancing")]
    public Balance balancingPart;

    [Header("Health")]
    public float healthMax;
    public float health;
    private float defaultHealth = 5;

    [Header("Dmg Visuals")]
    public SpriteRenderer sprite;
    private Color spriteColorOriginal;

    [Header("Environment Damage")]
    private float environmentDamageDenominator = 10f;

    [Header("Hinges")]
    public HingeJoint2D bodyPartHinge;
    public HingeJoint2D otherBodyPartConnectedByHinge2D;

    // Start is called before the first frame update
    void Start()
    {
        // Establish Body + Head to Spawn Damage From
        body = GetComponentInParent<Body>();

        if (GetComponent<Balance>() != null)
            balancingPart = GetComponent<Balance>();

        if (GetComponent<DirectionalForce>() != null)
            directionalForce = GetComponent<DirectionalForce>();

        // Set HingeJoint2D - Not all Body Parts have Hinge Joints
        if (GetComponent<HingeJoint2D>() != null)
        {
            bodyPartHinge = GetComponent<HingeJoint2D>();
            //DetermineDirectlyConenctedHinge();
        }

        if (GetComponent<Rigidbody2D>() != null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Set SpriteRenderer - For Flashing Damage on Hit
        sprite = GetComponent<SpriteRenderer>();
        spriteColorOriginal = sprite.color;

        // Set Health Stats - Use default Health if no health set for this body part
        health = healthMax;
        if (healthMax <= 0)
            health = defaultHealth;
        
    }


    void Update()
    {
        //Debug.Log(this.name + " " + isGrounded);
    }
    

    public void ApplyDirectionalForce(Direction direction)
    {
        if (disabled)
            return;

        if (directionalForce == null)
            return;
    
        directionalForce.ApplyForce(direction);
    }



    #region Damage and Death
    void ReduceHealth()
    {

    }

    public void TakeDamage(float dmg, Body attackingPlayersBody, Players attackingPlayersType)
    {
        // If Body Part disabled, return
        if (disabled)
            return;

        // If damage is under threshhold (also checked in Weapon and OnCollision2d)                             
        if (dmg < dmgThreshold) // TODO: Make this global for testing, because Weapon + OnCollision2d (for bodypart) also handle magnitude returns
            return;

        // Spawn Particles - OPTION: use 'TakeDamage'
        //ParticleManager.Inst.PlayParticle(ParticleManager.Inst.particleBlood, dmg, this.transform);

        // Play Sound
        SoundManager.Inst.Play(SoundManager.Inst.playerHit);

        // Spawn Damage Numbers
        DamageNumberManager.Inst.SpawnDamageNumber(dmg, attackingPlayersType, body.head.transform);
        
        // Flash Body on Hit
        FlashBodyPart(.25f, 0f);

        // Reduce Health
        health -= dmg;

        // Check if bodyPart is Destroyed/Disabled
        if (health <= 0) 
        {
            // Spawn Particles - OPTION: use on 'DiableBodyPart'
            ParticleManager.Inst.PlayParticle(ParticleManager.Inst.particleBlood, dmg, this.transform);
            DisableBodyPart(attackingPlayersType);
        }           
    }



    void DisableBodyPart(Players playerDealingDamage)
    {
        // Use Slow Time if the option is ticked
        if (body.slowTimeOnDisableBodyPart)
            TimeManager.Inst.SlowTime(.1f, .5f, true);

        // Removing Body Parts via Body - Array of Parts
        body.DisableBodyPart(this);

        // Return if the player is already dead
        if (!body.alive)
            return;

        // Kill the Player if Head or 'Chest' is destroyed
        if (eBodyPart == BodyParts.Head || eBodyPart == BodyParts.Body)
        {
            body.DisableBody(playerDealingDamage);
        }
    }



    // Damage Flashing
    public void FlashBodyPart(float waitTimeAmount, float flashSpeedAmount)
    {
        // Coroutine for Flash Body Part ~ Could be whole body later
        StartCoroutine(FlashBodyPartCoroutine(waitTimeAmount, flashSpeedAmount));
    }

    private IEnumerator FlashBodyPartCoroutine(float waitTime, float flashSpeed)
    {
        sprite.color = Color.white;
        yield return new WaitForSeconds(waitTime);
        sprite.color = spriteColorOriginal;
    }

    #endregion


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Used for Jumping
        isGrounded = true;

        // Damage based on magnitude / threshodl (i.e. 30 / 6 = 5 damage)
        float potentialMagnitudeDamage = collision.relativeVelocity.magnitude / environmentDamageDenominator;

        if(potentialMagnitudeDamage <= 5)
            return;

        // Damage if hitting a Wall or other Causes Damage object
        if (collision.gameObject.tag == "CausesDamage")
        {
            TakeDamage(potentialMagnitudeDamage, null, Players.Environment);
        }   

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Used for Jumping
        //Debug.Log(this.name + "OnCollisionExit2D"); 
        isGrounded = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Used for Jumping
        isGrounded = true;
    }


    // Archie
    #region Disable Hinges
    void DetermineDirectlyConenctedHinge()
{
    HingeJoint2D[] hingeBodyParts = gameObject.GetComponentInParent<Movement>().GetComponentsInChildren<HingeJoint2D>();

    foreach (var hinge in hingeBodyParts)
    {
        if (hinge.connectedBody.gameObject == this.gameObject)
            otherBodyPartConnectedByHinge2D = hinge;
    }
}

    //Disable Hinge of Body Part --- Likely need a recurssive action here
    void DestoryAllConnectedBodyPartHinges()
    {
        HingeJoint2D[] hingeBodyParts = gameObject.GetComponentInParent<Movement>().GetComponentsInChildren<HingeJoint2D>();

        foreach (var hinge in hingeBodyParts)
        {
            if (hinge.connectedBody.gameObject == this.gameObject)
            {
                Debug.Log("Connected Hinge Name: " + hinge.connectedBody.gameObject.name);
                if (hinge.GetComponent<Balance>() != null)
                    hinge.GetComponent<Balance>().force = 0;

                if (hinge.GetComponent<Weapon>() != null)
                    hinge.GetComponent<Weapon>().DisableWeapon();

                hinge.enabled = false;
            }
        }

        if (bodyPartHinge != null)
            bodyPartHinge.enabled = false;
    }

    #endregion






















    void PlayHitSound()
    {

    }
    void PlayDeathSound()
    {

    }

    void PlayHitParticles()
    {

    }

    void PlayDeathParticles()
    {

    }
}
