using UnityEngine;

public enum WeaponType
{
    SwordShield, 
    Bow,
    Snowball,
    Rock,
    Sword,
    Shield
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Inventory/Weapon")]
public class WeaponData : ItemData
{
    [Header("Weapon Information")]
    public WeaponType weaponType;

    [Header("Weapon Stats")]
    public float damage = 10f;

    [Header("Weapon Models")]
    public GameObject handPrefab;
    public GameObject offHandPrefab;
    public GameObject storedPrefab;
    public GameObject secondaryStoredPrefab;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;

    [Header("Sword & Shield Animations")]
    public string swordUnsheatheAnimation;
    public string shieldUnsheatheAnimation;
    public string swordSheatheAnimation;
    public string shieldSheatheAnimation;
    public string idleAnimation;
    public string combatIdleAnimation;
    public string[] attackAnimations;
    public string blockAnimation = "BlockShield";

    [Header("Bow Animations")]
    public string bowUnsheatheAnimation;
    public string bowSheatheAnimation;
    public string bowIdleAnimation;
    public string arrowUnsheatheAnimation;
    public string bowCombatIdleAnimation;
    public string bowLoadAnimation;
    public string bowHoldAnimation;
    public string bowReleaseAnimation;

    [Header("Arrow Hand Model")]
    public GameObject arrowHandPrefab;

    [Header("Ammunition")]
    public ItemData ammunitionData;
    public int ammoRequiredPerShot = 1;

    [Header("Throwable Animations")]
    public string throwAnimation = "ThrowBall";
}
