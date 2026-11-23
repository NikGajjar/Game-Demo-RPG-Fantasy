using UnityEngine;

public class Player2Movement : MonoBehaviour
{
   /* [SerializeField] private AnimScript player_Animation;
    [SerializeField] public Animator animator;


    [SerializeField] private float MoveSpeed ;
    [SerializeField] private float JumpForce ;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private BoxCollider playerCollider;

    public bool isGrounded;
    public bool inCombat = false;
    public Vector2 movement = Vector2.zero;
    private float MS;

    private Rigidbody rb;
    public static Player2Movement Instance;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }




    void Update()
    {
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        HandleJump();

        CameraMove();
    }




    void FixedUpdate()
    {
    
        InPutMenager();

        Vector3 velocity = MS * (movement.x * transform.right + movement.y * transform.forward);
    
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        isGrounded = Physics.CheckSphere(transform.position, 0.1f, LayerMask.GetMask("Ground"));

    }




    private void HandleJump()
    {

        
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
        
        
        Debug.Log(isGrounded ? "Touching Ground" : "In Air");
    }



    private void CameraMove()
    {

        Vector2 mouse = CameraSwitchT.Instance.mouseSensitivity * Time.deltaTime * new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        transform.Rotate(Vector3.up * mouse.x);

    }





    public void InPutMenager()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MS = MoveSpeed / 2f;
        }
        else
        {
            MS = MoveSpeed;
        }

        //if(Input.GetMouseButtonDown(0))
        //{
        //    inCombat = true;
        //}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Cursor.lockState == CursorLockMode.None && Input.GetKeyDown(KeyCode.O))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

    }









    void Awake()
    {
        Instance = this;
    }
*/

    /* Work But not good for player movement because it is not smooth and not good for player animation.
     * 
     * public float Mspeed = 10;
     public Vector3 VD3;

     float Xinput, Yinput;

     private bool isGrounded;

     public Rigidbody rb;



     void Update()
     {
         jump();
         Pmovement();
     }


     public void Pmovement()
     {
         Xinput = Input.GetAxis("Horizontal");
         Yinput = Input.GetAxis("Vertical");



         if (Input.GetKey(KeyCode.LeftShift))
         {
             Mspeed = 20f;
         }
         if(Input.GetKey(KeyCode.LeftControl))
         {
             Mspeed = 4f;
         }
         if(!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
         {
             Mspeed = 10f;

         }

         VD3 = new Vector3(Xinput, 0, Yinput) * Mspeed * Time.deltaTime;
         rb.MovePosition(transform.position + VD3);

     }

     public void jump()
     {

         isGrounded = Physics.CheckSphere(transform.position, 0.1f, LayerMask.GetMask("Ground"));
         if (isGrounded)
         {
             if (Input.GetKeyDown(KeyCode.Space))
             {
                 GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
             }
         }

         Debug.Log(isGrounded ? "Touching Ground" : "In Air");
     }*/
}