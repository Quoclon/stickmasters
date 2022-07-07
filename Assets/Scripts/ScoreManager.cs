using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
	#region Singleton
	// Singleton instance.
	public static ScoreManager Inst = null;

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

	public int scoreP1 = 0;
	public int scoreP2 = 0;
	public int scoreP3 = 0;
	public int scoreP4 = 0;


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
