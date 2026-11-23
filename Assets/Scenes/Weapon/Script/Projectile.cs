
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction, float speed)
    {
        if (rb == null)
        {
            Debug.LogError("Projectile needs a Rigidbody!");
            return;
        }

        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifetime);
    }
}
