using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Type")]
    public eWeaponType weaponType;

    [Header("Impact Force")]
    public PointEffector2D pointEffector2d;
    public float effectorForce;

    [Header("Weapon Stats")]
    public Players weaponOwnerType;
    public Body ownersBody;
    public float dmgMultiplier = 1f;
    //public eDamageType dmgType;
    public float bleedDmg;
    //public float dmgEffectorModifer;
    public bool weaponDisabled;

    [Header("Manual Swinging - Force")]
    public float weaponForce;
    private Vector2 SwingDirection;

    [Header("Manual Swinging - Torque")]
    public float weaponTorqueForce;

    [Header("Manual Swinging - Balance")]
    public Balance balance;
    public float balanceRotationDefault;

    [Header("Physics Material")]
    public PhysicsMaterial2D physicsMaterial;

    [Header("Weapon RigidBody")]
    Rigidbody2D rb;

    [Header("Rigid Body")]
    public DirectionalForce directionalForce;

    [Header("Weapon Holder Hinge")]
    public HingeJoint2D weaponsHinge;
    public HingeJoint2D weaponHolderHinge;

    [Header("Link Weapons")]
    public GameObject[] weaponLinks;

    [Header("Weapon Sounds")]
    private bool readyToPlaySwingSound;
    private float swingSoundTimer = 0f;
    private float swingSoundTimerMax = 2f;
    private float totalVelocityPerSwing = 0;
    private float averageVelocityPerSecond = 0f;
    
    [Header("State")]
    public bool isGrounded;

    public void SetupWeapon()
    {
        // ~ TESTING
        balanceRotationDefault = 180f; /// testing

        // Rigid Body
        rb = GetComponent<Rigidbody2D>();

        // Setup Hinge
        if(weaponsHinge == null)
            weaponsHinge = GetComponent<HingeJoint2D>();
        //weaponsHinge.connectedBody = gameObject.GetComponentInParent<Rigidbody2D>();  // Already Setup 
        
        // Only auto-setup if weapon does not already have a weaponHolderHinge assigneed (i.e. whip, flail are manual assign)
        if(weaponHolderHinge == null)
            weaponHolderHinge = weaponsHinge.connectedBody.GetComponent<HingeJoint2D>();

        // Ownership
        ownersBody = GetComponentInParent<Body>();
        weaponOwnerType = ownersBody.playerType;

        // Sounds
        readyToPlaySwingSound = true;
        swingSoundTimer = swingSoundTimerMax;

        // Balance
        if (GetComponent<Balance>() != null)
        {
            balance = GetComponent<Balance>();
            balance.targetRotation = balanceRotationDefault;
        }

        if(rb.GetComponent<PhysicsMaterial2D>() != null)
        {
            physicsMaterial =  rb.GetComponent<PhysicsMaterial2D>();
            Debug.Log("physicsMaterial2d: " + physicsMaterial.bounciness);

        }

        // Directional Force
        if (GetComponent<DirectionalForce>() != null)
            directionalForce = GetComponent<DirectionalForce>();

        if (GetComponentInChildren<PointEffector2D>() != null)
            pointEffector2d = GetComponentInChildren<PointEffector2D>();

        // Collision Layers
        if (ownersBody.weaponCollidesWithGround)
            gameObject.layer = 10;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (weaponDisabled)
            return;

        // ~ TODO: TEST/REMOVE
        if (Input.GetMouseButtonDown(0) && weaponOwnerType == Players.P1)
        {
            //SwingWeaponForce();
            //ChangeBalance();
        }

        // ~ TODO: TEST/REMOVE
        if (Input.GetMouseButtonDown(1) && weaponOwnerType == Players.P1)
        {
            //SwingWeaponTorque();
        }

        // Check if sword swing has enough magnitude for a sound
        //CheckSwordSwingSound();
    }

    void SwingWeaponForce()
    { 
        Vector2 relativeMousePositionToPlayersChest = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ownersBody.chest.transform.position;
        rb.AddForce(relativeMousePositionToPlayersChest.normalized * weaponForce);
    }
    void SwingWeaponTorque()
    {
        Vector2 relativeMousePositionToPlayersChest = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ownersBody.chest.transform.position;
        rb.AddTorque(-relativeMousePositionToPlayersChest.normalized.x * weaponTorqueForce);
    }
    void ChangeBalance()
    {
        if (balance == null)
            return;

        // TESTING -- better way to auto flip within Zero?
        if (balance.targetRotation > 0)
            balance.targetRotation = 0;

        else if(balance.targetRotation == 0)
            balance.targetRotation = balanceRotationDefault;
    }
    public void ApplyDirectionalForce(Direction direction)
    {
        if (weaponDisabled)
            return;

        if (directionalForce == null)
            return;

        directionalForce.ApplyForce(direction);
    }
    void CheckSwordSwingSound()
    {
        swingSoundTimer += Time.deltaTime;

        if (swingSoundTimer >= 1)
        {
            //totalVelocityPerSwing += rb.velocity.magnitude / Time.deltaTime;
            //averageVelocityPerSecond += totalVelocityPerSwing / swingSoundTimer;
            totalVelocityPerSwing += rb.velocity.magnitude;
            averageVelocityPerSecond = totalVelocityPerSwing / swingSoundTimer * Time.deltaTime;
        }


        Debug.Log(averageVelocityPerSecond);

        if (swingSoundTimer > swingSoundTimerMax)
        {
            readyToPlaySwingSound = true;
        }

        if (!readyToPlaySwingSound)
            return;

        // if (rb.velocity.magnitude > 10)
        if (Mathf.Abs(averageVelocityPerSecond) > 5)
        {
            // ~ TODO: Improve with checking the sound playing last
            SoundManager.Inst.PlayRandomFromArray(SoundManager.Inst.swordSwings);
            readyToPlaySwingSound = false;
            swingSoundTimer = 0;
            totalVelocityPerSwing = 0f;
            averageVelocityPerSecond = 0f;
        }
    }


    public void DisableWeapon()
    {
        //Debug.Log("DisableWeapon");

        //DisablePhysicsMaterial();
      
        // Adjust State
        weaponDisabled = true;

        // Release Weapon from holder
        weaponsHinge.enabled = false;

        foreach (var link in weaponLinks)
        {
            Rigidbody2D _rb = link.GetComponent<Rigidbody2D>();
            _rb.mass = 4;
            _rb.gravityScale = 1f;
            link.layer = 8;
        }
        // Adjust gravit and mass so weapon falls
        rb.mass = 4;
        rb.gravityScale = 1f;

        // Set Weapon to WeaponIgnoreCollisionExceptGround (i.e. layer 8)
        gameObject.layer = 8;

        // Disable any "Force" Point Effectors on Weapon (i.e. cudgel)
        if (GetComponentInChildren<PointEffector2D>() != null)
            pointEffector2d.forceMagnitude = 0;
    }

    public void DisablePhysicsMaterial()
    {

        if (physicsMaterial != null)
        {
            Debug.Log("Bounciness " + physicsMaterial.bounciness);
            physicsMaterial.bounciness = 0;
            physicsMaterial.friction = 0;
            Debug.Log("Post Bounciness " + physicsMaterial.bounciness);

        }

    }

   
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Inst == null)
            return;

        if (weaponDisabled && collision.gameObject.tag == "Ground")
        {
            // Move into the groudn slightly (soem weapons have bigger colliders) ~ todo: not geat for laying flat
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            // ~ Performance Improvements?
            rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            if(GetComponent<CapsuleCollider2D>() != null)
                GetComponent<CapsuleCollider2D>().enabled = false;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Background";
            spriteRenderer.sortingOrder = -1;   // Sit just behind ground layer, to hide handles, etc.
            return;
        }

        if(GameManager.Inst != null)
        {
            if (GameManager.Inst.isRoundOver)
                return;
        }

        // Don't deal damage if not above threshhold (i.e. zero)
        if (collision.relativeVelocity.magnitude < 1)
            return;

        if (collision.gameObject.tag == "Ground")
            isGrounded = true;
            

        // magnitude used for Weapon Damage amount
        float dmgMagnitude = collision.relativeVelocity.magnitude;

        // Collision with Another Weapon
        if (collision.gameObject.tag == "CausesDamage")
        {
            return;
        }
        else if (collision.gameObject.tag == "Link")
        {
            /*
            // ~ TODO: Disable link hinges
            HingeJoint2D[] linkHinges = GetComponentsInChildren<HingeJoint2D>();
            foreach (var hinge in linkHinges)
            {
                if (hinge.gameObject.tag != "Link")
                    continue;

                Debug.Log("Hinge Name: " + hinge.name);
                if (hinge.attachedRigidbody.GetComponent<Weapon>() != null)
                {
                    hinge.attachedRigidbody.GetComponent<Weapon>().DisableWeapon();
                }

                hinge.enabled = false;              
            }
            */
            return;
            
        }
        else if (collision.gameObject.tag == "Weapon")
        {
            if(ContinueWithCollision(collision))
                WeaponCollision(collision, dmgMagnitude);
        }
        else
        {
            //Debug.Log("Weapon.cs 'else' collision tag: " + collision.gameObject.tag);

            // Don't let weapons do damage is lazy grounded
            if (isGrounded)
                return;

            // Check if it's actually a body part, or ground, or same team, etc.
            if (ContinueWithCollision(collision))
                BodyPartCollision(collision, dmgMagnitude);
        }     
    }

    void WeaponCollision(Collision2D collision, float dmgMagnitude)
    {
        // Create Sound
        SoundManager.Inst.PlayRandomFromArray(SoundManager.Inst.swordClashes);

        // Spawn Particles at Collision Location
        ParticleManager.Inst.SpawnSpriteAnimation(collision);
        //ParticleManager.Inst.PlayParticle(ParticleManager.Inst.particleSteel, dmgMagnitude, collision.transform);
        //ParticleManager.Inst.SpawnSpriteAnimation(collision.GetContact(0).point);
    }

    void BodyPartCollision(Collision2D collision, float dmgMagnitude)
    {

        //Debug.Log("collision.name: " + collision.gameObject.name);
        // SLow Time on Hit (slowdown for .1f seconds, time get cut in half)
        if (ownersBody.slowTimeOnHit)
            TimeManager.Inst.SlowTime(.05f, .8f, false);

        // Pass Magnitude as Damage, and pass Player Type (so if the body part is destroyed, GameManager can declare a winner)
        collision.gameObject.GetComponent<BodyPart>().TakeDamage(dmgMagnitude * dmgMultiplier, bleedDmg, ownersBody, weaponOwnerType, collision);

        // Deal damage first above, the spawn a Force Point Effector for a moment via coroutine
        if (pointEffector2d != null && dmgMagnitude >= 5)
        {
            float forceAmount = effectorForce * dmgMagnitude;
            EnableEffector(0.5f, forceAmount);
        }

        // Spawn Particles at Collision Location
        //ParticleManager.Inst.PlayParticle(ParticleManager.Inst.particleBlood, dmgMagnitude, collision.transform);

        // Play Sound
        //SoundManager.Inst.Play(SoundManager.Inst.playerHit);
        
    }

    bool ContinueWithCollision(Collision2D collision)
    {
        // Get the collision body
        if (collision.gameObject.GetComponentInParent<Body>() == null)
            return false;

        Body collisionPlayerBody = collision.gameObject.GetComponentInParent<Body>();

        // Check if there is a body part to damage, check if it's the same "Type" of Player (i.e. NPC can't harm their type)
        if (collisionPlayerBody.alive == false || collisionPlayerBody.playerType == weaponOwnerType)
        {
            return false;
        }

        // ~TODO: Update for 4 players
        if(GameManager.Inst != null)
        {
            if (GameManager.Inst.gameMode == eGameMode.Coop)
            {
                // ~ Did this cause bugs?
                if (GameManager.Inst.isPlayerTypePlayer(collisionPlayerBody.playerType) && GameManager.Inst.isPlayerTypePlayer(weaponOwnerType))
                    return false;
                
                /*
                if (collisionPlayerBody.playerType == Players.P1 && weaponOwnerType == Players.P2)
                    return false;

                if (collisionPlayerBody.playerType == Players.P2 && weaponOwnerType == Players.P1)
                    return false;
                */
            }
        }

        return true;
    }

    public void EnableEffector(float waitTimeAmount, float amount)
    {
        StartCoroutine(EnableEffectorCoroutine(waitTimeAmount, amount));
    }

    private IEnumerator EnableEffectorCoroutine(float waitTime, float amount)
    {
        pointEffector2d.forceMagnitude = amount;
        yield return new WaitForSeconds(waitTime);
        pointEffector2d.forceMagnitude = 0;
    }

    /*
    void DetermineDirectlyConenctedHinge()
    {
        HingeJoint2D[] hingeBodyParts = gameObject.GetComponentInParent<Movement>().GetComponentsInChildren<HingeJoint2D>();

        foreach (var hinge in hingeBodyParts)
        {
            if (hinge.connectedBody.gameObject == this.gameObject)
                weaponHolderHinge = hinge;
        }
    }
    */

}


