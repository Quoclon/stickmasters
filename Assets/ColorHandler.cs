using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHandler : MonoBehaviour
{
    //SpriteRenderer sprite;
    public Color[] playerColors;
    public Color[] npcColors;


    // Start is called before the first frame update
    void Start()
    {
        //sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public Color GetColorByLevel(int level)
    {
        if (level > npcColors.Length)
            return npcColors[npcColors.Length-1];
        else
            return npcColors[level - 1];
    }

    public Color GetColorByWave(int wave)
    {
        if (wave > npcColors.Length)
            return npcColors[npcColors.Length-1];
        else
            return npcColors[wave - 1];
    }

    public Color GetPlayerColor(int playerNumber)
    {
        if (playerNumber > npcColors.Length)
            return playerColors[playerColors.Length - 1];
        else
            return playerColors[playerNumber - 1];
    }
}
