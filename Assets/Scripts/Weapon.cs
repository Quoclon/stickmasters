using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public Players weaponOwner;
    public Body ownersBody;
    public float dmgMultiplier = 1f;
    public bool weaponDisabled;

    [Header("Manual Swinging - Force")]
    public float weaponForce;
    private Vector2 SwingDirection;


    [Header("Manual Swinging - Torque")]
    public float weaponTorqueForce;

    [Header("Weapon RigidBody")]
    Rigidbody2D rb;

    [Header("Rigid Body")]
    public DirectionalForce directionalForce;

    [Header("Weapon Holder Hinge")]
    public HingeJoint2D weaponsHinge;
    public HingeJoint2D weaponHolderHinge;

    [Header("Weapon Sounds")]
    private bool readyToPlaySwingSound;
    private float swingSoundTimer;
    private float swingSoundTimerMax = 4f;

    // Start is called before the first frame update
    void Start()
    {     
        rb = GetComponent<Rigidbody2D>();
        weaponsHinge = GetComponent<HingeJoint2D>();
        weaponHolderHinge = weaponsHinge.connectedBody.GetComponent<HingeJoint2D>();
        ownersBody = GetComponentInParent<Body>();
        weaponOwner = ownersBody.playerType;  // ~ NEEDS WORK
        readyToPlaySwingSound = true;
        swingSoundTimer = swingSoundTimerMax;

        if (GetComponent<DirectionalForce>() != null)
            directionalForce = GetComponent<DirectionalForce>();


    }


    // Update is called once per frame
    void Update()
    {
        if (weaponDisabled)
            return;

        // ~ TODO: TEST/REMOVE
        if (Input.GetMouseButtonDown(0) && weaponOwner == Players.P1)
        {
            //SwingWeaponForce();
        }

        // ~ TODO: TEST/REMOVE
        if (Input.GetMouseButtonDown(1) && weaponOwner == Players.P1)
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
    }
 
    void CheckSwordSwingSound()
    {
        swingSoundTimer -= Time.deltaTime;

        if (swingSoundTimer < 0)
        {
            readyToPlaySwingSound = true;
            swingSoundTimer = swingSoundTimerMax;
        }

        if (rb.velocity.magnitude < 2)
        {
            //readyToPlaySwingSound = true;
        }

        if (!readyToPlaySwingSound)
            return;

        if (rb.velocity.magnitude > 10)
        {
            // ~ TODO: Improve with checking the sound playing last
            SoundManager.Inst.PlayRandomFromArray(SoundManager.Inst.swordSwings);
            readyToPlaySwingSound = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (weaponDisabled)
            return;

        if (GameManager.Inst.isGameOver)
            return;

        // Collision with Another Weapon
        if (collision.gameObject.tag == "Weapon")
        {
            WeaponCollision(collision);
        }
        else if(collision.gameObject.tag == "CausesDamage")
        {
            return;
        }
        else
        {
            BodyPartCollision(collision);
        }     
    }

    void WeaponCollision(Collision2D collision)
    {
        // ~ TODO: Knock weapons out of hands, or destroy, can pickup, or use fists, etc.
        float dmgMagnitude = collision.relativeVelocity.magnitude;

        // Create Sound
        SoundManager.Inst.PlayRandomFromArray(SoundManager.Inst.swordClashes);

        // Spawn Particles at Collision Location
        //ParticleManager.Inst.PlayParticle(ParticleManager.Inst.particleSteel, dmgMagnitude, collision.transform);
        ParticleManager.Inst.SpawnSpriteAnimation(collision);
        //ParticleManager.Inst.SpawnSpriteAnimation(collision.GetContact(0).point);
    }

    void BodyPartCollision(Collision2D collision)
    {
        float dmgMagnitude = collision.relativeVelocity.magnitude;

        Body collisionPlayerBody = collision.gameObject.GetComponentInParent<Body>();

        // Check if there is a body part to damage
        if (collisionPlayerBody.alive == false || collisionPlayerBody.playerType == weaponOwner)
        {
            //Debug.Log("No BodyPart Script Attached to this collision Object");
            return;
        }
        else
        {
            // SLow Time on Hit (slowdown for .1f seconds, time get cut in half)
            //TimeManager.Inst.SlowTime(.05f, .75f, false);

            // Pass Magnitude as Damage, and pass Player Type (so if the body part is destroyed, GameManager can declare a winner)
            collision.gameObject.GetComponent<BodyPart>().TakeDamage(dmgMagnitude * dmgMultiplier, weaponOwner);

            // Spawn Particles at Collision Location
            ParticleManager.Inst.PlayParticle(ParticleManager.Inst.particleBlood, dmgMagnitude, collision.transform);

            // Play Sound
            SoundManager.Inst.Play(SoundManager.Inst.playerHit);
        }
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
