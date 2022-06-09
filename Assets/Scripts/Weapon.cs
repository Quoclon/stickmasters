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

    [Header("Weapon RigidBody")]
    Rigidbody2D rb;

    [Header("Rigid Body")]
    public DirectionalForce directionalForce;

    [Header("Weapon Holder Hinge")]
    public HingeJoint2D weaponsHinge;
    public HingeJoint2D weaponHolderHinge;

    [Header("Weapon Sounds")]
    private bool readyToPlaySwingSound;
    private float swingSoundTimer = 0f;
    private float swingSoundTimerMax = 2f;
    private float totalVelocityPerSwing = 0;
    private float averageVelocityPerSecond = 0f;

    public void SetupWeapon()
    {
        // ~ TESTING
        balanceRotationDefault = 180f; /// testing

        // Rigid Body
        rb = GetComponent<Rigidbody2D>();

        // Setup Hinge
        weaponsHinge = GetComponent<HingeJoint2D>();
        //weaponsHinge.connectedBody = gameObject.GetComponentInParent<Rigidbody2D>();  // Already Setup 
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

        // Directional Force
        if (GetComponent<DirectionalForce>() != null)
            directionalForce = GetComponent<DirectionalForce>();

        if (GetComponentInChildren<PointEffector2D>() != null)
            pointEffector2d = GetComponentInChildren<PointEffector2D>();

        // Collision Layers
        if (ownersBody.weaponCollidesWithGround)
            gameObject.layer = 10;
    }

    public void SetupWeaponComplicated(GameObject bodyPartGO)
    {
        rb = GetComponent<Rigidbody2D>();

        // Setup Hinge
        weaponsHinge = GetComponent<HingeJoint2D>();
        weaponsHinge.connectedBody = bodyPartGO.GetComponent<Rigidbody2D>();

        //
        weaponHolderHinge = weaponsHinge.connectedBody.GetComponent<HingeJoint2D>();


        ownersBody = GetComponentInParent<Body>();
        weaponOwnerType = ownersBody.playerType;  // ~ NEEDS WORK
        readyToPlaySwingSound = true;
        swingSoundTimer = swingSoundTimerMax;

        if (GetComponent<DirectionalForce>() != null)
            directionalForce = GetComponent<DirectionalForce>();

        if (ownersBody.weaponCollidesWithGround)
            gameObject.layer = 8;
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
            ChangeBalance();
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

    public void DisableWeapon()
    {
        //Debug.Log("DisableWeapon");

        // Adjust State
        weaponDisabled = true;

        // Release Weapon from holder
        weaponsHinge.enabled = false;

        // Adjust gravit and mass so weapon falls
        rb.mass = 4;
        rb.gravityScale = 1f;

        // Set Weapon to WeaponIgnoreCollisionExceptGround (i.e. layer 8)
        gameObject.layer = 8;

        // Disable any "Force" Point Effectors on Weapon (i.e. cudgel)
        if (GetComponentInChildren<PointEffector2D>() != null)
            pointEffector2d.forceMagnitude = 0;
    }

    void CheckSwordSwingSound()
    {
        swingSoundTimer += Time.deltaTime;

        if(swingSoundTimer >= 1)
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
       if(Mathf.Abs(averageVelocityPerSecond) > 5)
        {
            // ~ TODO: Improve with checking the sound playing last
            SoundManager.Inst.PlayRandomFromArray(SoundManager.Inst.swordSwings);
            readyToPlaySwingSound = false;
            swingSoundTimer = 0;
            totalVelocityPerSwing = 0f;
            averageVelocityPerSecond = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (weaponDisabled)
            return;

        if (GameManager.Inst.isRoundOver)
            return;

        // Don't deal damage if not above threshhold (i.e. zero)
        if(collision.relativeVelocity.magnitude < 1)
            return;

        // magnitude used for Weapon Damage amount
        float dmgMagnitude = collision.relativeVelocity.magnitude;

        // Collision with Another Weapon
        if (collision.gameObject.tag == "Weapon")
        {
            WeaponCollision(collision, dmgMagnitude);
        }
        else if(collision.gameObject.tag == "CausesDamage")
        {
            return;
        }
        else
        {
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
        // Get the collision body
        if (collision.gameObject.GetComponentInParent<Body>() == null)
            return;
        Body collisionPlayerBody = collision.gameObject.GetComponentInParent<Body>();

        // Check if there is a body part to damage, check if it's the same "Type" of Player (i.e. Player or NPC can't harm their type)
        if (collisionPlayerBody.alive == false || collisionPlayerBody.playerType == weaponOwnerType)
        {
            return;
        }
        else
        {
            // SLow Time on Hit (slowdown for .1f seconds, time get cut in half)
            if (ownersBody.slowTimeOnHit)
                TimeManager.Inst.SlowTime(.05f, .8f, false);

            // Pass Magnitude as Damage, and pass Player Type (so if the body part is destroyed, GameManager can declare a winner)
            collision.gameObject.GetComponent<BodyPart>().TakeDamage(dmgMagnitude * dmgMultiplier, ownersBody, weaponOwnerType);

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


    void DetermineDirectlyConenctedHinge()
    {
        HingeJoint2D[] hingeBodyParts = gameObject.GetComponentInParent<Movement>().GetComponentsInChildren<HingeJoint2D>();

        foreach (var hinge in hingeBodyParts)
        {
            if (hinge.connectedBody.gameObject == this.gameObject)
                weaponHolderHinge = hinge;
        }
    }

}
