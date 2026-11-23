
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private AnimScript playerAnimation;
    [SerializeField] private Camera aimCamera;

    [Header("Projectile Spawn")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private Transform arrowHandHolder;
    [SerializeField] private Transform bowArrowPoint;

    private bool bowArrowReleased;

    [Header("Hand Attachments")]
    [SerializeField] private Transform rightHandHolder;
    [SerializeField] private Transform leftHandHolder;

    [Header("Stored Weapon Holders")]
    [SerializeField] private Transform sideHolder;
    [SerializeField] private Transform backHolder;

    private GameObject currentHandObject;
    private WeaponData equippedWeapon;

    private bool projectileReleased;
    private bool isThrowing;
    private bool isBlocking;
    private bool isAttacking;
    private bool isBowReleasing;

    private int attackIndex;

    public WeaponData EquippedWeapon => equippedWeapon;
    private GameObject currentArrowObject;
    private Damage currentSwordDamage;
    private ShieldBlock currentShieldBlock;



    // INITIALIZATION



    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponentInParent<PlayerMovement>();

        if (playerMovement == null)
            playerMovement = GetComponentInChildren<PlayerMovement>(true);

        if (inventory == null)
            inventory = GetComponentInParent<PlayerInventory>();

        if (playerAnimation == null)
            playerAnimation = GetComponent<AnimScript>();

        if (playerAnimation == null)
            playerAnimation = GetComponentInChildren<AnimScript>(true);

        if (aimCamera == null)
            aimCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (playerMovement == null)
            playerMovement = GetComponentInParent<PlayerMovement>();

        if (playerMovement == null)
            playerMovement = GetComponentInChildren<PlayerMovement>(true);

        if (playerMovement == null)
        {
            Debug.LogError("CombatController: PlayerMovement missing! Check the Player hierarchy.");
            return;
        }

        playerMovement.OnBlock += HandleBowBlock;
        playerMovement.OnAttack += HandleAttack;
        playerMovement.OnBlock += HandleBlock;
        playerMovement.OnBlockReleased += HandleBlockReleased;
        playerMovement.OnDrop += HandleDrop;

        playerMovement.OnSelectSword += SelectSword;
        playerMovement.OnSelectShield += SelectShield;
        playerMovement.OnSelectBow += SelectBow;
        playerMovement.OnSelectThrowable += SelectThrowable;
    }

    private void OnDisable()
    {
        if (playerMovement == null)
            return;

        // Unsubscribe from events
        playerMovement.OnBlock  -= HandleBowBlock;
        playerMovement.OnAttack -= HandleAttack;
        playerMovement.OnBlock -= HandleBlock;
        playerMovement.OnBlockReleased -= HandleBlockReleased;

        playerMovement.OnDrop -= HandleDrop;

        playerMovement.OnSelectSword -= SelectSword;
        playerMovement.OnSelectShield -= SelectShield;
        playerMovement.OnSelectBow -= SelectBow;
        playerMovement.OnSelectThrowable -= SelectThrowable;
    }


    // WEAPON SELECTION

    private bool CanChangeWeapon()
    {
        return inventory != null &&  playerAnimation != null && !isThrowing && !isAttacking && !isBlocking;
    }

    private void SelectSword()
    {
        if (CanChangeWeapon()) EquipWeapon(inventory.SelectedSword);
    }

    private void SelectShield()
    {
        if (CanChangeWeapon()) EquipWeapon(inventory.SelectedShield);
    }

    private void SelectBow()
    {
        if (CanChangeWeapon()) EquipWeapon(inventory.SelectedBow);
    }

    private void SelectThrowable()
    {
        if (CanChangeWeapon())EquipWeapon(inventory.SelectedThrowable);
    }


    // EQUIP 

    private void EquipWeapon(WeaponData weapon)
    {
        if (weapon == null)
        {
            Debug.Log("Weapon not available in inventory.");
            return;
        }

        if (inventory.GetItemCount(weapon) <= 0)
        {
            Debug.Log("No quantity available: " + weapon.itemName);
            return;
        }

        if (equippedWeapon == weapon)
        {
            UnequipWeapon();
            return;
        }

        UnequipWeapon();

        equippedWeapon = weapon;

        Transform targetHolder = GetHandHolder(weapon);

        if (weapon.handPrefab != null && targetHolder != null)
            SpawnHandObject(weapon.handPrefab, targetHolder);

        string idleName = GetCombatIdleName(weapon);

        playerAnimation.SetCombatIdleAnimation(idleName);

        PlayEquipAnimation(weapon);

        Debug.Log("Equipped: " + weapon.itemName);
    }

    private string GetCombatIdleName(WeaponData weapon)
    {
        if (weapon.weaponType == WeaponType.Bow)
            return weapon.bowIdleAnimation;

        if (!string.IsNullOrEmpty(weapon.combatIdleAnimation))
            return weapon.combatIdleAnimation;

        return weapon.idleAnimation;
    }

    private Transform GetHandHolder(WeaponData weapon)
    {
        switch (weapon.weaponType)
        {
            case WeaponType.Sword:
                return rightHandHolder;

            case WeaponType.Shield:
                return leftHandHolder;

            case WeaponType.Bow:
                return leftHandHolder;

            case WeaponType.Snowball:
            case WeaponType.Rock:
                return rightHandHolder;

            default:
                return rightHandHolder;
        }
    }

    private void SpawnHandObject(GameObject prefab, Transform holder)
    {
        ClearHandObject();

        currentHandObject = Instantiate(prefab, holder);

        currentHandObject.transform.localPosition = Vector3.zero;
        currentHandObject.transform.localRotation = prefab.transform.localRotation;
        currentHandObject.transform.localScale = Vector3.one;

        // Find sword damage component
        currentSwordDamage =
            currentHandObject.GetComponentInChildren<Damage>(true);

        if (currentSwordDamage != null)
            currentSwordDamage.EndAttack();

        // Find shield blocking component
        currentShieldBlock = currentHandObject.GetComponentInChildren<ShieldBlock>(true);

        if (currentShieldBlock != null)
            currentShieldBlock.SetBlocking(false);
    }

    private void ClearHandObject()
    {
        if (currentHandObject != null)
        {
            Destroy(currentHandObject);
            currentHandObject = null;
        }

        currentSwordDamage = null;
        currentShieldBlock = null;
    }


    // EQUIP 

    private void PlayEquipAnimation(WeaponData weapon)
    {
        string animationName = "";

        switch (weapon.weaponType)
        {
            case WeaponType.Sword:
                animationName = weapon.swordUnsheatheAnimation;
                break;

            case WeaponType.Shield:
                animationName = weapon.shieldUnsheatheAnimation;
                break;

            case WeaponType.Bow:
                animationName = weapon.bowUnsheatheAnimation;
                    
                break;

            case WeaponType.Snowball:
            case WeaponType.Rock:
                animationName = weapon.combatIdleAnimation;
                break;
        }

        if (!string.IsNullOrEmpty(animationName))
            playerAnimation.ChangeAnimation(animationName);
        else
            playerAnimation.ResumeLocomotion();
    }


    // UNEQUIP WEAPON


    private void UnequipWeapon()
    {
        if (equippedWeapon == null)
            return;

        WeaponData oldWeapon = equippedWeapon;

        string animationName = "";

        switch (oldWeapon.weaponType)
        {
            case WeaponType.Sword:
                animationName = oldWeapon.swordSheatheAnimation;
                break;

            case WeaponType.Shield:
                animationName = oldWeapon.shieldSheatheAnimation;
                break;

            case WeaponType.Bow:
                animationName = oldWeapon.bowSheatheAnimation;
                break;
        }

        ClearHandObject();
        ClearHeldArrow();

        equippedWeapon = null;

        projectileReleased = false;
        isThrowing = false;
        isBlocking = false;
        isAttacking = false;

        attackIndex = 0;

        playerAnimation.ClearCombatIdleAnimation();
        playerAnimation.SetExternalAnimationControl(false);

        if (!string.IsNullOrEmpty(animationName))
            playerAnimation.ChangeAnimation(animationName);
        else
            playerAnimation.ResumeLocomotion();
    }


    // DROP WEAPON


    private void HandleDrop()
    {
        if (isThrowing || isAttacking || isBlocking)
            return;

        if (equippedWeapon == null)
            return;

        WeaponData weapon = equippedWeapon;

        UnequipWeapon();

        inventory.DropItem(weapon, 1);
    }


    // ATTACK


    private void HandleAttack()
    {
        if (equippedWeapon == null || isThrowing || isAttacking || isBlocking)
            return;

        switch (equippedWeapon.weaponType)
        {
            case WeaponType.Sword:
                PlayAttack();
                break;

            case WeaponType.Snowball:
            case WeaponType.Rock:
                ThrowItem();
                break;
        }
    }

    private void PlayAttack()
    {
        if (equippedWeapon.attackAnimations == null || equippedWeapon.attackAnimations.Length == 0)
            return;

        string animationName = equippedWeapon.attackAnimations[attackIndex];

        isAttacking = true;
        if (currentSwordDamage != null)
            currentSwordDamage.BeginAttack();

        // Stop player movement during attack
        playerMovement.LockMovement();

        playerAnimation.SetExternalAnimationControl(true);
        playerAnimation.ChangeAnimation(animationName);

        attackIndex++;

        if (attackIndex >= equippedWeapon.attackAnimations.Length)
            attackIndex = 0;
    }

    // Called by OnEnd on combat animation states.
    public void OnCombatAnimationFinished()
    {
        if (isThrowing)
        {
            OnThrowAnimationFinished();
            return;
        }

        if (isAttacking)
        {
            isAttacking = false;

            if (currentSwordDamage != null)
                currentSwordDamage.EndAttack();

            playerMovement.UnlockMovement();
        }

        if (isBowReleasing)
        {
            isBowReleasing = false;

            playerMovement.UnlockMovement();
            playerAnimation.ResumeFromPlayerInput();
            return;
        }

        playerAnimation.ResumeFromPlayerInput();
    }

    // SHIELD BLOCK


    private void HandleBlock()
    {
        if (equippedWeapon == null ||equippedWeapon.weaponType != WeaponType.Shield ||isThrowing || isAttacking)
            return;

        isBlocking = true;

        playerMovement.LockMovement();

        if (currentShieldBlock != null)
            currentShieldBlock.SetBlocking(true);

        playerAnimation.SetExternalAnimationControl(true);

        if (!string.IsNullOrEmpty(equippedWeapon.blockAnimation))
            playerAnimation.ChangeAnimation(equippedWeapon.blockAnimation);
    }

    private void HandleBlockReleased()
    {
        if (equippedWeapon == null)
            return;

        if (equippedWeapon.weaponType == WeaponType.Shield && isBlocking)
        {
            isBlocking = false;
            if (currentShieldBlock != null)
                currentShieldBlock.SetBlocking(false);

            // Allow movement again
            playerMovement.UnlockMovement();

            playerAnimation.ResumeLocomotion();
        }

        if (equippedWeapon.weaponType == WeaponType.Bow)
        {
            if (currentArrowObject == null) return;

            PlayBowRelease();
        }
    }


    // BOW



    private void HandleBowBlock()
    {
        if (equippedWeapon == null || equippedWeapon.weaponType != WeaponType.Bow || isThrowing || isAttacking || isBowReleasing)
            return;

        if (equippedWeapon.ammunitionData == null)
        {
            Debug.LogWarning("Bow ammunition data is missing!");
            return;
        }

        if (inventory.GetItemCount(equippedWeapon.ammunitionData) < equippedWeapon.ammoRequiredPerShot)
        {
            Debug.Log("Not enough arrows!");
            return;
        }

        playerMovement.LockMovement();

        bowArrowReleased = false;

        SpawnHeldArrow();

        PlayBowLoad();
    }


    private void SpawnHeldArrow()
    {
        if (currentArrowObject != null)
            return;

        if (equippedWeapon.arrowHandPrefab == null)
        {
            Debug.LogWarning("Arrow Hand Prefab is missing!");
            return;
        }

        if (arrowHandHolder == null)
        {
            Debug.LogWarning("Arrow Hand Holder is not assigned!");
            return;
        }

        currentArrowObject = Instantiate( equippedWeapon.arrowHandPrefab, arrowHandHolder );

        currentArrowObject.transform.localPosition = Vector3.zero;

        currentArrowObject.transform.localRotation =equippedWeapon.arrowHandPrefab.transform.localRotation;

        currentArrowObject.transform.localScale = Vector3.one;

        ArrowScript arrowScript = currentArrowObject.GetComponent<ArrowScript>();

        if (arrowScript != null) arrowScript.SetHeld();

        Debug.Log("Held arrow spawned.");
    }


    private void ClearHeldArrow()
    {
        if (currentArrowObject != null)
        {
            Destroy(currentArrowObject);
            currentArrowObject = null;
        }
    }

    private void FireBowArrow()
    {
        if (bowArrowReleased)
            return;

        if (currentArrowObject == null)
        {
            Debug.LogWarning("No held arrow!");
            return;
        }

        if (equippedWeapon.projectilePrefab == null || equippedWeapon.ammunitionData == null || bowArrowPoint == null)
        {
            Debug.LogWarning("Bow projectile references are missing!");
            return;
        }

        int ammoRequired = equippedWeapon.ammoRequiredPerShot;

        if (inventory.GetItemCount(equippedWeapon.ammunitionData) < ammoRequired)
        {
            Debug.LogWarning("Not enough arrows!");
            return;
        }

        if (aimCamera == null)
        {
            Debug.LogError("Aim Camera is missing!");
            return;
        }

        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 aimPoint = ray.origin + ray.direction * 200f;

        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            aimPoint = hit.point;
        }

        Vector3 direction = (aimPoint - bowArrowPoint.position).normalized;

        Quaternion aimRotation = Quaternion.LookRotation(direction);

        Quaternion arrowRotation = aimRotation * equippedWeapon.projectilePrefab.transform.rotation;

        GameObject projectile = Instantiate( equippedWeapon.projectilePrefab, bowArrowPoint.position,arrowRotation
        );

        ArrowScript arrowScript = projectile.GetComponent<ArrowScript>();

        if (arrowScript == null)
        {
            Destroy(projectile);
            Debug.LogError("Projectile prefab needs ArrowScript!");
            return;
        }

        arrowScript.SetProjectile(equippedWeapon.projectileSpeed,equippedWeapon.damage, transform);

        if (!inventory.RemoveItem(equippedWeapon.ammunitionData, ammoRequired))
        {
            Destroy(projectile);
            return;
        }

        bowArrowReleased = true;

        Destroy(currentArrowObject);
        currentArrowObject = null;

        Debug.Log("Arrow fired!");
    }
    private void PlayBowLoad()
    {
        if (equippedWeapon == null || equippedWeapon.weaponType != WeaponType.Bow) return;

        if (string.IsNullOrEmpty(equippedWeapon.bowLoadAnimation))
        {
            Debug.LogWarning("Bow Load Animation is not assigned!");
            return;
        }

        playerAnimation.SetExternalAnimationControl(true);
        playerAnimation.ChangeAnimation(equippedWeapon.bowLoadAnimation);
    }

    private void PlayBowRelease()
    {
        if (equippedWeapon == null ||
            equippedWeapon.weaponType != WeaponType.Bow)
            return;

        if (isThrowing || isAttacking || isBowReleasing)
            return;

        // Shoot immediately when right-click is released.
        FireBowArrow();

        if (!string.IsNullOrEmpty(equippedWeapon.bowReleaseAnimation))
        {
            playerAnimation.SetCombatIdleAnimation(
                equippedWeapon.bowCombatIdleAnimation
            );

            isBowReleasing = true;

            playerAnimation.SetExternalAnimationControl(true);

            playerAnimation.ChangeAnimation(
                equippedWeapon.bowReleaseAnimation
            );
        }
        else
        {
            playerMovement.UnlockMovement();
            playerAnimation.ResumeFromPlayerInput();
        }
    }

    // THROWABLE


    private void ThrowItem()
    {
        if (equippedWeapon == null || isThrowing)
            return;

        if (equippedWeapon.projectilePrefab == null)
        {
            Debug.LogWarning("Projectile Prefab is missing.");
            return;
        }

        if (inventory.GetItemCount(equippedWeapon) <= 0)
        {
            UnequipWeapon();
            return;
        }

        projectileReleased = false;
        isThrowing = true;

        playerAnimation.SetExternalAnimationControl(true);

        playerAnimation.ChangeAnimation(equippedWeapon.throwAnimation);
    }

    // Animation Event: OnThrowRelease
    public void OnThrowRelease()
    {
        if (projectileReleased || !isThrowing || equippedWeapon == null)
            return;

        if (inventory.GetItemCount(equippedWeapon) <= 0)
            return;

        if (equippedWeapon.projectilePrefab == null)
            return;

        projectileReleased = true;

        Vector3 position = throwPoint != null? throwPoint.position: transform.position + transform.forward;
        Vector3 direction = transform.forward;

        GameObject projectile = Instantiate(equippedWeapon.projectilePrefab,position, Quaternion.LookRotation(direction));

        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.Launch( direction, equippedWeapon.projectileSpeed);
        }

        ClearHandObject();

        bool removed = inventory.RemoveItem(equippedWeapon, 1);

        if (!removed)
        {
            Destroy(projectile);
            Debug.LogWarning("Could not remove item from inventory.");
            return;
        }

        Debug.Log("Thrown: " + equippedWeapon.itemName);
        Debug.Log("Remaining quantity: " + inventory.GetItemCount(equippedWeapon));
    }

    // Animation Event: OnThrowAnimationFinished
    public void OnThrowAnimationFinished()
    {
        if (!isThrowing || equippedWeapon == null)
            return;

        WeaponData thrownWeapon = equippedWeapon;

        isThrowing = false;
        projectileReleased = false;

        if (inventory.GetItemCount(thrownWeapon) > 0)
        {
            Transform holder = GetHandHolder(thrownWeapon);

            if (thrownWeapon.handPrefab != null && holder != null)
                SpawnHandObject(thrownWeapon.handPrefab, holder);

            playerAnimation.ResumeFromPlayerInput();
        }
        else
        {
            UnequipWeapon();
        }

        Debug.Log("Throw completed. Remaining: " + inventory.GetItemCount(thrownWeapon));
    }
}
