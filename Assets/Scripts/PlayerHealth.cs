using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // Start is called before the first frame update
    private float health;
    private float shields;
    private float lerpTimer;
    public float maxHealth = 100;
    public float maxShields = 100;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public Image frontShieldBar;
    public Image backShieldBar;

    
    
    void Start()
    {
        health = maxHealth;
        shields = maxShields;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        shields = Mathf.Clamp(shields, 0, maxShields);
        UpdateSheildUI();
        UpdateHealthUI();
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(Random.Range(5, 10));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            RestoreHealth(Random.Range(5, 10));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            RestoreSheilds(Random.Range(5, 10));
        }
    }

    public void UpdateHealthUI()
    {
        Debug.Log("Health value at: " + health);
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if(fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.yellow;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete; //for animation purpose makes transition smoother
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if(fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete; //for animation purpose makes transition smoother
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }

    }

    public void UpdateSheildUI()
    {
        Debug.Log("Shield power at: " + shields);
        float fillF = frontShieldBar.fillAmount;
        float fillB = backShieldBar.fillAmount;
        float hFraction = shields / maxShields;
        if (fillB > hFraction)
        {
            frontShieldBar.fillAmount = hFraction;
            backShieldBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete; //for animation purpose makes transition smoother
            backShieldBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            backShieldBar.color = Color.green;
            backShieldBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete; //for animation purpose makes transition smoother
            frontShieldBar.fillAmount = Mathf.Lerp(fillF, backShieldBar.fillAmount, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        if (shields > 0)
        {
            shields -= damage;
            lerpTimer = 0f;
        }
        else { 
        health -= damage;
        lerpTimer = 0f;
        }
    }

    public void RestoreSheilds(float healAmount)
    {
        shields += healAmount;
        lerpTimer = 0f;

    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
            
     }


}
