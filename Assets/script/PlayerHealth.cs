

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 150;
    private int currentHealth;

    [Header("Scene Settings")]
    public string nextSceneName;

    [Header("UI Settings")]
    public Text healthText;

    [Header("Hurt Effect")]
    public GameObject hurtEffectUI; // Assign your hurt effect GameObject in the Canvas

    private float hurtThreshold => maxHealth * 0.4f; // 40% threshold

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        UpdateHurtEffect(); // initialize state
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Player HP: " + currentHealth);
        UpdateHealthUI();
        UpdateHurtEffect();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Player healed! Current health: " + currentHealth);
        UpdateHealthUI();
        UpdateHurtEffect(); // update hurt effect if health is now higher
    }

    public void SetHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthUI();
        UpdateHurtEffect();
    }


    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
    }

    void UpdateHurtEffect()
    {
        if (hurtEffectUI != null)
        {
            hurtEffectUI.SetActive(currentHealth <= hurtThreshold);
        }
    }

    void Die()
    {
        Debug.Log("Player died! Loading next scene...");

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveHighScore();
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
