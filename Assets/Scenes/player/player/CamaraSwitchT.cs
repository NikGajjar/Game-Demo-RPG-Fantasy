
using UnityEngine;

public class CameraSwitchT : MonoBehaviour
{
    public static CameraSwitchT Instance;

    [Header("References")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 75f;

    [Header("Camera Rotation")]
    [SerializeField] private float maxY = 60f;
    [SerializeField] private float minY = -40f;

    private float rotationX;

    private void Awake()
    {
        Instance = this;

        if (playerMovement == null)
            playerMovement = GetComponentInParent<PlayerMovement>();

        if (playerRoot == null && playerMovement != null)
            playerRoot = playerMovement.transform;
    }

    private void Start()
    {
        if (playerMovement == null)
        {
            Debug.LogError("CameraSwitchT: PlayerMovement reference missing!");
            enabled = false;
            return;
        }

        if (playerRoot == null)
        {
            Debug.LogError("CameraSwitchT: Player Root reference missing!");
            enabled = false;
            return;
        }

        // Start from the current FocusPoint rotation.
        rotationX = transform.localEulerAngles.x;

        if (rotationX > 180f)
            rotationX -= 360f;

        rotationX = Mathf.Clamp(rotationX, minY, maxY);
    }

    private void LateUpdate()
    {
        if (playerMovement == null || playerRoot == null)
            return;

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        CMovement();
    }

    public void CMovement()
    {
        Vector2 mouseInput = playerMovement.MouseDelta;

        // Horizontal mouse movement rotates the player.
        float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;

        playerRoot.Rotate(Vector3.up * mouseX, Space.World);

        // Vertical mouse movement rotates FocusPoint.
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp( rotationX, minY, maxY );
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }



    /* public static CameraSwitchT Instance;
     public float mouseSensitivity = 25f;
     public Vector2 MouseM;

     public float maxY = 30f;
     public float minY = -30f;

     private void Update()
     {
         CMovement();

     }


     public void CMovement()
     {

         MouseM.y += Input.GetAxis("Mouse Y") * (mouseSensitivity * 2) * Time.deltaTime;

         MouseM.y = Mathf.Clamp(MouseM.y, minY, maxY);

         transform.localRotation = Quaternion.Euler(-MouseM.y, 0f, 0f);


     }



     void Awake()
     {
         Instance = this;
     }*/

}