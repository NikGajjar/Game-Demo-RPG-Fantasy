using System.Collections.Generic;
using UnityEngine;

public class DamageTest : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageInterval = 1f;

    private Dictionary<Health, float> nextDamageTime = new Dictionary<Health, float>();

    private void OnTriggerStay(Collider other)
    {
        Health targetHealth = other.GetComponentInParent<Health>();

        if (targetHealth == null)
            return;

        if (Time.time >= GetNextDamageTime(targetHealth))
        {
            targetHealth.TakeDamage(damageAmount);

            nextDamageTime[targetHealth] = Time.time + damageInterval;

            Debug.Log("Test Zone dealt " + damageAmount + " damage to " + targetHealth.name);
        }
    }

    private float GetNextDamageTime(Health target)
    {
        if (!nextDamageTime.ContainsKey(target))
            return 0f;

        return nextDamageTime[target];
    }

    private void OnTriggerExit(Collider other)
    {
        Health targetHealth = other.GetComponentInParent<Health>();

        if (targetHealth != null)
            nextDamageTime.Remove(targetHealth);
    }
}
