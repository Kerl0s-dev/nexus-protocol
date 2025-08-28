using UnityEngine;

public class Health : MonoBehaviour
{
    CharacterMovement CM;

    public int maxHealth = 100;
    public int currentHealth;
    public bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CM = GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player takes " + damage + " damage. Current health: " + currentHealth);
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player has died.");
        CM.enabled = false; // Disable character movement
        // Here you can add death animation, sound, or respawn logic
    }
}
