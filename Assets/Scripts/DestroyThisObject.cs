using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyThisObject : MonoBehaviour
{
    [Header("Fade Out")]
    public bool fadeOutOverTime;
    SpriteRenderer sprite;

    [Header("Fade/Destroy Timer")]
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        if (fadeOutOverTime)
        {
            sprite = GetComponent<SpriteRenderer>();
            StartCoroutine(FadeAlphaToZero(sprite, timer));
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)      
            Destroy(this.gameObject);
    }

    IEnumerator FadeAlphaToZero(SpriteRenderer renderer, float duration)
    {
        Color startColor = renderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            renderer.color = Color.Lerp(startColor, endColor, time / duration);
            yield return null;
        }
    }
}
