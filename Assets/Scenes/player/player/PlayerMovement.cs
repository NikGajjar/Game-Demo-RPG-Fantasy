using System;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    //  EVENTS 

    public event Action OnAttack;
    public event Action OnBlock;
    public event Action OnBlockReleased;

    public event Action OnDrop;

    public event Action OnSelectSword;
    public event Action OnSelectShield;
    public event Action OnSelectBow;
    public event Action OnSelectThrowable;


    [Header("Movement")]
    [SerializeField] private float MoveSpeed = 2f;
    [SerializeField] private float SprintSpeed = 5f;
    [SerializeField] private float AimingSpeed = 1f;
    [SerializeField] private float JumpForce = 6f;

    [Header("Ground Detection")]
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private float GroundRadius = 0.2f;
    [SerializeField] private LayerMask GroundLayer;

    [Header("References")]
    [SerializeField] private AnimScript playerAnimation;

    [Header("Inventory Interaction")]
    [SerializeField] private float pickupRadius = 2f;
    [SerializeField] private LayerMask pickupLayer;

    private Rigidbody rb;
    private PlayerInventory inventory;

    // Movement state
    public bool isGrounded;
    public bool wasGrounded;

    public Vector2 movement { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsMovementLocked { get; private set; }

    // Centralized mouse input for CameraSwitchT
    public Vector2 MouseDelta { get; private set; }

    // Centralized keyboard input
    public bool JumpPressed { get; private set; }
    public bool PickupPressed { get; private set; }
    public bool DropPressed { get; private set; }

    // Weapon selection
    public bool SelectSwordPressed { get; private set; }
    public bool SelectShieldPressed { get; private set; }
    public bool SelectBowPressed { get; private set; }
    public bool SelectThrowablePressed { get; private set; }

    // Combat input
    public bool AttackPressed { get; private set; }
    public bool BlockPressed { get; private set; }
    public bool BlockReleased { get; private set; }
    public bool BlockHeld { get; private set; }

    private float currentSpeed;
    private bool jumpRequested;
    private bool suppressMouseInputThisFrame;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        inventory = GetComponent<PlayerInventory>();

        if (rb == null) Debug.LogError("PlayerMovement: Rigidbody is missing!");
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerAnimation == null) playerAnimation = GetComponent<AnimScript>();

        if (playerAnimation == null) Debug.LogError("PlayerMovement: AnimScript reference missing!");
    }

    private void Update()
    {
        suppressMouseInputThisFrame = false;

        HandleCursor();
        ReadInputs();

        if (PickupPressed)   TryPickupItem();

        if (JumpPressed && isGrounded)   jumpRequested = true;

        gameSetting();
    }


    private void ReadInputs()
    {
        // MOVEMENT

        movement = new Vector2( Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")
        );

        movement = Vector2.ClampMagnitude(movement, 1f);

        IsRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = IsRunning ? SprintSpeed : MoveSpeed;

        //  KEYBOARD INPUT 

        JumpPressed = Input.GetKeyDown(KeyCode.Space);
        PickupPressed = Input.GetKeyDown(KeyCode.E);
        DropPressed = Input.GetKeyDown(KeyCode.G);

        // WEAPON SELECTION 

        SelectSwordPressed = Input.GetKeyDown(KeyCode.Alpha1);
        SelectShieldPressed = Input.GetKeyDown(KeyCode.Alpha2);
        SelectBowPressed = Input.GetKeyDown(KeyCode.Alpha3);
        SelectThrowablePressed = Input.GetKeyDown(KeyCode.Alpha4);

        //  MOUSE INPUT 

        bool mouseAvailable = Cursor.lockState == CursorLockMode.Locked && !suppressMouseInputThisFrame;

        AttackPressed = mouseAvailable && Input.GetMouseButtonDown(0);

        BlockPressed = mouseAvailable && Input.GetMouseButtonDown(1);
        BlockReleased = mouseAvailable && Input.GetMouseButtonUp(1);
        BlockHeld = mouseAvailable && Input.GetMouseButton(1);

        //  CAMERA INPUT 

        if (mouseAvailable)
        {
            MouseDelta = new Vector2(
                Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y")
            );
        }
        else
        {
            MouseDelta = Vector2.zero;
        }
 
        // INVOKE EVENTS AFTER READING INPUT

        if (AttackPressed)
            OnAttack?.Invoke();

        if (BlockPressed)
            OnBlock?.Invoke();

        if (BlockReleased)
            OnBlockReleased?.Invoke();

        if (DropPressed)
            OnDrop?.Invoke();

        if (SelectSwordPressed)
            OnSelectSword?.Invoke();

        if (SelectShieldPressed)
            OnSelectShield?.Invoke();

        if (SelectBowPressed)
            OnSelectBow?.Invoke();

        if (SelectThrowablePressed)
            OnSelectThrowable?.Invoke();
    }


    //  MOVEMENT 

    private void FixedUpdate()
    {
        wasGrounded = isGrounded;

        CheckGround();
        HandleMovement();
        HandleJump();
        HandleAirAnimations();
    }

    private void HandleMovement()
    {
        if (IsMovementLocked)
        {
            rb.linearVelocity = new Vector3( 0f, rb.linearVelocity.y, 0f);

            return;
        }

        if (rb == null)
            return;

        Vector3 direction = transform.right * movement.x +transform.forward * movement.y;

        Vector3 velocity = direction * currentSpeed;

        rb.linearVelocity = new Vector3( velocity.x, rb.linearVelocity.y, velocity.z );
    }

    public void LockMovement()
    {
        IsMovementLocked = true;
    }

    public void UnlockMovement()
    {
        IsMovementLocked = false;
    }

    //  

    private void HandleJump()
    {
        if (IsMovementLocked)
        {
            jumpRequested = false;
            return;
        }
        if (!jumpRequested || !isGrounded || rb == null)
            return;

        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);

        if (playerAnimation != null)
            playerAnimation.PlayJump();

        jumpRequested = false;
    }
    //  AIR 

    private void HandleAirAnimations()
    {
        if (playerAnimation == null)
            return;

        // While airborne
        if (!isGrounded)
        {
            if (rb.linearVelocity.y < 0f)
            {
                playerAnimation.PlayFall();
            }

            return;
        }

        if (isGrounded && !wasGrounded)
        {
            playerAnimation.PlayLand();
        }
    }


    //  GROUND 

    private void CheckGround()
    {
        if (GroundCheck == null)
            return;

        isGrounded = Physics.CheckSphere(GroundCheck.position,GroundRadius,GroundLayer,QueryTriggerInteraction.Ignore);
    }

    //  

    private void HandleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            suppressMouseInputThisFrame = true;
        }
    }

    //  PICKUP 

    private void TryPickupItem()
    {
        if (inventory == null)
            return;

        Collider[] nearbyItems = Physics.OverlapSphere(transform.position,pickupRadius, pickupLayer);

        ItemPickup closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider itemCollider in nearbyItems)
        {
            ItemPickup pickup = itemCollider.GetComponentInParent<ItemPickup>();

            if (pickup == null)
                continue;

            float distance = Vector3.Distance(transform.position, pickup.transform.position );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = pickup;
            }
        }

        if (closest != null)
            closest.TryPickup(inventory);
    }

    //  PUBLIC ACCESS 

    public Vector2 GetMovement()
    {
        return movement;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (GroundCheck == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GroundCheck.position, GroundRadius);
    }




    // Game Setting 
    public void gameSetting()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0f;
            Debug.Log("Game Paused!");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            Debug.Log("Scene Restarted!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        if (Time.timeScale == 0f && Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale = 1f;
            Debug.Log("Game Resumed!");

        }
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 1f)
        {

            Time.timeScale = 0f;
            //Application.Quit();
            Debug.Log("Game Quit!");
        }

        else if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 0f)
        {

            Time.timeScale = 1f;
            Debug.Log("Game Resumed!");
        }


    }



    /*   [SerializeField] private AnimScript player_Animation;


       private bool isGrounded;
       public Vector2 Xrotation;


       void Update()
       {
           PMovement();

       }



       public void PMovement()
       {

           jump();

           if (Input.GetKey(KeyCode.LeftShift))
           {
               PMInput(10f);
           }

           if (Input.GetKey(KeyCode.LeftControl))
           {
               PMInput(2f);
           }
           if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
           {
               PMInput(5f);
           }    
       }

       public void jump()
       {

           isGrounded = Physics.CheckSphere(transform.position, 0.1f, LayerMask.GetMask("Ground"));
           if(isGrounded)
           {
             if (Input.GetKeyDown(KeyCode.Space))
               {
                   GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
               }
           }

           Debug.Log(isGrounded ? "Touching Ground" : "In Air");
       }

       public void PMInput(float V) 
       {
           float horizontalInput = Input.GetAxis("Horizontal");
           float verticalInput = Input.GetAxis("Vertical");
           Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
           transform.Translate(movement * Time.deltaTime * V);


           Xrotation.x += Input.GetAxis("Mouse X") * CameraSwitchT.Instance.mouseSensitivity * Time.deltaTime;

           transform.localRotation = Quaternion.Euler(0f, Xrotation.x, 0f);
       }
   */



}
