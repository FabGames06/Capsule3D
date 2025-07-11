using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    private InputSystem_Actions controls;   // Référence au nouveau système d'Input
    private Rigidbody rb;                   // le rigidbody de la capsule
    private Vector2 moveInput;              // la valeur de mouvement retournée par le système d'Input
    private bool isGrounded;                // sommes-nous au sol ?
    private bool firstJump;                 // est-ce le premier saut ?
    private bool secondJump;                // est-ce le second saut ?
    private float speed = 1f;               // vitesse de déplacement pour normal et "sprint"
    [SerializeField]
    private GameObject leftArm;             // bras gauche pour l'animation
    [SerializeField]
    private GameObject rightArm;            // bras droit pour l'animation

    private void Awake()
    {
        // Instancie le système d'Input
        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Player.Enable(); // Activer l'Action Map "Player"
        controls.Player.Move.performed += OnMove; // Mouvement
        controls.Player.Move.canceled += OnMoveCancel; // Arrêt du mouvement
        controls.Player.Jump.performed += OnJump; // Saut
        controls.Player.Sprint.performed += OnSprint;
        controls.Player.Sprint.canceled += OnSprintCancelled;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMoveCancel;
        controls.Player.Jump.performed -= OnJump;
        controls.Player.Sprint.performed -= OnSprint;
        controls.Player.Sprint.canceled -= OnSprintCancelled;
        controls.Player.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    private void OnMoveCancel(InputAction.CallbackContext context)
    {
        // on donne un vecteur nul pour arreter le mouvement
        moveInput = new Vector2(0, 0);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * 6, ForceMode.Impulse);
            isGrounded = false;
            firstJump = true;
            leftArm.transform.rotation = Quaternion.Euler(0f, 0f, 45f); // Tourne de 45° autour de Z
            rightArm.transform.rotation = Quaternion.Euler(0f, 0f, -45f);   // idem -45° pour bras droit
        }
        else
            if(firstJump && !secondJump)
            {
                // gestion du double saut
                rb.AddForce(Vector3.up * 6, ForceMode.Impulse);
                secondJump = true;
            }
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        // on augmente la vitesse pour le "sprint" (touche left shift dans le tableau d'actions)
        speed = 5f;
    }

    private void OnSprintCancelled(InputAction.CallbackContext context)
    {
        // on remet la vitesse de base qd le "sprint" est terminé/bouton relaché
        speed = 1f;
    }

    void Update()
    {
        // comme le clavier gère x et y en déplacement, mais qu'en 3D les déplacemments se font en x et z (gauche, droite, avance, recule)
        // alors on met la valeur de y dans l'emplacement/l'axe de z ci-dessous
        Vector3 movement = new Vector3(moveInput.x * speed, 0, moveInput.y * speed);
        transform.Translate(movement * Time.deltaTime);
    }

    /*
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 newPosition = rb.position + movement * Time.fixedDeltaTime * 5f;
        rb.MovePosition(newPosition);
    }
    */

    private void OnCollisionEnter(Collision collision)
    {
        // permet d'activer le saut après une détection du sol en contact avec la capsule
        if(collision.gameObject.CompareTag("Plane"))
        {
            isGrounded = true;
            firstJump = false;
            secondJump = false;
            // les bras se mettent "droits"/straight
            leftArm.transform.rotation = Quaternion.Euler(0f, 0f, 90f); // Tourne de 90° autour de Z
            rightArm.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
    }

    void OnDrawGizmos()
    {
        // visible uniquement dans la scène
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
