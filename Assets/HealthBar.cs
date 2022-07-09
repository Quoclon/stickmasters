using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Managed by Body? Health should be in body?
    // This just manages healthbar?
    //float health;
    //float healthMax;
    public Slider slider;
    public Transform playerChestTransform;

    [Header("Fading")]
    [SerializeField] private CanvasGroup canvasGroupUI;
    [SerializeField] private bool playerDead = false;
    [SerializeField] private bool fadeIn = false;
    [SerializeField] private bool fadeOut = false;

    [Header("Bleeding")]
    public GameObject bleedingHeartPanel;
    public Animator bleedingHeartAnimator;


    // Start is called before the first frame update
    void Start()
    {
        playerChestTransform = GetComponent<Body>().chest.transform;
        SetupBleedingHeart();
    }

    void SetupBleedingHeart()
    {
        bleedingHeartAnimator.speed = 0f;
        bleedingHeartPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
  
        if (playerDead)
            return;

        slider.transform.position = playerChestTransform.position + new Vector3(0, 2, 0);

        if (fadeIn)
        {
            if(canvasGroupUI.alpha < 1)
            {
                canvasGroupUI.alpha += Time.deltaTime;
                if (canvasGroupUI.alpha >= 1)
                    fadeIn = false;
            }
        }

        if (fadeOut)
        {
            if (canvasGroupUI.alpha >= 0)
            {
                canvasGroupUI.alpha -= Time.deltaTime;
                if (canvasGroupUI.alpha == 0)
                    fadeOut = false;
            }
        }
    }

    public void ShowUI()
    {
        fadeIn = true;
    }

    public void HideUI()
    {
        fadeOut = true;
    }

    public void ShowHealthBar()
    {
        canvasGroupUI.alpha = 1;
        fadeOut = true;
    }

    public void DisableUI()
    {
        playerDead = true;
        canvasGroupUI.alpha = 0;
    }
    public void SetHealthBarCurrentHealth(float _currentHealth)
    {
        slider.value = _currentHealth;
        ShowHealthBar();
        //StartCoroutine(waitBeforeFading(2f));
    }

    public void SetHealthBarDefaultSetup(float _maxHealth)
    {
        slider.maxValue = _maxHealth;
        slider.value = _maxHealth;
        HideUI();
        //StartCoroutine(waitBeforeFading(.5f));
    }

    // Fast Animation based on more damage
    public void SetBleedAnimation(float animationSpeed)
    {
        bleedingHeartPanel.gameObject.SetActive(true);
        bleedingHeartAnimator.speed = animationSpeed;
        //Debug.Log("animationSpeed: " + animationSpeed);
    }


    IEnumerator waitBeforeFading(float waitTime)
    {
        ShowUI();
        yield return new WaitForSeconds(waitTime);
        HideUI();
    }

}
