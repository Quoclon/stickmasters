using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

	[Header("Particle Based")]
	public ParticleSystem particleBlood;
	public ParticleSystem[] bloodOnHitParticles;
	public int particleNumber = -1;

	[Header("Sprite Based")]
    public GameObject WeaponClashSprite;

	public float particleLifeSpan;

	// Control Emission based on Magnitude of Impact
	// Adjust color based on body part type
	// Control 

	#region Singleton
	// Singleton instance.
	public static ParticleManager Inst = null;

	// Initialize the singleton instance.
	private void Awake()
	{
		// If there is not already an instance of SoundManager, set it to this.
		if (Inst == null)
		{
			Inst = this;
		}
		//If an instance already exists, destroy whatever this object is to enforce the singleton.
		else if (Inst != this)
		{
			Destroy(gameObject);
		}

		//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad(gameObject);
	}
	#endregion

	// Start is called before the first frame update
	void Start()
    {

	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void PlayParticle(ParticleSystem particleTypeToInstantiate, float collisionMagnitude, Transform collisionPosition)
	{
		ParticleSystem particle = Instantiate(particleTypeToInstantiate, collisionPosition);

		// ~ TODO - FIgure out how to make the particles bigger - this is NOT working
		//particle.emissionRate += (collisionMagnitude * 1.5f);
	}

	public void PlayRandomParticle(ParticleSystem[] particleTypeToInstantiate, float collisionMagnitude, Collision2D collision)
	{
		int randomNumber = -1;
		if (particleNumber == -1)
			randomNumber = Random.Range(0, particleTypeToInstantiate.Length);
		else
			randomNumber = particleNumber;


		ParticleSystem particle = Instantiate(particleTypeToInstantiate[randomNumber], collision.contacts[0].point, collision.transform.rotation);
		//Debug.Log(particleTypeToInstantiate[randomNumber].name);

		// ~ TODO - FIgure out how to make the particles bigger - this is NOT working
		//particle.emissionRate += (collisionMagnitude * 1.5f);
	}



	// ~NOTE: BloodParticleController - controls the blood splat on ground; it's on the ParticleSystem with blood particles that collide and cause the Sprite
	public void SpawnSpriteAnimation(Collision2D collision)
    {
        GameObject animatedSprite = Instantiate(WeaponClashSprite, collision.contacts[0].point, Quaternion.identity);
    }
}
