using UnityEngine; // Allows use of Unity Engine API

[RequireComponent(typeof(CharacterController))] // Ensures GameObject has a CharacterController component
public class PlayerMovement : MonoBehaviour // Defines PlayerMovement class derived from MonoBehaviour
{ // Start of class definition
    [Header("Movement Settings")] // Adds a header in the Inspector for grouping variables
    public float speed = 6f; // Movement speed in units per second
    public float jumpHeight = 2f; // Height the player will jump
    public float gravity = -9.81f; // Gravity acceleration in meters per second squared

    private CharacterController controller; // Reference to the CharacterController component
    private Vector3 velocity; // Velocity vector for handling movement and gravity
    public bool isGrounded; // Flag to check if player is touching the ground

    public Transform groundCheck; // Transform used to check if the player is on the ground
    public float groundDistance = 0.4f; // Radius of the sphere to check for ground
    public LayerMask groundMask; // Layer mask to specify what is considered ground

    void Start() // Called once when the script instance is being loaded
    { // Start of Start method
        controller = GetComponent<CharacterController>(); // Gets and stores the CharacterController component
    } // End of Start method

    void Update() // Called once per frame
    { // Start of Update method
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // Performs ground check
        JumpInput();
        MovementInputs();
    } // End of Update method

    public void MovementInputs()
    {
        float x = Input.GetAxis("Horizontal"); // Gets horizontal input (A/D, Left/Right)
        float z = Input.GetAxis("Vertical"); // Gets vertical input (W/S, Up/Down)

        Vector3 move = transform.right * x + transform.forward * z; // Calculates movement direction in world space
        controller.Move(move * speed * Time.deltaTime); // Moves the character controller based on input and speed
    }
    public void JumpInput()
    {
       
        if (isGrounded && velocity.y < 0) // If grounded and moving downward
        { // Start of if block
            velocity.y = -2f; // Applies a small downward force to keep the player grounded
        } // End of if block

        if (Input.GetButtonDown("Jump") && isGrounded) // Checks for jump input and if player is grounded
        { // Start of jump if block
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calculates the initial jump velocity
        } // End of jump if block

        velocity.y += gravity * Time.deltaTime; // Applies gravity to the velocity over time
        controller.Move(velocity * Time.deltaTime); // Moves the character controller based on gravity and time
    }

    void OnDrawGizmosSelected() // Visualize ground check sphere in Editor
    { // Start of Gizmos method
        if (groundCheck == null) return; // If no groundCheck set, do nothing
        Gizmos.color = Color.yellow; // Set gizmo color
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance); // Draws a wire sphere at groundCheck position
    } // End of Gizmos method
} // End of class
