
using UnityEngine;

public class ShieldBlock : MonoBehaviour
{
    [Header("Shield HitBox")]
    [SerializeField] private Collider shieldCollider;

    public bool IsBlocking { get; private set; }

    private void Awake()
    {
        if (shieldCollider == null)
            shieldCollider = GetComponentInChildren<Collider>(true);

        SetBlocking(false);
    }
    private void Update()
    {
        shieldCollider = GetComponentInChildren<Collider>(true);
    }
    public void SetBlocking(bool blocking)
    {
        IsBlocking = blocking;

        if (shieldCollider != null)
            shieldCollider.enabled = blocking;
    }
}
