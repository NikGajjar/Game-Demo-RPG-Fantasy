
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Range(1f, 500f)]
    private float maxHealth = 100f;

    [SerializeField]
    private float currentHealth;

    [Header("Respawn Settings")]
    [SerializeField] private Transform respawnPoint;

    private bool isDead;

    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private CombatController combatController;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        combatController = GetComponent<CombatController>();

        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f)
            return;

        currentHealth = Mathf.Clamp(
            currentHealth - damage, 0f, maxHealth
        );

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            isDead = true;

            if (playerMovement != null)
                playerMovement.LockMovement();

            OnDeath?.Invoke();

            if (combatController != null)
                combatController.enabled = false;

            Debug.Log(gameObject.name + " died!");
        }
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0f)
            return;

        currentHealth = Mathf.Clamp(
            currentHealth + amount, 0f, maxHealth
        );

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Respawn()
    {
        if (!isDead)
            return;

        if (respawnPoint == null)
        {
            Debug.LogError("Respawn Point is not assigned!");
            return;
        }

        transform.SetPositionAndRotation(
            respawnPoint.position,
            respawnPoint.rotation
        );

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        currentHealth = maxHealth;
        isDead = false;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (combatController != null)
            combatController.enabled = true;

        if (playerMovement != null)
            playerMovement.UnlockMovement();
    }
}
