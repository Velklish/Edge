using UnityEngine;

public class BasicPlayerController : MonoBehaviour
{
    private PlayerControls playerInput;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    
    [SerializeField]
    private float playerSpeed = 2.0f;
    
    [SerializeField]
    private float jumpHeight = 1.0f;
    
    [SerializeField]
    private float gravityValue = -9.81f;

    [SerializeField] private bool receiveInput;
    
    public Animator animator;

    private Transform cameraMain;
    private Vector3 currentMove = Vector3.zero;

    public GameObject stunEffect;
    
    private void Awake()
    {
        playerInput = new PlayerControls();
        
    }

    private void OnEnable()
    {
        if (receiveInput)
        {
            playerInput.Enable();
        }
        
    }

    private void Start()
    {
        cameraMain = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        if (receiveInput)
        {
            playerInput.Enable();
        }
        else
        {
            playerInput.Disable();
        }
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movementInput = playerInput.Player.Move.ReadValue<Vector2>();

        if (groundedPlayer)
        {
            currentMove = (cameraMain.forward * movementInput.y + cameraMain.right * movementInput.x);
            currentMove.y = 0;
        }

        controller.Move(currentMove.normalized * (Time.deltaTime * playerSpeed));
        
        if (currentMove != Vector3.zero && groundedPlayer)
        {
            gameObject.transform.forward = currentMove;
        }
        
        // Changes the height position of the player..
        if (playerInput.Player.Jump.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (playerInput.Player.Attack.triggered)
        {
            Attack();
        }

        var speed = Mathf.Abs(movementInput.x) + Mathf.Abs(movementInput.y);
        
        animator.SetFloat("Speed", speed);
    }
    
    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.root != gameObject.transform.root)
        {
            switch (other.tag)
            {
                case "Crate": Stun(2.0f); break;
                default: break;
            }
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        print("Collision" + collision.gameObject);
    }
    
    public void Stun(float time)
    {
        Instantiate(stunEffect, gameObject.transform.Find("EffectSpot"));
        print("stunned");
        playerInput.Disable();
        Invoke(nameof(OnEnable), time);
    }
}
