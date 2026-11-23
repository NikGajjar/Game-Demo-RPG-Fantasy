
using UnityEngine;

public enum ArrowMode
{
    Pickup,
    Projectile,
    Held
}

public class ArrowScript : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private ArrowMode mode;

    [SerializeField] private float arrowSpeed = 30f;
    [SerializeField] private float arrowLifetime = 10f;

    [Header("Collision Settings")]
    [SerializeField] private float collisionRadius = 0.025f;

    private bool hasHit;
    private float damageAmount = 5f;

    private Transform ownerRoot;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        hasHit = false;
    }

    private void FixedUpdate()
    {

        if (mode != ArrowMode.Projectile || hasHit)
            return;

        Vector3 direction = transform.forward;
        float distance = arrowSpeed * Time.fixedDeltaTime;

        // Detect objects between current and next position.
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            collisionRadius,
            direction,
            distance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide
        );

        // Find the closest valid collision.
        float closestDistance = float.MaxValue;
        RaycastHit closestHit = new RaycastHit();
        bool foundHit = false;

        foreach (RaycastHit hit in hits)
        {
            // Ignore other arrow objects.
            ArrowScript otherArrow =
                hit.collider.GetComponentInParent<ArrowScript>();

            if (otherArrow != null)
                continue;

            if (ownerRoot != null &&
                hit.collider.transform.root == ownerRoot)
                continue;

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestHit = hit;
                foundHit = true;
            }
        }

        if (foundHit)
        {
            transform.position = closestHit.point;
            HandleHit(closestHit.collider);
            return;
        }

        Vector3 nextPosition =
            transform.position + direction * distance;

        if (rb != null && rb.isKinematic)
            rb.MovePosition(nextPosition);
        else
            transform.position = nextPosition;
    }

    public void SetProjectile(
        float speed,
        float damage,
        Transform owner)
    {
        mode = ArrowMode.Projectile;
        arrowSpeed = speed;
        damageAmount = damage;

        ownerRoot = owner != null ? owner.root : null;

        hasHit = false;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Destroy(gameObject, arrowLifetime);
    }

    public void SetHeld()
    {
        mode = ArrowMode.Held;
        hasHit = false;
    }

    public void SetPickup()
    {
        mode = ArrowMode.Pickup;
        hasHit = false;
    }

    private void HandleHit(Collider other)
    {
        if (hasHit)
            return;

        // Ignore the shooter.
        if (ownerRoot != null &&
            other.transform.root == ownerRoot)
            return;

        hasHit = true;

        // Check shield first.
        ShieldBlock shield =
            other.GetComponentInParent<ShieldBlock>();

        if (shield != null && shield.IsBlocking)
        {
            Debug.Log("Arrow blocked by shield!");

            Destroy(gameObject);
            return;
        }

        // Check target health.
        Health targetHealth =
            other.GetComponentInParent<Health>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damageAmount);

            Debug.Log(
                "Arrow dealt " + damageAmount +
                " damage to " + targetHealth.gameObject.name
            );
        }
        else
        {
            Debug.Log("Arrow hit: " + other.name);
        }

        // Destroy on walls, shields, enemies and other objects.
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mode == ArrowMode.Pickup)
        {
            if (other.CompareTag("Player"))
                Destroy(gameObject);

            return;
        }

        if (mode != ArrowMode.Projectile || hasHit)
            return;
        // Ignore collisions with other arrows.
        ArrowScript otherArrow =
            other.GetComponentInParent<ArrowScript>();

        if (otherArrow != null && otherArrow != this)
            return;
        if (ownerRoot != null &&
            other.transform.root == ownerRoot)
            return;

        HandleHit(other);
    }
}
