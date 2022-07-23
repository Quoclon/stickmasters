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
    private float environmentDamageDenominator = 3;

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

        // ResetGrounded State
        isGrounded = false;
    
        // Set Health Stats - Use default Health if no health set for this body part
        health = healthMax;

        if (healthMax <= 0)
            health = defaultHealth;     
    }

    public void SetupSpriteColor(bool onlyColorHead, Color color)
    {
        sprite = GetComponent<SpriteRenderer>();


        Color lighterColor = color;    
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        Debug.Log("H: " + h + " " + "S: " + s + " " + "V: " + v);
        Color darkerColor = Color.HSVToRGB(h, s, v - 0.4f);

        // Color all body parts
        if (!onlyColorHead)
        {
            switch (eBodyPart)
            {
                case BodyParts.Default:
                    break;
                case BodyParts.Head:
                    sprite.color = lighterColor;
                    break;
                case BodyParts.Body:
                    sprite.color = lighterColor;
                    break;
                case BodyParts.UpperRightArm:
                    sprite.color = lighterColor;
                    break;
                case BodyParts.LowerRightArm:
                    sprite.color = lighterColor;
                    break;
                case BodyParts.UpperLeftArm:
                    sprite.color = darkerColor;
                    break;
                case BodyParts.LowerLeftArm:
                    sprite.color = darkerColor;
                    break;
                case BodyParts.UpperLeftLeg:
                    sprite.color = darkerColor;
                    break;
                case BodyParts.LowerLeftLeg:
                    sprite.color = darkerColor;
                    break;
                case BodyParts.UpperRightLeg:
                    sprite.color = lighterColor;
                    break;
                case BodyParts.LowerRightLeg:
                    sprite.color = lighterColor;
                    break;
                case BodyParts.WeaponHolder:
                    break;
                default:
                    break;
            }


        }
       
        // Color only head
        if (onlyColorHead && eBodyPart == BodyParts.Head)
        {
            sprite.color = color;
        }

        // Use for flashing damage and returning to current color
        spriteColorOriginal = sprite.color;
    }



    void Update()
    {
        //if (body.playerType == Players.P2)
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

    public float GetDmgThreshold()
    {
        return dmgThreshold;
    }

    public void TakeDamage(float dmg, float bleedDmg, Body attackingPlayersBody, Players attackingPlayersType, Collision2D collision)
    {
        // ~ TODO: Call directly from weapon, etc. eventually
        body.DamageBodyPart(this, dmg, bleedDmg, attackingPlayersType, collision);
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
        // Used for Jumping - Grounded whether it's ground, players arm, a weapon, etc.
        isGrounded = true;

        // Damage if hitting a Wall or other Causes Damage object
        if (collision.gameObject.tag == "CausesDamage")
        {
            // Damage based on magnitude / threshodl (i.e. 30 / 6 = 5 damage)
            float potentialMagnitudeDamage = collision.relativeVelocity.magnitude / environmentDamageDenominator;

            if (potentialMagnitudeDamage <= 5)
                return;

            TakeDamage(potentialMagnitudeDamage, 0, null, Players.Environment, collision);
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
}
