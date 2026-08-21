using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovimiento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform camara;
    private CharacterController controlador;

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;

    [Header("Gravedad")]
    [SerializeField] private float gravedadDelJugador = -9.81f; // Adaptado al estándar físico
    private Vector3 velocidadVertical;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();

        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;
    }

    void Update()
    {
        // Unificamos todo en un solo método para hacer un único Move() al final
        MoverJugador();
    }

    private void MoverJugador()
    {
        // 1. Comprobamos el suelo ANTES de aplicar gravedad
        if (controlador.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f; // Lo mantiene pegado al piso suavemente
        }

        // 2. Capturamos las teclas (AWSD y Flechas)
        float valorHorizontal = Input.GetAxisRaw("Horizontal");
        float valorVertical = Input.GetAxisRaw("Vertical");

        // Calculamos hacia donde mira la cámara
        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;

        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        // 3. Vector de movimiento en el plano
        Vector3 direccionPlano = (derechaCamara * valorHorizontal + adelanteCamara * valorVertical);

        // Normalizar EVITA que ir en diagonal sea más rápido
        if (direccionPlano.sqrMagnitude > 0.01f)
            direccionPlano.Normalize();

        // Vector final de movimiento en XZ
        Vector3 desplazamientoXZ = direccionPlano * velocidadMovimiento;

        // 4. Aplicamos fuerza de gravedad en Y
        velocidadVertical.y += gravedadDelJugador * Time.deltaTime;

        // 5. Unificamos ambos vectores y hacemos UN SOLO Move()
        Vector3 movimientoFinal = desplazamientoXZ + velocidadVertical;
        
        // Multiplicamos por deltaTime solo al momento de mover
        controlador.Move(movimientoFinal * Time.deltaTime);
    }
}