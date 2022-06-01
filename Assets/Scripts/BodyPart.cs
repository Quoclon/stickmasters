using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    [Header("Body Deatails")]
    public Body body;
    public BodyParts eBodyPart;

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
    public void TakeDamage(float dmg, Players playerDealingDamage)
    {
        // If Body Part disabled, return
        if (disabled)
            return;

        DamageNumberManager.Inst.SpawnDamageNumber(dmg, playerDealingDamage, body.head.transform);
        FlashBodyPart(.1f, 0f);
        ReduceHealth(dmg, playerDealingDamage);
    }

    void ReduceHealth(float dmg, Players playerDealingDamage)
    {
        health -= dmg;

        // Check if bodyPart is Destroyed/Disabled
        if (health <= 0)
            DisableBodyPart(playerDealingDamage);
    }

    void DisableBodyPart(Players playerDealingDamage)
    {
        if (body.slowTimeOnDisableBodyPart)
        {
            TimeManager.Inst.SlowTime(.1f, .5f, true);
        }

        // Removing Body Parts via Body Array of Parts (Centrealized) ~ Is this useful? Awareness?
        body.DisableBodyPart(this);

        //DestoryAllConnectedBodyPartHinges();

        // Return if the player is already dead
        if (!body.alive)
            return;

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

        // Damage if hitting a Wall or other Causes Damage object
        if(collision.gameObject.tag == "CausesDamage")
        {
            float potentialMagnitudeDamage = collision.relativeVelocity.magnitude / environmentDamageDenominator;
            // If Damage is over threshhold, deal damage
            if (potentialMagnitudeDamage > 1)
                TakeDamage(potentialMagnitudeDamage, Players.Environment);
        }   
 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Used for Jumping
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
