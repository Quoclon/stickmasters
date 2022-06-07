using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    #region Singleton
    // Singleton instance.
    public static MainMenuManager Inst = null;

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

    public eGameMode gameMode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSinglePlayerMode()
    {
        gameMode = eGameMode.SinglePlayer;
    }

    public void SetMultiplayerPlayerMode()
    {
        gameMode = eGameMode.MultiPlayer;
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
