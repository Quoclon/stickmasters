using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	#region Singleton
	// Singleton instance.
	public static TimeManager Inst = null;

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

	public void SlowTime(float waitTimeAmount, float slowdownAmount, bool bodyPartDestroyed)
	{
		// Coroutine for Slow Mo on Hit
		//if (Time.timeScale == 1 || bodyPartDestroyed)
		StartCoroutine(SlowTimeCoroutine(waitTimeAmount, slowdownAmount));
	}

	private IEnumerator SlowTimeCoroutine(float waitTime, float slowdownAmount)
	{
		Time.timeScale = slowdownAmount;
		yield return new WaitForSeconds(waitTime);
		Time.timeScale = 1f;
	}
}
