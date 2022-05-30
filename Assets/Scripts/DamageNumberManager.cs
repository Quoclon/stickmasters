using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumberManager : MonoBehaviour
{
	#region Singleton
	// Singleton instance.
	public static DamageNumberManager Inst = null;

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

	public GameObject damageNumberPrefab;

	[Header("Static Offsets")]
	public float offsetX;
	public float offsetY;

	[Header("Random Offsets - X")]
	public float minRandomX;
	public float maxRandomX;

	private float randomOffsetToAddToX;

	[Header("Random Offsets - Y")]
	public float minRandomY;
	public float maxRandomY;

	private float randomOffsetToAddToY;

	// Start is called before the first frame update
	void Start()
    {
		// ~ EXPLANATION: Without this, the first time the dmgNumber spawns it creates a lot of Lag - Probably preloads the Prefab?
		GameObject dmgNumber = Instantiate(damageNumberPrefab, new Vector3(10000, 10000, 10000), Quaternion.identity);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SpawnDamageNumber(float dmgAmt, Players playerType, Transform dmgPlayerHead)
	{
		randomOffsetToAddToX = Random.Range(minRandomX, maxRandomX);
		Vector3 spawnLocationAboveHead = new Vector3(dmgPlayerHead.transform.position.x + offsetX + randomOffsetToAddToX, dmgPlayerHead.transform.position.y + offsetY + randomOffsetToAddToY, dmgPlayerHead.transform.position.z);
		GameObject dmgNumber = Instantiate(damageNumberPrefab, spawnLocationAboveHead, Quaternion.identity);

		
		TextMeshProUGUI damageText = dmgNumber.GetComponentInChildren<TextMeshProUGUI>();
		damageText.text = dmgAmt.ToString("F0");

		if (Players.P1 == playerType)
			damageText.color = Color.blue;
		else if (Players.AI == playerType)
			damageText.color = Color.red;
		else if (Players.Environment == playerType)
			damageText.color = Color.magenta;

		Destroy(dmgNumber, .25f);
	}
}
