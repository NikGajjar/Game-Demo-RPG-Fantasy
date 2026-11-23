
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private WeaponData weaponData;

    [SerializeField, Min(0f)]
    private float customDamage = 10f;

    private bool attackActive;
    private bool hasHitTarget;

    private Transform ownerRoot;

    private void Awake()
    {
        ownerRoot = transform.root;
    }

    public void BeginAttack()
    {
        attackActive = true;
        hasHitTarget = false;
    }

    public void EndAttack()
    {
        attackActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!attackActive || hasHitTarget)
            return;

        if (other.transform.root == ownerRoot)
            return;

        // Shield gets first opportunity to block.
        ShieldBlock shield = other.GetComponentInParent<ShieldBlock>();

        if (shield != null && shield.IsBlocking)
        {
            hasHitTarget = true;

            Debug.Log("Sword attack blocked by shield!");

            return;
        }

        // Find the target's Health component.
        Health targetHealth = other.GetComponentInParent<Health>();

        if (targetHealth == null)
            return;

        float damageAmount = weaponData != null? weaponData.damage : customDamage;

        hasHitTarget = true;

        targetHealth.TakeDamage(damageAmount);

        Debug.Log("Sword dealt " + damageAmount + " damage.");
    }
}
