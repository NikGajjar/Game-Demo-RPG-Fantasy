
using UnityEngine;

public class AnimScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Blend Tree Settings")]
    [SerializeField] private string animationState = "AnimationTree";
    [SerializeField] private string moveXParameter = "MoveX";
    [SerializeField] private string moveYParameter = "MoveY";
    [SerializeField] private float blendDampTime = 0.1f;

    [Header("Animation Transition")]
    [SerializeField] private float transitionTime = 0.15f;

    private bool isJumping;
    private bool isLanding;
    private bool externalAnimationControl;

    private int moveXHash;
    private int moveYHash;
    private int locomotionHash;

    private string currentAnimation = "";
    private string combatIdleAnimation = "";

    private Health health;
    private CombatController combatController;
    private bool isDying;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (health == null)
            health = GetComponentInParent<Health>();

        combatController = GetComponent<CombatController>();

        if (combatController == null)
            combatController = GetComponentInParent<CombatController>();
        // Find Animator
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Find PlayerMovement on parent
        if (playerMovement == null)
            playerMovement = GetComponentInParent<PlayerMovement>();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        // Cache Animator hashes
        moveXHash = Animator.StringToHash(moveXParameter);
        moveYHash = Animator.StringToHash(moveYParameter);
        locomotionHash = Animator.StringToHash(animationState);

        //animator.SetLayerWeight(1, 1f); 

    }

    private void Start()
    {
        if (animator == null || playerMovement == null)
        {
            Debug.LogError(
                "AnimScript: Animator or PlayerMovement is missing!"
            );

            enabled = false;
            return;
        }

        if (!animator.HasState(0, locomotionHash))
        {
            Debug.LogError(
                "AnimScript: AnimationTree state not found!"
            );

            enabled = false;
            return;
        }

     

        UpdateBlendTree();

        PlayLocomotion();
    }

    private void Update()
    {
        if (animator == null || playerMovement == null)
            return;

        // Always update Blend Tree values from player input.
        UpdateBlendTree();

        // Do not interrupt special animations.
        if (isJumping || isLanding || externalAnimationControl)
            return;

        // Airborne animations are controlled separately.
        if (!playerMovement.isGrounded)
            return;

        Vector2 input = playerMovement.GetMovement();

        if (input.magnitude > 0.1f)
        {
            PlayLocomotion();
        }
        else
        {
            PlayIdle();
        }
    }


    // BLEND TREE
   

    private void UpdateBlendTree()
    {
        Vector2 input = playerMovement.GetMovement();

        float multiplier = playerMovement.IsRunning ? 2f : 1f;

        animator.SetFloat(moveXHash, input.x * multiplier, blendDampTime, Time.deltaTime );
        animator.SetFloat(moveYHash, input.y * multiplier, blendDampTime, Time.deltaTime);


    }

    private void PlayLocomotion()
    {
        if (currentAnimation == animationState)
            return;

        currentAnimation = animationState;

        animator.CrossFade(locomotionHash, transitionTime);
    }

    private void PlayIdle()
    {
        // The Blend Tree handles normal idle.
        // A weapon-specific combat idle can override it.
        if (!string.IsNullOrEmpty(combatIdleAnimation))
        {
            ChangeAnimation(combatIdleAnimation);
        }
        else
        {
            PlayLocomotion();
        }
    }


    private int bowLayerIndex = 1;


    public void StopBowLayer()
    {
        animator.SetLayerWeight(bowLayerIndex, 0f);
    }


    // COMBAT IDLE


    public void SetCombatIdleAnimation(string animationName)
    {
        combatIdleAnimation = animationName;
    }

    public void ClearCombatIdleAnimation()
    {
        combatIdleAnimation = "";
    }


    // JUMP / FALL / LAND


    public void PlayJump()
    {
        if (isJumping)
            return;
        
        isJumping = true;
        isLanding = false;

        ChangeAnimation("JumpB");
    }

    public void PlayFall()
    {
        if (!isJumping || isLanding)
            return;

        ChangeAnimation("Fall");
    }

    public void PlayLand()
    {
        if (!isJumping || isLanding)
            return;

        isLanding = true;

        ChangeAnimation("JumpL");
    }

    // Called by OnEnd after JumpL finishes.
    public void AnimationFinished()
    {
        // Death animation finished
        if (isDying)
        {
            isDying = false;

            if (health != null)
                health.Respawn();

            // Force transition out of Death / CombatDeath.
            currentAnimation = "";

            isJumping = false;
            isLanding = false;

            externalAnimationControl = false;

            if (animator != null)
                animator.CrossFade(locomotionHash, transitionTime);

            return;
        }

        // Normal jump landing finished
        isJumping = false;
        isLanding = false;

        ResumeFromPlayerInput();
    }

    // EXTERNAL ANIMATION CONTROL


    public void SetExternalAnimationControl(bool value)
    {
        externalAnimationControl = value;
    }

    public void ResumeLocomotion()
    {
        ResumeFromPlayerInput();
    }

    public void ResumeFromPlayerInput()
    {
        externalAnimationControl = false;

        currentAnimation = "";

        if (animator == null || playerMovement == null)
            return;

        UpdateBlendTree();

        if (!playerMovement.isGrounded)
            return;

        Vector2 input = playerMovement.GetMovement();

        if (input.magnitude > 0.1f)
        {
            PlayLocomotion();
        }
        else
        {
            PlayIdle();
        }
    }

   
    // GENERAL ANIMATION
   

    public void ChangeAnimation(string animationName)
    {
        if (string.IsNullOrEmpty(animationName) || animator == null)
            return;

        int animationHash = Animator.StringToHash(animationName);

        if (!animator.HasState(0, animationHash))
        {
            Debug.LogWarning( "Animation state not found: " + animationName );

            return;
        }

        if (currentAnimation == animationName)
            return;

        currentAnimation = animationName;

        animator.CrossFade( animationHash,  transitionTime );
    }

   
    // ANIMATION EVENTS
   

    public void OnThrowRelease()
    {
        CombatController combat =  GetComponentInParent<CombatController>();

        if (combat == null)
            combat = GetComponentInChildren<CombatController>();

        if (combat != null)
            combat.OnThrowRelease();

        else Debug.LogError("AnimScript: CombatController not found!");
    }

    public void OnThrowAnimationFinished()
    {
        CombatController combat = GetComponentInParent<CombatController>();

        if (combat == null)
            combat = GetComponentInChildren<CombatController>();

        if (combat != null)
            combat.OnThrowAnimationFinished();
        else
            Debug.LogError(
                "AnimScript: CombatController not found!"
            );
    }


    private void PlayDeathAnimation()
    {
        if (isDying)
            return;

        isDying = true;

        bool inCombat =
            combatController != null &&
            combatController.EquippedWeapon != null;

        SetExternalAnimationControl(true);

        if (inCombat)
            ChangeAnimation("CombatDeath");
        else
            ChangeAnimation("Death");
    }




    private void OnEnable()
    {
        if (health != null)
            health.OnDeath += PlayDeathAnimation;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDeath -= PlayDeathAnimation;
    }





}
